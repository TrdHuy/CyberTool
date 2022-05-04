using LogGuard_v0._1.AppResources.AttachedProperties;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter
{
    public class TagShowFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        protected new bool _isFilterEnable = true;

        public TagShowFilterUCViewModel(BaseViewModel parent) : base(parent)
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
                return TagShow(itemVM);
            }
            return true;
        }

        public override bool IsUseFilterEngine => true;

        private bool TagShow(LWI_ParseableViewModel data)
        {
            data.HighlightTagSource = null;
            if (!CurrentEngine.IsVaild())
            {
                CurrentEngine.Refresh();
                return true;
            }

            if (IsFilterEnable && data.Tag != null)
            {
                return CurrentEngine.ContainIgnoreCase(data.Tag.ToString());
            }
            return true;
        }

        protected override bool DoHighlight(object obj)
        {
            var data = obj as LWI_ParseableViewModel;
            if (data != null)
            {
                data.HighlightTagSource = CurrentEngine
                            .GetMatchWords()
                            .OrderBy(o => o.StartIndex)
                            .ToArray();
            }
            return true;
        }

        protected override void DoCleanHighlightSource(object obj)
        {
            var data = obj as LWI_ParseableViewModel;
            if (data != null)
            {
                data.HighlightTagSource = null;
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
                        FilterConditionHelperContent = "Shows log lines which tag ignore lower/upper case containing: " + CurrentEngine.HelperContent;
                        break;
                    case FilterType.Syntax:
                        FilterConditionHelperContent = "Shows log lines which tag ignore lower/upper case containing:\n" + CurrentEngine.HelperContent;
                        break;
                }
            }

            
        }

    }
}
