using cyber_base.implement.command;
using cyber_base.view_model;
using log_guard.@base.flow.source_filterr;
using log_guard.view_models.watcher;

namespace log_guard.view_models.advance_filter.tag_filter
{
    internal class TagRemoveFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        protected new bool _isFilterEnable = true;

        public TagRemoveFilterUCViewModel(BaseViewModel parent) : base(parent)
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
            var itemVM = obj as LWI_ParseableViewModel;
            if (itemVM != null)
            {
                return TagRemove(itemVM);
            }
            return true;
        }
        public override bool IsUseFilterEngine => true;

        private bool TagRemove(LWI_ParseableViewModel data)
        {
            if (!CurrentEngine.IsVaild())
            {
                CurrentEngine.Refresh();
                return true;
            }

            if (IsFilterEnable && data.Tag != null)
            {
                if (CurrentEngine.ContainIgnoreCase(data.Tag.ToString()))
                {
                    return false;
                }
                return true;
            }
            return true;
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
                        FilterConditionHelperContent = "Removes log lines which tag ignore lower/upper case containing: " + CurrentEngine.HelperContent;
                        break;
                    case FilterType.Syntax:
                        FilterConditionHelperContent = "Removes log lines which tag ignore lower/upper case containing:\n" + CurrentEngine.HelperContent;
                        break;
                }
            }
        }
    }
}
