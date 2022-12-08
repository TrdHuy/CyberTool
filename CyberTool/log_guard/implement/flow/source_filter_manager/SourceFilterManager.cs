using log_guard.@base.flow.source_filter;
using log_guard.@base.flow.source_filterr;
using log_guard.@base.module;
using log_guard.implement.module;
using System.Threading;

namespace log_guard.implement.flow.source_filter_manager
{
    internal class SourceFilterManager : BaseLogGuardModule, ISourceFilterManager
    {
        private event SourceFilterConditionChangedHandler? _filterConditionChanged;

        private IMechanicalSourceFilter? _logTagFilter;
        private IMechanicalSourceFilter? _logTagRemoveFilter;
        private IMechanicalSourceFilter? _logMessageFilter;
        private IMechanicalSourceFilter? _logPidFilter;
        private IMechanicalSourceFilter? _logTidFilter;
        private IMechanicalSourceFilter? _logStartTimeFilter;
        private IMechanicalSourceFilter? _logEndTimeFilter;
        private IMechanicalSourceFilter? _logMessageRemoveFilter;
        private ISourceFilter? _logLevelFilter;

        public IMechanicalSourceFilter? LogTagRemoveFilter { get => _logTagRemoveFilter; set => _logTagRemoveFilter = value; }
        public IMechanicalSourceFilter? LogTagFilter { get => _logTagFilter; set => _logTagFilter = value; }
        public IMechanicalSourceFilter? LogMessageFilter { get => _logMessageFilter; set => _logMessageFilter = value; }
        public IMechanicalSourceFilter? LogTidFilter { get => _logTidFilter; set => _logTidFilter = value; }
        public IMechanicalSourceFilter? LogPidFilter { get => _logPidFilter; set => _logPidFilter = value; }
        public IMechanicalSourceFilter? LogStartTimeFilter { get => _logStartTimeFilter; set => _logStartTimeFilter = value; }
        public IMechanicalSourceFilter? LogEndTimeFilter { get => _logEndTimeFilter; set => _logEndTimeFilter = value; }
        public IMechanicalSourceFilter? LogMessageRemoveFilter { get => _logMessageRemoveFilter; set => _logMessageRemoveFilter = value; }
        public ISourceFilter? LogLevelFilter { get => _logLevelFilter; set => _logLevelFilter = value; }

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
            return (LogTagRemoveFilter != null ? LogTagRemoveFilter.Filter(obj) : true)
                && (LogMessageRemoveFilter != null ? LogMessageRemoveFilter.Filter(obj) : true)
                && (LogLevelFilter != null ? LogLevelFilter.Filter(obj) : true)
                && (LogTagFilter != null ? LogTagFilter.Filter(obj) : true)
                && (LogMessageFilter != null ? LogMessageFilter.Filter(obj) : true)
                && (LogTidFilter != null ? LogTidFilter.Filter(obj) : true)
                && (LogPidFilter != null ? LogPidFilter.Filter(obj) : true)
                && (LogStartTimeFilter != null ? LogStartTimeFilter.Filter(obj) : true)
                && (LogEndTimeFilter != null ? LogEndTimeFilter.Filter(obj) : true);
        }

        public void NotifyFilterPropertyChanged(ISourceFilter sender, object e)
        {
            _filterConditionChanged?.Invoke(sender, new ConditionChangedEventArgs());
        }

        public static SourceFilterManager Current
        {
            get
            {
                return LogGuardModuleManager.SFM_Instance;
            }
        }

    }
}
