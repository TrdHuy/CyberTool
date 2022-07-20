using cyber_base.implement.utils;
using cyber_base.view_model;
using log_guard.@base.flow;
using log_guard.@base.watcher;
using log_guard.definitions;
using log_guard.implement.device;
using log_guard.implement.flow.run_thread_config;
using log_guard.implement.flow.source_manager;
using log_guard.implement.flow.state_controller;
using log_guard.models.vo;
using log_guard.view_models.command;
using log_guard.view_models.device;
using log_guard.view_models.parser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models
{
    internal class LogGuardViewModel : BaseViewModel, ISourceHolder
    {
        private RangeObservableCollection<ILogWatcherElements> _logItemVMs;
        private int _logCount;
        private string _parserBoxTip;
        private int _selectedCmdIndex;
        private LogGuardState _currentLogGuardState = LogGuardState.NONE;
        private bool _useAutoScroll = true;
        private Dictionary<LogParserOption, int> _parserOptionIndexMap = new Dictionary<LogParserOption, int>();
        private ObservableCollection<LogParserItemViewModel> _deviceCmdItemsSource = new ObservableCollection<LogParserItemViewModel>();
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
                DeviceManager.Current.SelectedDevice = value;
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
                if (_deviceCmdItemsSource[value].ParserVO != null)
                {
                    RunThreadConfigManager.Current.CurrentParser = _deviceCmdItemsSource[value].ParserVO;
                }
                ParserBoxTip = _deviceCmdItemsSource[value].ParserTip;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string ParserBoxTip
        {
            get
            {
                return _parserBoxTip;
            }
            set
            {
                _parserBoxTip = value;
                InvalidateOwn();
            }
        }


        [Bindable(true)]
        public ObservableCollection<LogParserItemViewModel> DeviceCmdItemsSource
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
        public LG_ButtonCommandVM CommandViewModel { get; set; }

        [Bindable(true)]
        public LG_GestureCommandVM GestureViewModel { get; set; }

        [Bindable(true)]
        public LogGuardState CurrentLogGuardState
        {
            get
            {
                return _currentLogGuardState;
            }
            private set
            {
                _currentLogGuardState = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public RangeObservableCollection<ILogWatcherElements> ItemsSource
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

        public LogGuardViewModel()
        {
            Init();
        }

        public LogGuardViewModel(BaseViewModel parentVM) : base(parentVM)
        {
            Init();
        }

        private void Init()
        {
            CommandViewModel = new LG_ButtonCommandVM(this);
            GestureViewModel = new LG_GestureCommandVM(this);

            SourceManager.Current.AddSourceHolder(this);
            InitDeviceCmdItemsList();
        }

        private void InitDeviceCmdItemsList()
        {
            int index = 0;

            foreach (KeyValuePair<LogParserOption, LogParserVO> entry in LogParserDefinition.LOG_PARSER_FORMATS_LIST)
            {
                var item = entry.Value;
                var itemVM = new LogParserItemViewModel(item);
                _parserOptionIndexMap.Add(entry.Key, index);
                DeviceCmdItemsSource.Add(itemVM);

                if (RunThreadConfigManager.Current.CurrentParser != null
                    && RunThreadConfigManager.Current.CurrentParser.Cmd == item.Cmd)
                {
                    SelectedCmdIndex = index;
                }

                if (item.FormatContact == LogParserFormatContact.OPEN_DUMPSTATE_FILE)
                {
                    itemVM.OnComboBoxItemSelected = GestureViewModel.ParserFormatSelectedCommand;
                }

                index++;
            }
        }

        public void SelectParserOption(LogParserOption opt)
        {
            if (_parserOptionIndexMap.ContainsKey(opt))
            {
                SelectedCmdIndex = _parserOptionIndexMap[opt];
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            // Stop before clear
            StateController.Current?.Stop();
            SourceManager.Current.ClearSource();
            SourceManager.Current.RemoveSourceHolder(this);
        }

        public override void OnBegin()
        {
            base.OnBegin();
            StateController.Current.StateChanged -= OnLogGuardStateChanged;
            StateController.Current.StateChanged += OnLogGuardStateChanged;
        }

        private void OnLogGuardStateChanged(object sender, StateChangedEventArgs e)
        {
            CurrentLogGuardState = e.NewState;
        }
    }
}
