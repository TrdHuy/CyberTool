using cyber_base.async_task;
using cyber_base.implement.utils;
using cyber_base.view.window;
using log_guard.@base.device;
using log_guard.@base.module;
using log_guard.implement.module;
using log_guard.implement.process;
using log_guard.models.info.builder;
using log_guard.view_models.device;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace log_guard.implement.device
{
    internal class DeviceManager : BaseLogGuardModule, IDeviceManager
    {
        private static CancellationTokenSource? _scanDeviceCTS;

        private List<IDeviceHolder> _deviceHolders;
        private RangeObservableCollection<IDeviceItem> _deviceSource;
        private IDeviceItem? _selectedDevice;

        public event FinishScanDeviceHandler? FinishScanDevice;
        public event SelectedDeviceChangedHandler? SelectedDeviceChanged;
        public event SerialPortChangedHandler? SerialPortChanged;
        public event SelectedDeviceUnplugedHandler? SelectedDeviceUnpluged;

        public RangeObservableCollection<IDeviceItem> DeviceSource => _deviceSource;
        public List<IDeviceHolder> DeviceHolders => _deviceHolders;

        public IDeviceItem? SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                var oldVal = _selectedDevice;
                _selectedDevice = value;
                if (oldVal == null)
                {
                    SelectedDeviceChanged?.Invoke(this, new SelectedDeviceChangedEventArgs(null, value as IDeviceItem));
                }
                else if (!oldVal.Equals(_selectedDevice))
                {
                    SelectedDeviceChanged?.Invoke(this, new SelectedDeviceChangedEventArgs(oldVal as IDeviceItem, value as IDeviceItem));
                }
            }
        }

        public DeviceManager()
        {
            _deviceHolders = new List<IDeviceHolder>();
            _deviceSource = new RangeObservableCollection<IDeviceItem>();
        }

        public override void OnModuleStart()
        {
            SerialPortService.PortsChanged -= OnSerialPortChanged;
            SerialPortService.PortsChanged += OnSerialPortChanged;
        }

        public override void OnModuleDestroy()
        {
            _scanDeviceCTS?.Cancel();
        }

        private void OnSerialPortChanged(object? sender, PortsChangedArgs e)
        {
            var selectedDevice = SelectedDevice;
            DeviceSource.Clear();
            SerialPortChanged?.Invoke(this, e);
            if (e.EventType == EventType.Removal && selectedDevice != null)
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
            var resMes = LogGuardService
                .Current
                .ServiceManager?
                .App
                .OpenWaitingTaskBox("Finding your device(s)!"
                           , "Scanning"
                           , async (param, result, token) =>
                           {
                               var waitingBox = param as IStandBox;
                               var res = StartScanDevice();
                               if (token.IsCancellationRequested)
                               {
                                   result.MesResult = MessageAsyncTaskResult.Aborted;
                               }
                               else
                               {
                                   result.Result = res;
                                   result.MesResult = MessageAsyncTaskResult.Done;
                               }
                               DeviceSource.Clear();

                               var sourceChangedCallback = new Action(() =>
                               {
                                   waitingBox?.UpdateMessageAndTitle("Found " + DeviceSource?.Count() + " device(s)", "Scanning!");
                               });

                               if (res != null)
                               {
                                   await DeviceSource.AddNewRangeAsync(res, sourceChangedCallback);
                               }

                               return result;
                           }
                           , null
                           , async (param, result) =>
                           {
                               var waitingBox = param as IStandBox;
                               var lstDevice = result.Result as IAsyncEnumerable<IDeviceItem>;
                               if (result.MesResult == MessageAsyncTaskResult.Done)
                               {
                                   waitingBox?.UpdateMessageAndTitle("Found " + DeviceSource?.Count() + " device(s)", "Done!");
                               }
                               return result;
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

        public async void ForceUpdateListDevices()
        {
            DeviceSource.Clear();
            var devices = StartScanDevice();
            if (devices != null)
            {
                await DeviceSource.AddNewRangeAsync(devices);
            }
        }

        private async void UpdateListDevicesWhenSerialPortRemoved(IDeviceItem selectedDevice)
        {
            var devices = StartScanDevice();
            var shouldNotifySelectedDeviceUnplug = false;
            if (selectedDevice == null)
            {
                shouldNotifySelectedDeviceUnplug = false;
            }
            else
            {
                shouldNotifySelectedDeviceUnplug = true;
                if (devices != null)
                {
                    await DeviceSource.AddNewRangeAsync(devices);

                    foreach (var device in DeviceSource)
                    {
                        if (device.SerialNumber.Equals(selectedDevice.SerialNumber))
                        {
                            shouldNotifySelectedDeviceUnplug = false;
                            break;
                        }
                    }
                }
            }
            if (shouldNotifySelectedDeviceUnplug)
            {
                SelectedDeviceUnpluged?.Invoke(this, EventArgs.Empty);
            }
        }

        private async void UpdateListDevicesWhenSerialPortInserted()
        {
            // Pause for port loading
            await Task.Delay(300);

            var devices = StartScanDevice();
            if (devices != null)
            {
                await DeviceSource.AddNewRangeAsync(devices);
            }
        }

        public static DeviceManager Current
        {
            get
            {
                return LogGuardModuleManager.DM_Instance;
            }
        }

        private IAsyncEnumerable<IDeviceItem>? StartScanDevice()
        {
            _scanDeviceCTS?.Cancel();
            _scanDeviceCTS = new CancellationTokenSource();
            IAsyncEnumerable<IDeviceItem>? list = null;
            if (_scanDeviceCTS != null)
            {
                list = StartScanDevice(_scanDeviceCTS);
            }
            return list;
        }

        private async IAsyncEnumerable<IDeviceItem> StartScanDevice(CancellationTokenSource token)
        {
            string[] stringSeparators = new string[] { "\r\n" };
            List<string> serialNumbers = new List<string>();

            using (var process = DeviceCmdExecuter
                .Current
                .CreateProcess(DeviceCmdContact.CMD_DEVICES))
            {
                process.Start();
                try
                {
                    await process.WaitForExitAsync(token.Token);
                }
                catch (OperationCanceledException e)
                {
                    HandleAbortDeviceScanRequest();
                    throw e;
                }
                ProcessManager.Current.AddNewProcessID(process.Id);
                string? line;
                process.StandardOutput.ReadLine();
                while ((line = process.StandardOutput.ReadLine()) != null)
                {
                    if (token.IsCancellationRequested)
                    {
                        HandleAbortDeviceScanRequest();
                        throw new OperationCanceledException();
                    }

                    string tmp = line.Split('\t')[0];
                    if (tmp != "")
                        serialNumbers.Add(tmp);
                }
            }

            if (serialNumbers != null && serialNumbers.Count > 0)
            {
                for (int i = 0; i < serialNumbers.Count; i++)
                {
                    string serialNumber = serialNumbers[i];
                    using (var process = DeviceCmdExecuter
                        .Current
                        .CreateProcess(
                            DeviceCmdExecuter
                            .Current
                            .CreateCommandADB(
                                DeviceCmdContact.CMD_BUILD_NUMBER
                                , DeviceCmdContact.ADB_SHELL_COMMAND_TYPE
                                , false
                                , true
                                , serialNumber)))
                    {
                        if (token.IsCancellationRequested)
                        {
                            HandleAbortDeviceScanRequest();
                            throw new OperationCanceledException();
                        }

                        process.StartInfo.Arguments = DeviceCmdExecuter.Current.CreateCommandADB(DeviceCmdContact.CMD_BUILD_NUMBER, DeviceCmdContact.ADB_SHELL_COMMAND_TYPE, false, true, serialNumber);
                        process.Start();

                        try
                        {
                            await process.WaitForExitAsync(token.Token);
                        }
                        catch (OperationCanceledException e)
                        {
                            HandleAbortDeviceScanRequest();
                            throw e;
                        }
                        ProcessManager.Current.AddNewProcessID(process.Id);

                        var tmp = process.StandardOutput.ReadLine();
                        if (tmp != null)
                        {
                            string[] buildNumber = tmp.Split(stringSeparators, StringSplitOptions.None);

                            DeviceBuilder dvBuilder = new DeviceBuilder();
                            var dvInfo = dvBuilder.BuildBNumber(buildNumber[0])
                                    .BuildSerialNumber(serialNumber)
                                    .Build();
                            var deviceVM = new DeviceItemViewModel(dvInfo);
                            yield return deviceVM;
                        }

                    }

                }

            }

            FinishScanDevice?.Invoke(this, EventArgs.Empty);
        }

        private void HandleAbortDeviceScanRequest()
        {
            FinishScanDevice?.Invoke(this, EventArgs.Empty);
        }


        #region SerialPortService
        private static class SerialPortService
        {
            private static Logger SPSLogger = new Logger("SerialPortService", "log_guard");

            private static string[] _serialPorts;

            private static ManagementEventWatcher? arrival;

            private static ManagementEventWatcher? removal;

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
                arrival?.Stop();
                removal?.Stop();
            }

            public static event EventHandler<PortsChangedArgs>? PortsChanged;

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
                    SPSLogger.E(err.ToString());
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
                            PortsChanged?.Invoke(null, new PortsChangedArgs(eventType, _serialPorts));
                        }
                    }
                    catch
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
