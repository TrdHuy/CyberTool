using log_guard.@base.device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.flow
{
    public enum LogGuardState
    {
        NONE = 0,
        RUNNING = 1,
        PAUSING = 2,
        STOP = 3,
    }
    internal interface IStateController
    {
        bool Start();
        void Stop();
        void Resume();
        void Pause();
        ISourceManager LGSourceManager { get; }
        IRunThreadConfigManager RTCManager { get; }
        IDeviceManager DeviceManager { get; }
        LogGuardState CurrentState { get; set; }
        LogGuardState PreviousState { get; set; }
        object SynchronizeStateObject { get; set; }

        event StateChangedHandler StateChanged;
    }

    public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);

    public class StateChangedEventArgs
    {
        public LogGuardState NewState { get; private set; }
        public LogGuardState OldState { get; private set; }

        public StateChangedEventArgs(LogGuardState newState, LogGuardState oldState)
        {
            NewState = newState;
            OldState = oldState;
        }
    }
}
