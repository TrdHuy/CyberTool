using LogGuard_v0._1.AppResources.AttachedProperties;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceHighlightManager;
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
    public class MessageHighlightUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        [Bindable(true)]
        public CommandExecuterModel MessageHighlightRightClickCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterModel MessageHighlightLeftClickCommand { get; set; }

        protected override bool IsUseFilterEngine { get => true; }

        public MessageHighlightUCViewModel(BaseViewModel parent) : base(parent)
        {
            MessageHighlightLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsFilterEnable = !IsFilterEnable;
                return null;
            });

            MessageHighlightRightClickCommand = new CommandExecuterModel((paramaters) =>
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
            if (!CurrentEngine.IsVaild())
            {
                CurrentEngine.Refresh();
            }
            return true;
        }

        protected override bool DoHighlight(object obj)
        {
            var data = obj as LogWatcherItemViewModel;

            if (data != null)
            {
                if (!IsFilterEnable || FilterContent == "")
                {
                    data.ExtraHighlightMessageSource = null;
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

                return !CurrentEngine.IsMatchLstEmpty;
            }
            return false;
        }

        protected override void DoCleanHighlightSource(object obj)
        {
            var data = obj as LogWatcherItemViewModel;
            if (data != null)
            {
                data.ExtraHighlightMessageSource = null;
            }
        }

        protected override void OnFilterContentChanged(string value)
        {
            UpdateEngingeComparableSource(value);
            if (IsFilterEnable)
                SourceHighlightManagerImpl.Current.NotifyHighlightPropertyChanged(this, value);
        }

        protected override void OnFilterEnableChanged(bool value)
        {
            SourceHighlightManagerImpl.Current.NotifyHighlightPropertyChanged(this, value);
        }
    }
}

