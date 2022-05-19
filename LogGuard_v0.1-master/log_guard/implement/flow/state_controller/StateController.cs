using log_guard._config;
using log_guard.@base.device;
using log_guard.@base.flow;
using log_guard.@base.module;
using log_guard.definitions;
using log_guard.implement.device;
using log_guard.implement.flow.run_thread_config;
using log_guard.implement.flow.source_manager;
using log_guard.implement.module;
using log_guard.implement.process;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace log_guard.implement.flow.state_controller
{
    internal abstract class StateController : IStateController, ILogGuardModule
    {
        private LogGuardState _currentState;
        private LogGuardState _previousState;
        private object _syncObject;

        public LogGuardState CurrentState { get => _currentState; set => _currentState = value; }
        public LogGuardState PreviousState { get => _previousState; set => _previousState = value; }
        public object SynchronizeStateObject { get => _syncObject; set => _syncObject = value; }
        public IRunThreadConfigManager RTCManager => RunThreadConfigManager.Current;

        public bool IsRunning { get; private set; }
        public bool IsPausing { get; private set; }
        public bool IsStop { get; private set; }

        public ISourceManager LwSourceManager => SourceManager.Current;
        public IDeviceManager DcManager => DeviceManager.Current;

        public event StateChangedHandler? StateChanged;

        protected Thread? RunningThread;
        protected Process? CaptureProc;

        public StateController()
        {
            _syncObject = new object();
            CurrentState = LogGuardState.STOP;
            PreviousState = LogGuardState.NONE;

        }

        public void OnModuleStart()
        {

            if (LogGuardService.Current.ServiceManager != null)
            {
                LogGuardService.Current.ServiceManager.App.CyberApp.Exit -= StateControllerAppExit;
                LogGuardService.Current.ServiceManager.App.CyberApp.Exit += StateControllerAppExit;
            }
        }

        public void Pause()
        {
            OnPause();
            bool isNeedNotifyStateChange = CurrentState != LogGuardState.PAUSING;
            UpdatePausingState();
            if (isNeedNotifyStateChange)
            {
                StateChanged?.Invoke(this, new StateChangedEventArgs(CurrentState, PreviousState));
            }
        }

        public void Resume()
        {
            OnResume();
            bool isNeedNotifyStateChange = CurrentState != LogGuardState.RUNNING;
            UpdateRunningState();
            if (isNeedNotifyStateChange)
            {
                StateChanged?.Invoke(this, new StateChangedEventArgs(CurrentState, PreviousState));
            }
        }

        public void Stop()
        {
            OnStop();

            if (CaptureProc != null)
            {
                if (!CaptureProc.HasExited)
                    CaptureProc.Kill();
                CaptureProc.Dispose();
                CaptureProc.Close();
                CaptureProc = null;
            }

            bool isNeedNotifyStateChange = CurrentState != LogGuardState.STOP;
            UpdateStopState();
            if (isNeedNotifyStateChange)
            {
                StateChanged?.Invoke(this, new StateChangedEventArgs(CurrentState, PreviousState));
            }
        }

        public void StartImportLogFile(string filePath)
        {
            LwSourceManager.UpdateLogParser(RTCManager.CurrentConfig);

            RunningThread = new Thread(() =>
            {
                if (File.Exists(filePath))
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {

                        // Iterating the file
                        while (sr.Peek() >= 0)
                        {

                            // Read the data in the file until the peak
                            var line = sr.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                LwSourceManager.AddItem(line);
                            }
                        }
                    }
                }
            });
            RunningThread.Start();
        }

        public bool Start()
        {
            if (CurrentState == LogGuardState.STOP || CurrentState == LogGuardState.NONE)
            {
                if (DcManager.DeviceSource.Count == 0)
                {
                    LogGuardService.Current.ServiceManager?.App.ShowWaringBox("No devices found!");
                    return false;
                }
                if (DcManager.SelectedDevice == null)
                {
                    LogGuardService.Current.ServiceManager?.App.ShowWaringBox("Please select a device!");
                    return false;
                }
                if (string.IsNullOrEmpty(RTCManager.CurrentParser.Cmd))
                {
                    LogGuardService.Current.ServiceManager?.App.ShowWaringBox("Command not found, please reselect parser format!");
                    return false;
                }
                var cmd = DeviceCmdExecuter
                    .Current
                    .CreateCommandADB(command: RTCManager.CurrentParser.Cmd
                        , type: DeviceCmdContact.ADB_NONE_SHELL_COMMAND_TYPE
                        , asroot: false
                        , multiDevice: true
                        , serialNumber: DcManager.SelectedDevice.SerialNumber);

                CaptureProc = DeviceCmdExecuter
                    .Current
                    .CreateProcess(cmd);

                LwSourceManager.UpdateLogParser(RTCManager.CurrentConfig);
                RunningThread = new Thread(Running);
                DcManager.SelectedDeviceUnpluged -= OnSelectedDeviceUnpluged;
                DcManager.SelectedDeviceUnpluged += OnSelectedDeviceUnpluged;
            }

            OnStart();
            RunningThread?.Start();

            bool isNeedNotifyStateChange = CurrentState != LogGuardState.RUNNING;
            UpdateRunningState();
            if (isNeedNotifyStateChange)
            {
                StateChanged?.Invoke(this, new StateChangedEventArgs(CurrentState, PreviousState));
            }
            return true;
        }

        private void OnSelectedDeviceUnpluged(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                Stop();
                LogGuardService
                    .Current
                    .ServiceManager?.App.CyberApp.Dispatcher.Invoke(() =>
                {
                    LogGuardService
                    .Current
                    .ServiceManager.App.ShowWaringBox(warning: "Your capturing device is unpluged",
                        isDialog: false);
                });

            }
        }

        private void Running()
        {
            if (CaptureProc != null)
            {
                lock (CaptureProc)
                {
                    CaptureProc.Start();
                    ProcessManager.Current.AddNewProcessID(CaptureProc.Id);
                    OnRunning();
                }
            }
            
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract void OnResume();
        protected abstract void OnRunning();
        protected abstract void OnPause();

        private void UpdateRunningState()
        {
            IsRunning = true;
            IsStop = false;
            IsPausing = false;
            PreviousState = CurrentState;
            CurrentState = LogGuardState.RUNNING;
        }
        private void UpdatePausingState()
        {
            IsRunning = false;
            IsStop = false;
            IsPausing = true;
            PreviousState = CurrentState;
            CurrentState = LogGuardState.PAUSING;
        }

        private void UpdateStopState()
        {
            IsRunning = false;
            IsStop = true;
            IsPausing = false;
            PreviousState = CurrentState;
            CurrentState = LogGuardState.STOP;
        }

        private void StateControllerAppExit(object sender, System.Windows.ExitEventArgs e)
        {
            // Force stop when close app
            Stop();
        }

        public static StateController? Current
        {
            get
            {
                return LogGuardModuleManager.SC_Instance;
            }
        }


    }
}
