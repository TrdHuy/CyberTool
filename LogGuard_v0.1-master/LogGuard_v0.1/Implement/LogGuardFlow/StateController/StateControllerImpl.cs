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


        public event StateChangedHandler StateChanged;

        protected Thread RunningThread;
        protected Process CaptureProc;

        protected StateControllerImpl()
        {
            _syncObject = new object();
            CurrentState = LogGuardState.STOP;
            PreviousState = LogGuardState.NONE;
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

        public void Start()
        {
            if (CurrentState == LogGuardState.STOP || CurrentState == LogGuardState.NONE)
            {
                CaptureProc = DeviceCmdExecuterImpl.Current.CreateProcess(RunThreadConfig.LogParserFormat.Cmd);
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
