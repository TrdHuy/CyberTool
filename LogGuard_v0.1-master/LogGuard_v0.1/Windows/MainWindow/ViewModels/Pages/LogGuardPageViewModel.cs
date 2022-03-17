using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.Device;
using LogGuard_v0._1.Implement.LogGuardFlow.RunThreadConfig;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages
{
    public class LogGuardPageViewModel : BaseViewModel, ISourceHolder
    {

        private RangeObservableCollection<LogWatcherItemViewModel> _logItemVMs;
        private int _logCount;
        private int _selectedCmdIndex;
        private LogGuardState _currentLogGuardState = LogGuardState.NONE;
        private bool _useAutoScroll = false;
        private ObservableCollection<LogParserVO> _deviceCmdItemsSource = new ObservableCollection<LogParserVO>();

        [Bindable(true)]
        public int SelectedCmdIndex
        {
            get
            {
                return _selectedCmdIndex;
            }
            set
            {
                _selectedCmdIndex = value;
                RunThreadConfigImpl.Current.LogParserFormat = _deviceCmdItemsSource[value];
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public ObservableCollection<LogParserVO> DeviceCmdItemsSource
        {
            get
            {
                return _deviceCmdItemsSource;
            }
            set
            {
                _deviceCmdItemsSource = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool UseAutoScroll
        {
            get
            {
                return _useAutoScroll;
            }
            set
            {
                _useAutoScroll = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public MSW_LogWatcherControlButtonCommandVM CommandViewModel { get; set; }

        [Bindable(true)]
        public RangeObservableCollection<LogWatcherItemViewModel> LogItemVMs
        {
            get
            {
                return _logItemVMs;
            }
            set
            {
                _logItemVMs = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public LogGuardState CurrentLogGuardState
        {
            get
            {
                return _currentLogGuardState;
            }
            set
            {
                _currentLogGuardState = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public RangeObservableCollection<LogWatcherItemViewModel> ItemsSource
        {
            get
            {
                return _logItemVMs;
            }
            set
            {
                _logItemVMs = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public int ItemCount
        {
            get
            {
                return _logCount;
            }
            set
            {
                _logCount = value;
                InvalidateOwn();
            }
        }

        public LogGuardPageViewModel()
        {
            CommandViewModel = new MSW_LogWatcherControlButtonCommandVM(this);
            LogItemVMs = new RangeObservableCollection<LogWatcherItemViewModel>();
            SourceManagerImpl.Current.AddSourceHolder(this);
            InitDeviceCmdItemsList();
        }

        public LogGuardPageViewModel(BaseViewModel parentVM) : base(parentVM)
        {
            CommandViewModel = new MSW_LogWatcherControlButtonCommandVM(this);
            LogItemVMs = new RangeObservableCollection<LogWatcherItemViewModel>();
            SourceManagerImpl.Current.AddSourceHolder(this);
            InitDeviceCmdItemsList();
        }

        private void InitDeviceCmdItemsList()
        {
            foreach (var item in DeviceCmdContact.CMD_CONTACT_USER_INTERFACE_LIST)
            {
                DeviceCmdItemsSource.Add(item);
            }
            SelectedCmdIndex = 0;
        }
        ~LogGuardPageViewModel()
        {

        }
    }
}
