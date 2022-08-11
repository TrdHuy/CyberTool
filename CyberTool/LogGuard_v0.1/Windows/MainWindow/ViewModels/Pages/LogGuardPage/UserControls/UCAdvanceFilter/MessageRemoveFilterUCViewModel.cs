using LogGuard_v0._1.AppResources.AttachedProperties;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter
{
    public class MessageRemoveFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        public override bool IsUseFilterEngine { get => true; }

        public MessageRemoveFilterUCViewModel(BaseViewModel parent) : base(parent)
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
                        FilterConditionHelperContent = "Removes log lines which message ignore lower/upper case containing: " + CurrentEngine.HelperContent;
                        break;
                    case FilterType.Syntax:
                        FilterConditionHelperContent = "Removes log lines which message ignore lower/upper case containing:\n" + CurrentEngine.HelperContent;
                        break;
                }
            }
        }
    }
}

