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
using System.Linq;
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

        public event FinishScanDeviceHandler FinishScanDevice;

        public RangeObservableCollection<DeviceItemViewModel> DeviceSource => _deviceSource;
        public List<IDeviceHolder> DeviceHolders => _deviceHolders;

        private DeviceManagerImpl()
        {
            _deviceHolders = new List<IDeviceHolder>();
            _deviceSource = new RangeObservableCollection<DeviceItemViewModel>();
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
                FinishScanDevice?.Invoke(this);
            }
            return dIVMs;
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
            _scanDeviceThread = new Thread(() =>
            {
                StartScanDevice();
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

    }
}
