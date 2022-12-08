using cyber_base.view_model;
using log_guard.@base.flow.source_filterr;
using log_guard.view_models.watcher;
using cyber_base.implement.command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.advance_filter.message_filter
{
    internal class MessageShowFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        public override bool IsUseFilterEngine { get => true; }

        public MessageShowFilterUCViewModel(BaseViewModel parent) : base(parent)
        {

            FilterLeftClickCommand = new CommandExecuterModel((paramaters) =>
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
            var data = obj as LWI_ParseableViewModel;
            if (!CurrentEngine.IsVaild())
            {
                CurrentEngine.Refresh();
                return true;
            }

            if (IsFilterEnable && data.Message != null)
            {
                if (CurrentEngine.ContainIgnoreCase(data.Message.ToString()))
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        protected override bool DoHighlight(object obj)
        {
            var data = obj as LWI_ParseableViewModel;
            if (data != null)
            {
                data.HighlightMessageSource = CurrentEngine
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
                data.HighlightMessageSource = null;
            }
        }

        protected override void UpdateFilterConditionHelperContent()
        {
            if (string.IsNullOrEmpty(CurrentEngine.HelperContent))
            {
                FilterConditionHelperContent = "Type a few words for helpful hints!";
            }
            else
            {
                switch (CurrentFilterMode)
                {
                    case FilterType.Simple:
                        FilterConditionHelperContent = "Show log lines which message ignore lower/upper case containing: " + CurrentEngine.HelperContent;
                        break;
                    case FilterType.Syntax:
                        FilterConditionHelperContent = "Show log lines which message ignore lower/upper case containing:\n" + CurrentEngine.HelperContent;
                        break;
                }
            }
        }
    }
}

