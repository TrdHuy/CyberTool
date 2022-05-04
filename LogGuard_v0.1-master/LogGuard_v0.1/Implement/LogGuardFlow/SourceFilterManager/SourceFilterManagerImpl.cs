using LogGuard_v0._1.Base.LogGuardFlow.SourceFilter;
using System.Threading;

namespace LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager
{
    public class SourceFilterManagerImpl : ISourceFilterManager
    {
        private static SourceFilterManagerImpl _instance;
        private event SourceFilterConditionChangedHandler _filterConditionChanged;
        private Thread NotifyFilterConditionChangedMessage { get; set; }

        private IMechanicalSourceFilter _logTagFilter;
        private IMechanicalSourceFilter _logTagRemoveFilter;
        private IMechanicalSourceFilter _logMessageFilter;
        private IMechanicalSourceFilter _logPidFilter;
        private IMechanicalSourceFilter _logTidFilter;
        private IMechanicalSourceFilter _logStartTimeFilter;
        private IMechanicalSourceFilter _logEndTimeFilter;
        private ISourceFilter _logLevelFilter;

        public IMechanicalSourceFilter LogTagRemoveFilter { get => _logTagRemoveFilter; set => _logTagRemoveFilter = value; }
        public IMechanicalSourceFilter LogTagFilter { get => _logTagFilter; set => _logTagFilter = value; }
        public IMechanicalSourceFilter LogMessageFilter { get => _logMessageFilter; set => _logMessageFilter = value; }
        public IMechanicalSourceFilter LogTidFilter { get => _logTidFilter; set => _logTidFilter = value; }
        public IMechanicalSourceFilter LogPidFilter { get => _logPidFilter; set => _logPidFilter = value; }
        public IMechanicalSourceFilter LogStartTimeFilter { get => _logStartTimeFilter; set => _logStartTimeFilter = value; }
        public IMechanicalSourceFilter LogEndTimeFilter { get => _logEndTimeFilter; set => _logEndTimeFilter = value; }
        public ISourceFilter LogLevelFilter { get => _logLevelFilter; set => _logLevelFilter = value; }

        public event SourceFilterConditionChangedHandler FilterConditionChanged
        {
            add
            {
                _filterConditionChanged += value;
            }
            remove
            {
                _filterConditionChanged -= value;
            }
        }

        public bool Filter(object obj)
        {
            return LogLevelFilter.Filter(obj)
                && LogTagFilter.Filter(obj)
                && LogMessageFilter.Filter(obj)
                && LogTagRemoveFilter.Filter(obj)
                && LogTidFilter.Filter(obj)
                && LogPidFilter.Filter(obj)
                && LogStartTimeFilter.Filter(obj)
                && LogEndTimeFilter.Filter(obj);
        }

        public void NotifyFilterPropertyChanged(ISourceFilter sender, object e)
        {
            if (NotifyFilterConditionChangedMessage != null
                && NotifyFilterConditionChangedMessage.IsAlive)
            {
                NotifyFilterConditionChangedMessage.Interrupt();
                NotifyFilterConditionChangedMessage.Abort();
            }

            NotifyFilterConditionChangedMessage = new Thread(() =>
            {
                _filterConditionChanged?.Invoke(sender, new ConditionChangedEventArgs());
            });
            NotifyFilterConditionChangedMessage.Start();
        }

        public static SourceFilterManagerImpl Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SourceFilterManagerImpl();
                }
                return _instance;
            }
        }

    }
}
