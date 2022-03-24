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
        private ISourceFilter _logMessageFilter;
        private ISourceFilter _logPidTidFilter;

        public ISourceFilter LogTagFilter { get => _logTagFilter; set => _logTagFilter = value; }
        public ISourceFilter LogMessageFilter { get => _logMessageFilter; set => _logMessageFilter = value; }
        public ISourceFilter LogPidTidFilter { get => _logPidTidFilter; set => _logPidTidFilter = value; }


        public event SourceFilterConditionChangedHandler FilterConditionChanged;

        public bool Filter(object obj)
        {
            return _logTagFilter.Filter(obj)
                && _logMessageFilter.Filter(obj)
                && _logPidTidFilter.Filter(obj);
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
