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
    public class PidTidFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        private string _pidFilterContent = "";
        private string _tidFilterContent = "";
        private bool _isPidFilterEnable = false;
        private bool _isTidFilterEnable = false;




        [Bindable(true)]
        public CommandExecuterModel PidFilterLeftClickCommand { get; set; }
        [Bindable(true)]
        public CommandExecuterModel TidFilterLeftClickCommand { get; set; }

        [Bindable(true)]
        public bool IsPidFilterEnable
        {
            get
            {
                return _isPidFilterEnable;
            }
            set
            {
                _isPidFilterEnable = value;
                InvalidateOwn();
            }
        }
        [Bindable(true)]
        public bool IsTidFilterEnable
        {
            get
            {
                return _isTidFilterEnable;
            }
            set
            {
                _isTidFilterEnable = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string PidFilterContent
        {
            get
            {
                return _pidFilterContent;
            }
            set
            {
                _pidFilterContent = value;
                SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string TidFilterContent
        {
            get
            {
                return _tidFilterContent;
            }
            set
            {
                _tidFilterContent = value;
                SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, value);
                InvalidateOwn();
            }
        }

        public PidTidFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            PidFilterLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsPidFilterEnable = !IsPidFilterEnable;
                return null;
            });

            TidFilterLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsTidFilterEnable = !IsTidFilterEnable;
                return null;
            });
        }

        public override bool Filter(object obj)
        {
            var itemVM = obj as LogWatcherItemViewModel;
            if (itemVM != null)
            {
                return itemVM
                    .Tid
                    .ToString()
                    .IndexOf(TidFilterContent, StringComparison.InvariantCultureIgnoreCase) != -1
                    && itemVM
                    .Pid
                    .ToString()
                    .IndexOf(PidFilterContent, StringComparison.InvariantCultureIgnoreCase) != -1;
            }
            return true;
        }
    }
}
