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

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.UserControls.UCAdvanceFilter
{
    public class TagShowFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {

        [Bindable(true)]
        public CommandExecuterModel TagShowRightClickCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterModel TagShowLeftClickCommand { get; set; }


        public TagShowFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            TagShowLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsFilterEnable = !IsFilterEnable;
                return null;
            });

            TagShowRightClickCommand = new CommandExecuterModel((paramaters) =>
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

            _isFilterEnable = true;
            UpdateHelperContent();
            UpdateEngingeComparableSource(FilterContent);
        }

        public override bool Filter(object obj)
        {
            var itemVM = obj as LogWatcherItemViewModel;
            if (itemVM != null)
            {
                return TagShow(itemVM);
            }
            return true;
        }

        protected override bool IsUseFilterEngine => true;

        private bool TagShow(LogWatcherItemViewModel data)
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
            var data = obj as LogWatcherItemViewModel;
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
            var data = obj as LogWatcherItemViewModel;
            if (data != null)
            {
                data.HighlightTagSource = null;
            }
        }
    }
}
