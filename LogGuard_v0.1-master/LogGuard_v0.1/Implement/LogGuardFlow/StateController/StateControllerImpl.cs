using LogGuard_v0._1.Base.Device;
using LogGuard_v0._1.Base.Log;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Implement.AndroidLog;
using LogGuard_v0._1.Implement.Device;
using LogGuard_v0._1.Implement.LogGuardFlow.RunThreadConfig;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using LogGuard_v0._1.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.LogGuardFlow.StateController
{
    public abstract class StateControllerImpl : IStateController
    {
        private LogGuardState _currentState;
        private LogGuardState _previousState;
        private object _syncObject;
        public LogGuardState CurrentState { get => _currentState; set => _currentState = value; }
        public LogGuardState PreviousState { get => _previousState; set => _previousState = value; }
        public object SynchronizeStateObject { get => _syncObject; set => _syncObject = value; }
        public IRunThreadConfig RunThreadConfig => RunThreadConfigImpl.Current;

        public bool IsRunning { get; private set; }
        public bool IsPausing { get; private set; }
        public bool IsStop { get; private set; }

        public ISourceManager LGSourceManager => SourceManagerImpl.Current;
        public IDeviceManager DeviceManager => DeviceManagerImpl.Current;


        public event StateChangedHandler StateChanged;

        protected Thread RunningThread;
        protected Process CaptureProc;

        protected StateControllerImpl()
        {
            _syncObject = new object();
            CurrentState = LogGuardState.STOP;
            PreviousState = LogGuardState.NONE;
            App.Current.OnMainWindowClosing += StateControllerMainWindowClosing;
        }

        private void StateControllerMainWindowClosing(object sender, EventArgs e)
        {
            // Force stop when close main window
            Stop();
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
                lock (CaptureProc)
                {
                    if (!CaptureProc.HasExited)
                        CaptureProc.Kill();
                    CaptureProc.Dispose();
                    CaptureProc.Close();
                    CaptureProc = null;
                }
            }

            bool isNeedNotifyStateChange = CurrentState != LogGuardState.STOP;
            UpdateStopState();
            if (isNeedNotifyStateChange)
            {
                StateChanged?.Invoke(this, new StateChangedEventArgs(CurrentState, PreviousState));
            }
        }

        public bool Start()
        {
            if (CurrentState == LogGuardState.STOP || CurrentState == LogGuardState.NONE)
            {
                if (DeviceManager.DeviceSource.Count == 0)
                {
                    App.Current.ShowWaringBox("No devices found!");
                    return false;
                }
                if (DeviceManager.SelectedDevice == null)
                {
                    App.Current.ShowWaringBox("Please select a device!");
                    return false;
                }
                var cmd = DeviceCmdExecuterImpl
                    .Current
                    .CreateCommandADB(command: RunThreadConfig.LogParserFormat.Cmd
                        , type: DeviceCmdContact.ADB_NONE_SHELL_COMMAND_TYPE
                        , asroot: false
                        , multiDevice: true
                        , serialNumber: DeviceManager.SelectedDevice.SerialNumber.ToString());

                CaptureProc = DeviceCmdExecuterImpl
                    .Current
                    .CreateProcess(cmd);


                LGSourceManager.UpdateLogParser(RunThreadConfig);
                RunningThread = new Thread(Running);
            }

            OnStart();
            RunningThread.Start();

            bool isNeedNotifyStateChange = CurrentState != LogGuardState.RUNNING;
            UpdateRunningState();
            if (isNeedNotifyStateChange)
            {
                StateChanged?.Invoke(this, new StateChangedEventArgs(CurrentState, PreviousState));
            }
            return true;
        }

        private void Running()
        {
            lock (CaptureProc)
            {
                CaptureProc.Start();
                ProcessManagement.GetInstance().AddNewProcessID(CaptureProc.Id);
                OnRunning();
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

        public static StateControllerImpl Current
        {
            get
            {
                return HighCpu_StateController.Current;
            }
        }

    }
}
