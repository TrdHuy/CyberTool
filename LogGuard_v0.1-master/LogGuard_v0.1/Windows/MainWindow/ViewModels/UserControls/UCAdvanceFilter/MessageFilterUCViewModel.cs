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
        private string _messageFilterContent = "";
        private bool _isMessageFilterEnable = false;
        private FilterType _messageFilterLevel = FilterType.Simple;

        [Bindable(true)]
        public CommandExecuterModel MessageFilterRightClickCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterModel MessageFilterLeftClickCommand { get; set; }

        [Bindable(true)]
        public string MessageFilterContent
        {
            get
            {
                return _messageFilterContent;
            }
            set
            {
                _messageFilterContent = value;
                SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsMessageFilterEnable
        {
            get
            {
                return _isMessageFilterEnable;
            }
            set
            {
                _isMessageFilterEnable = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public FilterType MessageFilterLevel
        {
            get
            {
                return _messageFilterLevel;
            }
            set
            {
                _messageFilterLevel = value;
                InvalidateOwn();
            }
        }



        public MessageFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            MessageFilterLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsMessageFilterEnable = !IsMessageFilterEnable;
                return null;
            });

            MessageFilterRightClickCommand = new CommandExecuterModel((paramaters) =>
            {
                switch (_messageFilterLevel)
                {
                    case FilterType.Simple:
                        MessageFilterLevel = FilterType.Syntax;
                        break;
                    case FilterType.Syntax:
                        MessageFilterLevel = FilterType.Advance;
                        break;
                    case FilterType.Advance:
                        MessageFilterLevel = FilterType.Simple;
                        break;
                }
                return null;
            });
        }

        public override bool Filter(object obj)
        {
            var itemVM = obj as LogWatcherItemViewModel;
            if (itemVM != null)
            {
                return itemVM
                    .Message
                    .ToString()
                    .IndexOf(MessageFilterContent, StringComparison.InvariantCultureIgnoreCase) != -1;
            }
            return true;
        }
    }
}

