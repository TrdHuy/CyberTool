using LogGuard_v0._1.AppResources.AttachedProperties;
using LogGuard_v0._1.Base.LogGuardFlow;
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

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.UserControls.UCAdvanceFilter
{
    public class MessageFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        [Bindable(true)]
        public CommandExecuterModel MessageFilterRightClickCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterModel MessageFilterLeftClickCommand { get; set; }

        protected override bool IsUseFilterEngine { get => true; }

        public MessageFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            MessageFilterLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsFilterEnable = !IsFilterEnable;
                return null;
            });

            MessageFilterRightClickCommand = new CommandExecuterModel((paramaters) =>
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

            UpdateHelperContent();
            UpdateEngingeComparableSource(FilterContent);
        }

        public override bool Filter(object obj)
        {
            var data = obj as LogWatcherItemViewModel;
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
    }
}

