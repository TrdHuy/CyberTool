﻿using LogGuard_v0._1.AppResources.AttachedProperties;
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
    public class TagRemoveFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {

        [Bindable(true)]
        public CommandExecuterModel TagRemoveRightClickCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterModel TagRemoveLeftClickCommand { get; set; }

        public TagRemoveFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            TagRemoveLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsFilterEnable = !IsFilterEnable;
                return null;
            });


            TagRemoveRightClickCommand = new CommandExecuterModel((paramaters) =>
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
                return TagRemove(itemVM);
            }
            return true;
        }
        protected override bool IsUseFilterEngine => true;

        private bool TagRemove(LogWatcherItemViewModel data)
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
    }
}
