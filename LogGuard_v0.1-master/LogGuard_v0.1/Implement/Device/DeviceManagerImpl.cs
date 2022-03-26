using LogGuard_v0._1.Base.AsyncTask;
using LogGuard_v0._1.Base.Device;
using LogGuard_v0._1.Base.UIEventHandler.Action.Executer;
using LogGuard_v0._1.Implement.LogGuardFlow.StateController;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.Models;
using LogGuard_v0._1.Windows.MainWindow.Models.Builder;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Device;
using LogGuard_v0._1.Windows.WaitingWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.Device
{
    public class DeviceManagerImpl : IDeviceManager
    {
        private static DeviceManagerImpl _instance;
        private List<IDeviceHolder> _deviceHolders;
        private RangeObservableCollection<DeviceItemViewModel> _deviceSource;
        private Thread _scanDeviceThread;
        private Process _process;
        private DeviceItemViewModel _selectedDevice;

        public event FinishScanDeviceHandler FinishScanDevice;
        public event SelectedDeviceChangedHandler SelectedDeviceChanged;
        public event SerialPortChangedHandler SerialPortChanged;
        public event SelectedDeviceUnplugedHandler SelectedDeviceUnpluged;
        
        public RangeObservableCollection<DeviceItemViewModel> DeviceSource => _deviceSource;
        public List<IDeviceHolder> DeviceHolders => _deviceHolders;
        public DeviceItemViewModel SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                var oldVal = _selectedDevice;
                _selectedDevice = value;
                if (oldVal == null)
                {
                    SelectedDeviceChanged?.Invoke(this, new SelectedDeviceChangedEventArgs(null, value));
                }
                else if (!oldVal.Equals(_selectedDevice))
                {
                    SelectedDeviceChanged?.Invoke(this, new SelectedDeviceChangedEventArgs(oldVal, value));
                }
            }
        }

        private DeviceManagerImpl()
        {
            _deviceHolders = new List<IDeviceHolder>();
            _deviceSource = new RangeObservableCollection<DeviceItemViewModel>();
            SerialPortService.PortsChanged -= OnSerialPortChanged;
            SerialPortService.PortsChanged += OnSerialPortChanged;
        }

        private void OnSerialPortChanged(object sender, PortsChangedArgs e)
        {
            var selectedDevice = SelectedDevice;
            DeviceSource.Clear();
            SerialPortChanged?.Invoke(this, e);
            if(e.EventType == EventType.Removal)
            {
                UpdateListDevicesWhenSerialPortRemoved(selectedDevice);
            }
            else
            {
                UpdateListDevicesWhenSerialPortInserted();
            }
        }

        public void UpdateListDevices()
        {
            var resMes = App.Current.OpenWaitingTaskBox("Finding your device(s)!"
                           , "Scanning"
                           , async (param, token) =>
                           {
                               var result = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);

                               var res = await Task.Run(StartScanDevice, token);
                               if (token.IsCancellationRequested)
                               {
                                   result.MesResult = MessageAsyncTaskResult.Aborted;
                               }
                               else
                               {
                                   result.Result = res;
                                   result.MesResult = MessageAsyncTaskResult.Done;
                               }
                               return result;
                           }
                           , null
                           , (param, result) =>
                           {
                               var waitingBox = param as WaitingBox;
                               var lstDevice = result.Result as IEnumerable<DeviceItemViewModel>;
                               if (result.MesResult == MessageAsyncTaskResult.Done)
                               {
                                   DeviceSource.Clear();
                                   DeviceSource.AddNewRange(lstDevice);

                                   waitingBox?.UpdateMessageAndTitle("Found " + lstDevice?.Count() + " device(s)", "Done");
                               }
                           }
                           , 5000);
        }

        public void AddDeviceHolder(IDeviceHolder holder)
        {
            _deviceHolders.Add(holder);
            holder.DevicesSource = DeviceSource;
        }

        public void RemoveDeviceHolder(IDeviceHolder holder)
        {
            _deviceHolders.Remove(holder);
        }

        public void ForceUpdateListDevices()
        {
            CleanUp();
            _scanDeviceThread = new Thread(() =>
            {
                DeviceSource.Clear();
                var devices = StartScanDevice();
                DeviceSource.AddNewRange(devices);
            });
            _scanDeviceThread.Start();
        }

        private void UpdateListDevicesWhenSerialPortRemoved(DeviceItemViewModel selectedDevice)
        {
            CleanUp();
            _scanDeviceThread = new Thread(() =>
            {
                var devices = StartScanDevice();
                bool shouldNotifySelectedDeviceUnplug = false;
                if (selectedDevice == null)
                {
                    shouldNotifySelectedDeviceUnplug = false;
                }
                else
                {
                    shouldNotifySelectedDeviceUnplug = true;
                    foreach (var device in devices)
                    {
                        if (device.SerialNumber.Equals(selectedDevice.SerialNumber))
                        {
                            shouldNotifySelectedDeviceUnplug = false;
                            break;
                        }
                    }
                }
                if (shouldNotifySelectedDeviceUnplug)
                {
                    SelectedDeviceUnpluged?.Invoke(this, EventArgs.Empty);
                }

                DeviceSource.AddNewRange(devices);

            });
            _scanDeviceThread.Start();
        }

        private void UpdateListDevicesWhenSerialPortInserted()
        {
            CleanUp();
            _scanDeviceThread = new Thread(() =>
            {
                // Pause for port loading
                Thread.Sleep(300);
                var devices = StartScanDevice();
                DeviceSource.AddNewRange(devices);

            });
            _scanDeviceThread.Start();
        }

        public static DeviceManagerImpl Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DeviceManagerImpl();
                }
                return _instance;
            }
        }

        private void CleanUp()
        {
            if (_scanDeviceThread != null && _scanDeviceThread.IsAlive)
            {
                _scanDeviceThread.Interrupt();
                _scanDeviceThread.Abort();
            }
            if (_process != null)
            {
                try
                {
                    if (!_process.HasExited)
                    {
                        _process.Kill();
                    }
                }
                catch { }
                finally
                {
                    _process.Dispose();
                    _process.Close();
                }

            }
        }

        private IEnumerable<DeviceItemViewModel> StartScanDevice()
        {
            string[] stringSeparators = new string[] { "\r\n" };
            List<string> serialNumbers = new List<string>();
            var dIVMs = new List<DeviceItemViewModel>();

            try
            {
                _process = DeviceCmdExecuterImpl.Current.CreateProcess(DeviceCmdContact.CMD_DEVICES);
                _process.Start();
                _process.WaitForExit();
                ProcessManagement.GetInstance().AddNewProcessID(_process.Id);


                string line;
                _process.StandardOutput.ReadLine();
                while ((line = _process.StandardOutput.ReadLine()) != null)
                {
                    string tmp = line.Split('\t')[0];
                    if (tmp != "")
                        serialNumbers.Add(tmp);
                }
                if (serialNumbers != null && serialNumbers.Count > 0)
                {
                    for (int i = 0; i < serialNumbers.Count; i++)
                    {
                        string serialNumber = serialNumbers[i];
                        _process.StartInfo.Arguments = DeviceCmdExecuterImpl.Current.CreateCommandADB(DeviceCmdContact.CMD_BUILD_NUMBER, DeviceCmdContact.ADB_SHELL_COMMAND_TYPE, false, true, serialNumber);
                        _process.Start();
                        _process.WaitForExit();
                        ProcessManagement.GetInstance().AddNewProcessID(_process.Id);

                        string tmp = _process.StandardOutput.ReadLine();
                        string[] buildNumber = tmp.Split(stringSeparators, StringSplitOptions.None);

                        DeviceBuilder dvBuilder = new DeviceBuilder();
                        var dvInfo = dvBuilder.BuildBNumber(buildNumber[0])
                                .BuildSerialNumber(serialNumber)
                                .Build();
                        var deviceVM = new DeviceItemViewModel(dvInfo);
                        dIVMs.Add(deviceVM);
                    }
                }

                if (!_process.HasExited)
                    _process.Kill();
            }
            catch { }
            finally
            {
                _process.Dispose();
                _process.Close();
                FinishScanDevice?.Invoke(this, EventArgs.Empty);
            }
            return dIVMs;
        }

        ~DeviceManagerImpl()
        {
            SerialPortService.CleanUp();
        }

        #region SerialPortService
        private static class SerialPortService
        {
            private static string[] _serialPorts;

            private static ManagementEventWatcher arrival;

            private static ManagementEventWatcher removal;

            static SerialPortService()
            {
                _serialPorts = GetAvailableSerialPorts();
                MonitorDeviceChanges();
            }

            /// <summary>
            /// If this method isn't called, an InvalidComObjectException will be thrown (like below):
            /// System.Runtime.InteropServices.InvalidComObjectException was unhandled
            ///Message=COM object that has been separated from its underlying RCW cannot be used.
            ///Source=mscorlib
            ///StackTrace:
            ///     at System.StubHelpers.StubHelpers.StubRegisterRCW(Object pThis, IntPtr pThread)
            ///     at System.Management.IWbemServices.CancelAsyncCall_(IWbemObjectSink pSink)
            ///     at System.Management.SinkForEventQuery.Cancel()
            ///     at System.Management.ManagementEventWatcher.Stop()
            ///     at System.Management.ManagementEventWatcher.Finalize()
            ///InnerException: 
            /// </summary>
            public static void CleanUp()
            {
                arrival.Stop();
                removal.Stop();
            }

            public static event EventHandler<PortsChangedArgs> PortsChanged;

            private static void MonitorDeviceChanges()
            {
                try
                {
                    var deviceArrivalQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
                    var deviceRemovalQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");

                    arrival = new ManagementEventWatcher(deviceArrivalQuery);
                    removal = new ManagementEventWatcher(deviceRemovalQuery);

                    arrival.EventArrived += (o, args) => RaisePortsChangedIfNecessary(EventType.Insertion);
                    removal.EventArrived += (sender, eventArgs) => RaisePortsChangedIfNecessary(EventType.Removal);

                    // Start listening for events
                    arrival.Start();
                    removal.Start();
                }
                catch (ManagementException err)
                {

                }
            }

            private static void RaisePortsChangedIfNecessary(EventType eventType)
            {
                lock (_serialPorts)
                {
                    var availableSerialPorts = GetAvailableSerialPorts();
                    try
                    {
                        /*Waiting for thread checking device*/
                        Thread.Sleep(300);

                        if (!_serialPorts.SequenceEqual(availableSerialPorts))
                        {
                            _serialPorts = availableSerialPorts;
                            PortsChanged.Invoke(null, new PortsChangedArgs(eventType, _serialPorts));
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            public static string[] GetAvailableSerialPorts()
            {
                return SerialPort.GetPortNames();
            }
        }

        public enum EventType
        {
            Insertion,
            Removal,
        }

        public class PortsChangedArgs : EventArgs
        {
            private readonly EventType _eventType;

            private readonly string[] _serialPorts;

            public PortsChangedArgs(EventType eventType, string[] serialPorts)
            {
                _eventType = eventType;
                _serialPorts = serialPorts;
            }

            public string[] SerialPorts
            {
                get
                {
                    return _serialPorts;
                }
            }

            public EventType EventType
            {
                get
                {
                    return _eventType;
                }
            }
        }

        #endregion
    }


}
