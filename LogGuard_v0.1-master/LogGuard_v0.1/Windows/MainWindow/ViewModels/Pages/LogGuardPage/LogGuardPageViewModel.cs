using LogGuard_v0._1.Base.Device;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.Device;
using LogGuard_v0._1.Implement.LogGuardFlow.RunThreadConfig;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using LogGuard_v0._1.Implement.LogGuardFlow.StateController;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Device;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages
{
    public class LogGuardPageViewModel : MSW_BasePageViewModel, ISourceHolder
    {

        private RangeObservableCollection<LogWatcherItemViewModel> _logItemVMs;
        private int _logCount;
        private int _selectedCmdIndex;
        private LogGuardState _currentLogGuardState = LogGuardState.NONE;
        private bool _useAutoScroll = true;
        private ObservableCollection<LogParserVO> _deviceCmdItemsSource = new ObservableCollection<LogParserVO>();
        private DeviceItemViewModel _selectedDevice;

        [Bindable(true)]
        public DeviceItemViewModel SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;
                DeviceManagerImpl.Current.SelectedDevice = value;
                InvalidateOwn();
            }
        }

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
                RunThreadConfigManager.Current.CurrentParser = _deviceCmdItemsSource[value];
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
        public MSW_LogWatcherControlGestureCommandVM GestureViewModel { get; set; }

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
            Init();
        }

        public LogGuardPageViewModel(BaseViewModel parentVM) : base(parentVM)
        {
            Init();
        }

        private void Init()
        {
            CommandViewModel = new MSW_LogWatcherControlButtonCommandVM(this);
            GestureViewModel = new MSW_LogWatcherControlGestureCommandVM(this);

            SourceManagerImpl.Current.AddSourceHolder(this);
            RunThreadConfigManager.Current.Init();
            InitDeviceCmdItemsList();
        }

        private void InitDeviceCmdItemsList()
        {
            int index = 0;
            for (int i = 0; i < DeviceCmdContact.CMD_CONTACT_USER_INTERFACE_LIST.Count; i++)
            {
                var item = DeviceCmdContact.CMD_CONTACT_USER_INTERFACE_LIST[i];
                DeviceCmdItemsSource.Add(item);
                if (RunThreadConfigManager.Current.CurrentParser != null
                    && RunThreadConfigManager.Current.CurrentParser.Cmd == item.Cmd)
                {
                    index = i;
                }
            }
            SelectedCmdIndex = index;
        }

        public override bool OnUnloaded()
        {
            // Stop before clear
            StateControllerImpl.Current.Stop();
            SourceManagerImpl.Current.ClearSource();
            SourceManagerImpl.Current.RemoveSourceHolder(this);

            return base.OnUnloaded();
        }

        public override void OnLoaded()
        {
            base.OnLoaded();

            StateControllerImpl.Current.StateChanged -= OnLogGuardStateChanged;
            StateControllerImpl.Current.StateChanged += OnLogGuardStateChanged;
        }

        private void OnLogGuardStateChanged(object sender, StateChangedEventArgs e)
        {
            CurrentLogGuardState = e.NewState;
        }
    }
}
