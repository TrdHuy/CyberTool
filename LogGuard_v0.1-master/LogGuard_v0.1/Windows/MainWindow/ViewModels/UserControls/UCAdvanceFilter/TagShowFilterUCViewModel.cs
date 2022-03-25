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
        private string _tagShowFilterContent = "";
        private string _tagShowHelperContent = "";
        private bool _isTagShowEnable = true;
        

        [Bindable(true)]
        public CommandExecuterModel TagShowRightClickCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterModel TagShowLeftClickCommand { get; set; }

        [Bindable(true)]
        public string TagShowHelperContent
        {
            get
            {
                return _tagShowHelperContent;
            }
            set
            {
                _tagShowHelperContent = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string TagShowFilterContent
        {
            get
            {
                return _tagShowFilterContent;
            }
            set
            {
                _tagShowFilterContent = value;
                SourceFilterManagerImpl.Current.NotifyFilterEngineSourceChanged(this, value);
                SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsTagShowEnable
        {
            get
            {
                return _isTagShowEnable;
            }
            set
            {
                _isTagShowEnable = value;
                SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public FilterType TagShowFilterLevel
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


        
        public TagShowFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            TagShowLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsTagShowEnable = !IsTagShowEnable;
                return null;
            });

            TagShowRightClickCommand = new CommandExecuterModel((paramaters) =>
            {
                switch (CurrentFilterMode)
                {
                    case FilterType.Simple:
                        TagShowFilterLevel = FilterType.Syntax;
                        break;
                    case FilterType.Syntax:
                        TagShowFilterLevel = FilterType.Advance;
                        break;
                    case FilterType.Advance:
                        TagShowFilterLevel = FilterType.Simple;
                        break;
                }
                UpdateTagShowHelperContent();
                return null;
            });

            
            UpdateTagShowHelperContent();
            UpdateEngingeComparableSource(TagShowFilterContent);
        }

        public override bool Filter(object obj)
        {
            var itemVM = obj as LogWatcherItemViewModel;
            if (itemVM != null)
            {
                return TagShow(itemVM) ;
            }
            return true;
        }


        private void UpdateTagShowHelperContent()
        {
            TagShowHelperContent = "Left click to enable filter\n" +
                "Right click to change filter mode\n" +
                "Filter mode: " + TagShowFilterLevel.ToString(); ;
        }

        private bool TagShow(LogWatcherItemViewModel data)
        {
            if (_isTagShowEnable)
            {
                if (CurrentEngine.ContainIgnoreCase(data.Tag.ToString()))
                {
                    return true;
                }
                return false;
            }
            return true;
        }

      

    }
}
