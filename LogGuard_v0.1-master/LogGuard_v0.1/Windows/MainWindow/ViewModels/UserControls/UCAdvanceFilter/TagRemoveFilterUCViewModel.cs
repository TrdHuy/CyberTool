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

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.UserControls.UCAdvanceFilter
{
    public class TagRemoveFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        private string _tagRemoveFilterContent = "";
        private string _tagRemoveHelperContent = "";
        private bool _isTagRemoveEnable = true;

        [Bindable(true)]
        public CommandExecuterModel TagRemoveRightClickCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterModel TagRemoveLeftClickCommand { get; set; }

        [Bindable(true)]
        public string TagRemoveHelperContent
        {
            get
            {
                return _tagRemoveHelperContent;
            }
            set
            {
                _tagRemoveHelperContent = value;
                InvalidateOwn();
            }
        }
        [Bindable(true)]
        public string TagRemoveFilterContent
        {
            get
            {
                return _tagRemoveFilterContent;
            }
            set
            {
                _tagRemoveFilterContent = value;
                SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsTagRemoveEnable
        {
            get
            {
                return _isTagRemoveEnable;
            }
            set
            {
                _isTagRemoveEnable = value;
                SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public FilterType TagRemoveFilterLevel
        {
            get
            {
                return CurrentFilterMode;
            }
            set
            {
                CurrentFilterMode = value;
                InvalidateOwn();
            }
        }

        public TagRemoveFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            TagRemoveLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsTagRemoveEnable = !IsTagRemoveEnable;
                return null;
            });


            TagRemoveRightClickCommand = new CommandExecuterModel((paramaters) =>
            {
                switch (CurrentFilterMode)
                {
                    case FilterType.Simple:
                        TagRemoveFilterLevel = FilterType.Syntax;
                        break;
                    case FilterType.Syntax:
                        TagRemoveFilterLevel = FilterType.Advance;
                        break;
                    case FilterType.Advance:
                        TagRemoveFilterLevel = FilterType.Simple;
                        break;
                }
                UpdateTagRemoveHelperContent();
                return null;
            });
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
        private void UpdateTagRemoveHelperContent()
        {
            TagRemoveHelperContent = "Left click to enable filter\n" +
                "Right click to change filter mode\n" +
                "Filter mode: " + TagRemoveFilterLevel.ToString(); ;
        }

        private bool TagRemove(LogWatcherItemViewModel data)
        {
            if (_isTagRemoveEnable)
            {
                if (!string.IsNullOrEmpty(TagRemoveFilterContent)
                    && data.Tag.ToString().IndexOf(TagRemoveFilterContent, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}
