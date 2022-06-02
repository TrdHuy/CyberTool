using log_guard.@base.flow.highlight;
using log_guard.@base.flow.source_filter;
using log_guard.@base.module;
using log_guard.implement.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace log_guard.implement.flow.source_highlight_manager
{
    internal class SourceHighlightManager : BaseLogGuardModule, ISourceHighlightManager
    {
        private ISourceHighlightor? _logMessHighlightor;
        private ISourceHighlightor? _logMessFilterHighlightor;
        private ISourceHighlightor? _logTagFilterHighlightor;

        public event SourceHighlightConditionChangedHandler? HighlightConditionChanged;
        public ISourceHighlightor? FinderHighlightor { get => _logMessHighlightor; set => _logMessHighlightor = value; }
        public ISourceHighlightor? MessageFilterHighlightor { get => _logMessFilterHighlightor; set => _logMessFilterHighlightor = value; }
        public ISourceHighlightor? TagFilterHighlightor { get => _logTagFilterHighlightor; set => _logTagFilterHighlightor = value; }

        public void NotifyHighlightPropertyChanged(ISourceHighlightor sender, object e)
        {
            HighlightConditionChanged?.Invoke(sender, new ConditionChangedEventArgs());
        }
        public void FilterHighlight(object obj)
        {
            MessageFilterHighlightor?.Highlight(obj);
            TagFilterHighlightor?.Highlight(obj);
        }

        public void FinderHighlight(object obj)
        {
            FinderHighlightor?.Highlight(obj);
        }

        public void Clean(object obj)
        {
            FinderHighlightor?.Clean(obj);
            TagFilterHighlightor?.Clean(obj);
            MessageFilterHighlightor?.Clean(obj);
        }


        public static SourceHighlightManager Current
        {
            get
            {
                return LogGuardModuleManager.SHM_Instance;
            }
        }

    }
}
