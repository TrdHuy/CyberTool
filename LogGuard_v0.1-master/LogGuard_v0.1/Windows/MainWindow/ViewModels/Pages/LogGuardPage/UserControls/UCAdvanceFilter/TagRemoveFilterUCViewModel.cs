using LogGuard_v0._1.AppResources.AttachedProperties;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter
{
    public class TagRemoveFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        protected new bool _isFilterEnable = true;

        public TagRemoveFilterUCViewModel(BaseViewModel parent) : base(parent)
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
