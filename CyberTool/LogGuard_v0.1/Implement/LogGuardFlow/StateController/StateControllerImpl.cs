﻿using LogGuard_v0._1._Config;
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
using System.IO;
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
        public RunThreadConfigManager RTCManager => RunThreadConfigManager.Current;

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
            LGSourceManager.UpdateLogParser(RTCManager.CurrentConfig);

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
                            LGSourceManager.AddItem(line);
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
                if (string.IsNullOrEmpty(RTCManager.CurrentParser.Cmd))
                {
                    App.Current.ShowWaringBox("Command not found, please reselect parser format!");
                    return false;
                }
                var cmd = DeviceCmdExecuterImpl
                    .Current
                    .CreateCommandADB(command: RTCManager.CurrentParser.Cmd
                        , type: DeviceCmdContact.ADB_NONE_SHELL_COMMAND_TYPE
                        , asroot: false
                        , multiDevice: true
                        , serialNumber: DeviceManager.SelectedDevice.SerialNumber.ToString());

                CaptureProc = DeviceCmdExecuterImpl
                    .Current
                    .CreateProcess(cmd);

                LGSourceManager.UpdateLogParser(RTCManager.CurrentConfig);
                RunningThread = new Thread(Running);
                DeviceManager.SelectedDeviceUnpluged -= OnSelectedDeviceUnpluged;
                DeviceManager.SelectedDeviceUnpluged += OnSelectedDeviceUnpluged;
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

        private void OnSelectedDeviceUnpluged(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                Stop();
                App.Current.Dispatcher.Invoke(() =>
                {
                    App.Current.ShowWaringBox(warning: "Your capturing device is unpluged",
                        isDialog: false);
                });

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
                if (RUNE.IS_SUPPORT_HIGH_CPU_LOG_CAPTURE)
                    return HighCpu_StateController.Current;
                else
                    return LowCpu_StateController.Current;
            }
        }

    }
}
