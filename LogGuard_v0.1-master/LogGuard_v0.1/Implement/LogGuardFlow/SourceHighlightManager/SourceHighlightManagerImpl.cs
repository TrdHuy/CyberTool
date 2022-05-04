using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.LogGuardFlow.SourceFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.LogGuardFlow.SourceHighlightManager
{
    public class SourceHighlightManagerImpl : ISourceHighlightManager
    {
        private static SourceHighlightManagerImpl _instance;
        private ISourceHighlightor _logMessHighlightor;
        private ISourceHighlightor _logMessFilterHighlightor;
        private ISourceHighlightor _logTagFilterHighlightor;
        private Thread NotifyHighlightConditionChangedMessage { get; set; }

        public event SourceHighlightConditionChangedHandler HighlightConditionChanged;
        public ISourceHighlightor MessageHighlightor { get => _logMessHighlightor; set => _logMessHighlightor = value; }
        public ISourceHighlightor MessageFilterHighlightor { get => _logMessFilterHighlightor; set => _logMessFilterHighlightor = value; }
        public ISourceHighlightor TagFilterHighlightor { get => _logTagFilterHighlightor; set => _logTagFilterHighlightor = value; }

        public void NotifyHighlightPropertyChanged(ISourceHighlightor sender, object e)
        {
            if (NotifyHighlightConditionChangedMessage != null
                && NotifyHighlightConditionChangedMessage.IsAlive)
            {
                NotifyHighlightConditionChangedMessage.Interrupt();
                NotifyHighlightConditionChangedMessage.Abort();
            }

            NotifyHighlightConditionChangedMessage = new Thread(() =>
            {
                HighlightConditionChanged?.Invoke(sender, new ConditionChangedEventArgs());
            });
            NotifyHighlightConditionChangedMessage.Start();
        }

        public void FilterHighlight(object obj)
        {
            MessageFilterHighlightor.Highlight(obj);
            TagFilterHighlightor?.Highlight(obj);

        }

        public void Highlight(object obj)
        {
            MessageHighlightor.Highlight(obj);
        }

        public void Clean(object obj)
        {
            MessageHighlightor?.Clean(obj);
            TagFilterHighlightor?.Clean(obj);
            MessageFilterHighlightor?.Clean(obj);
        }

        

        public static SourceHighlightManagerImpl Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SourceHighlightManagerImpl();
                }
                return _instance;
            }
        }

    }
}
