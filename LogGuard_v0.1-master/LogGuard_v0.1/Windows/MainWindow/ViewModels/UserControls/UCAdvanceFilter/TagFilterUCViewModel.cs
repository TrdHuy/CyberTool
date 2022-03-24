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
    public class TagFilterUCViewModel : BaseViewModel, ISourceFilter
    {
        private string _tagShowFilterContent = "";
        private string _tagShowHelperContent = "";
        private bool _isTagShowEnable = true;
        private FilterType _tagShowFilterLevel = FilterType.Simple;
        private string _tagRemoveFilterContent = "";
        private string _tagRemoveHelperContent = "";
        private bool _isTagRemoveEnable = true;
        private FilterType _tagRemoveFilterLevel = FilterType.Simple;

        [Bindable(true)]
        public CommandExecuterModel TagShowRightClickCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterModel TagShowLeftClickCommand { get; set; }

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
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public FilterType TagShowFilterLevel
        {
            get
            {
                return _tagShowFilterLevel;
            }
            set
            {
                _tagShowFilterLevel = value;
                InvalidateOwn();
            }
        }


        [Bindable(true)]
        public CommandExecuterModel TagRemoveRightClickCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterModel TagRemoveLeftClickCommand { get; set; }

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
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public FilterType TagRemoveFilterLevel
        {
            get
            {
                return _tagRemoveFilterLevel;
            }
            set
            {
                _tagRemoveFilterLevel = value;
                InvalidateOwn();
            }
        }

        public TagFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            TagShowLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsTagShowEnable = !IsTagShowEnable;
                return null;
            });

            TagShowRightClickCommand = new CommandExecuterModel((paramaters) =>
            {
                switch (_tagShowFilterLevel)
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

            TagRemoveLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsTagRemoveEnable = !IsTagRemoveEnable;


                return null;
            });


            TagRemoveRightClickCommand = new CommandExecuterModel((paramaters) =>
            {
                switch (_tagRemoveFilterLevel)
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
            UpdateTagShowHelperContent();
            UpdateTagRemoveHelperContent();
        }

        public bool Filter(object obj)
        {
            var itemVM = obj as LogWatcherItemViewModel;
            if (itemVM != null)
            {
                return itemVM
                    .Tag
                    .ToString()
                    .IndexOf(TagShowFilterContent, StringComparison.InvariantCultureIgnoreCase) != -1;
            }
            return true;
        }

        private void UpdateTagShowHelperContent()
        {
            TagShowHelperContent = "Left click to enable filter\n" +
                "Right click to change filter mode\n" +
                "Filter mode: " + TagShowFilterLevel.ToString(); ;
        }

        private void UpdateTagRemoveHelperContent()
        {
            TagRemoveHelperContent = "Left click to enable filter\n" +
                "Right click to change filter mode\n" +
                "Filter mode: " + TagRemoveFilterLevel.ToString(); ;
        }

    }
}
