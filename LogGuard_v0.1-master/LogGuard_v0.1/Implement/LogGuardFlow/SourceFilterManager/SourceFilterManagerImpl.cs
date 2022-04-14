using LogGuard_v0._1.AppResources.AttachedProperties;
using LogGuard_v0._1.Base.LogGuardFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager
{
    public class SourceFilterManagerImpl : ISourceFilterManager
    {
        private static SourceFilterManagerImpl _instance;

        private Thread NotifyFilterConditionChangedMessage { get; set; }

        private ISourceFilter _logTagFilter;
        private ISourceFilter _logTagRemoveFilter;
        private ISourceFilter _logMessageFilter;
        private ISourceFilter _logPidFilter;
        private ISourceFilter _logTidFilter;
        private ISourceFilter _logStartTimeFilter;
        private ISourceFilter _logEndTimeFilter;

        public ISourceFilter LogTagRemoveFilter { get => _logTagRemoveFilter; set => _logTagRemoveFilter = value; }
        public ISourceFilter LogTagFilter { get => _logTagFilter; set => _logTagFilter = value; }
        public ISourceFilter LogMessageFilter { get => _logMessageFilter; set => _logMessageFilter = value; }
        public ISourceFilter LogTidFilter { get => _logTidFilter; set => _logTidFilter = value; }
        public ISourceFilter LogPidFilter { get => _logPidFilter; set => _logPidFilter = value; }
        public ISourceFilter LogStartTimeFilter { get => _logStartTimeFilter; set => _logStartTimeFilter = value; }
        public ISourceFilter LogEndTimeFilter { get => _logEndTimeFilter; set => _logEndTimeFilter = value; }


        public event SourceFilterConditionChangedHandler FilterConditionChanged;

        public bool Filter(object obj)
        {
            return _logTagFilter.Filter(obj)
                && _logMessageFilter.Filter(obj)
                && _logTagRemoveFilter.Filter(obj)
                && _logTidFilter.Filter(obj)
                && _logPidFilter.Filter(obj)
                && _logStartTimeFilter.Filter(obj)
                && _logEndTimeFilter.Filter(obj);
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
                FilterConditionChanged?.Invoke(sender, new ConditionChangedEventArgs());
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
