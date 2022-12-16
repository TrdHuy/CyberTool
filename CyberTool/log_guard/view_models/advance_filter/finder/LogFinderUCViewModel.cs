using cyber_base.implement.command;
using cyber_base.view_model;
using log_guard.@base.flow.source_filterr;
using log_guard.implement.flow.source_highlight_manager;
using log_guard.view_models.watcher;
using System.Linq;

namespace log_guard.view_models.advance_filter.finder
{
    internal class LogFinderUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        public override bool IsUseFilterEngine { get => true; }

        public LogFinderUCViewModel(BaseViewModel parent) : base(parent)
        {
            FilterLeftClickCommand = new CommandExecuterImpl((paramaters) =>
            {
                switch (CurrentFilterMode)
                {
                    case FilterType.Simple:
                        CurrentFilterMode = FilterType.Syntax;
                        break;
                    case FilterType.Syntax:
                        CurrentFilterMode = FilterType.Advance;
                        break;
                    case FilterType.Advance:
                        CurrentFilterMode = FilterType.Simple;
                        break;
                }
                return null;
            });
        }

        public override bool Filter(object obj)
        {
            if (!CurrentEngine.IsVaild())
            {
                CurrentEngine.Refresh();
            }
            return true;
        }

        protected override bool DoHighlight(object obj)
        {
            var data = obj as LWI_ParseableViewModel;

            if (data != null)
            {
                if (FilterContent == "")
                {
                    data.ExtraHighlightMessageSource = null;
                    data.ExtraHighlightTagSource = null;
                    return false;
                }

                if (data.Message.Equals(""))
                {
                    return false;
                }

                CurrentEngine.ContainIgnoreCase(data.Message.ToString());

                data.ExtraHighlightMessageSource = CurrentEngine
                           .GetMatchWords()
                           .OrderBy(o => o.StartIndex)
                           .ToArray();

                CurrentEngine.ContainIgnoreCase(data.Tag.ToString());

                data.ExtraHighlightTagSource = CurrentEngine
                           .GetMatchWords()
                           .OrderBy(o => o.StartIndex)
                           .ToArray();

                return !CurrentEngine.IsMatchLstEmpty;
            }
            return false;
        }

        protected override void DoCleanHighlightSource(object obj)
        {
            var data = obj as LWI_ParseableViewModel;
            if (data != null)
            {
                data.ExtraHighlightMessageSource = null;
                data.ExtraHighlightTagSource = null;
            }
        }

        protected override void OnComparableSourceUpdated(object sender, object args)
        {
            SourceHighlightManager.Current.NotifyHighlightPropertyChanged(this, args);
        }

    }
}

