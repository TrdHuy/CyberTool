﻿using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceHighlightManager;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter.TimeFilter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter
{
    public class AdvanceFilterUCViewModel : BaseViewModel
    {
        private string _logLevelCount = "0 line(s)";
        private string _totalLogCount = "0 line(s)";
        private string _currentLogLevel = "Info log";
        private double _logCount;
        private string _displayingLogCount = "0 line(s)";
        private double _logValuePercent;
        private bool _isInfoChecked;
        private bool _isErrorChecked;
        private bool _isDebugChecked;
        private bool _isVerboseChecked;
        private bool _isWarningChecked;
        private bool _isFatalChecked;

        private TagShowFilterUCViewModel _tagFilterVM;
        private TagRemoveFilterUCViewModel _tagRemoveVM;
        private MessageShowFilterUCViewModel _messageFilterVM;
        private MessageRemoveFilterUCViewModel _messageRemoveFilterVM;
        private LogFinderUCViewModel _finderVM;
        private TidFilterUCViewModel _tidFilterVM;
        private PidFilterUCViewModel _pidFilterVM;
        private StartTimeFilterUCViewModel _startTimeFilterVM;
        private EndTimeFilterUCViewModel _endTimeFilterVM;

        #region Log measure tool binding area
        [Bindable(true)]
        public string CurrentLogLevel
        {
            get
            {
                return _currentLogLevel;
            }
            set
            {
                _currentLogLevel = value;
                InvalidateOwn();
            }

        }

        [Bindable(true)]
        public double LogCount
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



        [Bindable(true)]
        public double LogValue
        {
            get
            {
                return _logValuePercent;
            }
            set
            {
                var oldValue = _logValuePercent;
                _logValuePercent = value;
                if (oldValue != _logValuePercent)
                {
                    InvalidateOwn();
                    Invalidate("RadialProgressValue");
                }
            }
        }

        [Bindable(true)]
        public string RadialProgressValue
        {
            get
            {
                return _logValuePercent + "%";
            }
        }

        [Bindable(true)]
        public string LogLevelCount
        {
            get
            {
                return _logLevelCount;
            }
            set
            {
                _logLevelCount = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string TotalLogCount
        {
            get
            {
                return _totalLogCount;
            }
            set
            {
                _totalLogCount = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string DisplayingLogCount
        {
            get
            {
                return _displayingLogCount;
            }
            set
            {
                _displayingLogCount = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsInfoChecked
        {
            get
            {
                return _isInfoChecked;
            }
            set
            {
                _isInfoChecked = value;
                UpdateCurrentShowProcess("I", value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsErrorChecked
        {
            get
            {
                return _isErrorChecked;
            }
            set
            {
                _isErrorChecked = value;
                UpdateCurrentShowProcess("E", value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsDebugChecked
        {
            get
            {
                return _isDebugChecked;
            }
            set
            {
                _isDebugChecked = value;
                UpdateCurrentShowProcess("D", value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsVerboseChecked
        {
            get
            {
                return _isVerboseChecked;
            }
            set
            {
                _isVerboseChecked = value;
                UpdateCurrentShowProcess("V", value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsWarningChecked
        {
            get
            {
                return _isWarningChecked;
            }
            set
            {
                _isWarningChecked = value;
                UpdateCurrentShowProcess("W", value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsFatalChecked
        {
            get
            {
                return _isFatalChecked;
            }
            set
            {
                _isFatalChecked = value;
                UpdateCurrentShowProcess("F", value);
                InvalidateOwn();
            }
        }

        #endregion

        #region Filter tool binding area
        [Bindable(true)]
        public TagRemoveFilterUCViewModel TagRemoveContent
        {
            get
            {
                return _tagRemoveVM;
            }
            set
            {
                _tagRemoveVM = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public TagShowFilterUCViewModel TagFilterContent
        {
            get
            {
                return _tagFilterVM;
            }
            set
            {
                _tagFilterVM = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public TidFilterUCViewModel TidFilterContent
        {
            get
            {
                return _tidFilterVM;
            }
            set
            {
                _tidFilterVM = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public PidFilterUCViewModel PidFilterContent
        {
            get
            {
                return _pidFilterVM;
            }
            set
            {
                _pidFilterVM = value;
                InvalidateOwn();
            }
        }


        [Bindable(true)]
        public MessageShowFilterUCViewModel MessageFilterContent
        {
            get
            {
                return _messageFilterVM;
            }
            set
            {
                _messageFilterVM = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public MessageRemoveFilterUCViewModel MessageRemoveFilterContent
        {
            get
            {
                return _messageRemoveFilterVM;
            }
            set
            {
                _messageRemoveFilterVM = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public LogFinderUCViewModel FinderContent
        {
            get
            {
                return _finderVM;
            }
            set
            {
                _finderVM = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public StartTimeFilterUCViewModel StartTimeFilterContent
        {
            get
            {
                return _startTimeFilterVM;
            }
            set
            {
                _startTimeFilterVM = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public EndTimeFilterUCViewModel EndTimeFilterContent
        {
            get
            {
                return _endTimeFilterVM;
            }
            set
            {
                _endTimeFilterVM = value;
                InvalidateOwn();
            }
        }

        #endregion
        public AdvanceFilterUCViewModel()
        {
            Init();
        }

        public AdvanceFilterUCViewModel(BaseViewModel baseViewModel) : base(baseViewModel)
        {
            Init();
        }

        private void Init()
        {
            IsInfoChecked = true;
            SourceManagerImpl.Current.SourceCollectionChanged -= OnLogSourceCollectionChanged;
            SourceManagerImpl.Current.SourceCollectionChanged += OnLogSourceCollectionChanged;
            TagFilterContent = new TagShowFilterUCViewModel(this);
            MessageFilterContent = new MessageShowFilterUCViewModel(this);
            TidFilterContent = new TidFilterUCViewModel(this);
            PidFilterContent = new PidFilterUCViewModel(this);
            TagRemoveContent = new TagRemoveFilterUCViewModel(this);
            FinderContent = new LogFinderUCViewModel(this);
            StartTimeFilterContent = new StartTimeFilterUCViewModel(this);
            EndTimeFilterContent = new EndTimeFilterUCViewModel(this);
            MessageRemoveFilterContent = new MessageRemoveFilterUCViewModel(this);

            SourceFilterManagerImpl.Current.LogTagRemoveFilter = TagRemoveContent;
            SourceFilterManagerImpl.Current.LogTagFilter = TagFilterContent;
            SourceFilterManagerImpl.Current.LogMessageFilter = MessageFilterContent;
            SourceFilterManagerImpl.Current.LogMessageRemoveFilter = MessageRemoveFilterContent;
            SourceFilterManagerImpl.Current.LogTidFilter = TidFilterContent;
            SourceFilterManagerImpl.Current.LogPidFilter = PidFilterContent;
            SourceFilterManagerImpl.Current.LogStartTimeFilter = StartTimeFilterContent;
            SourceFilterManagerImpl.Current.LogEndTimeFilter = EndTimeFilterContent;

            SourceHighlightManagerImpl.Current.TagFilterHighlightor = TagFilterContent;
            SourceHighlightManagerImpl.Current.FinderHighlightor = FinderContent;
            SourceHighlightManagerImpl.Current.MessageFilterHighlightor = MessageFilterContent;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LogCount = 0;
            LogValue = 0;
            LogLevelCount = "0 line(s)";
            TotalLogCount = "0 line(s)";
            DisplayingLogCount = "0 line(s)";
            SourceManagerImpl.Current.SourceCollectionChanged -= OnLogSourceCollectionChanged;
        }

        private void OnLogSourceCollectionChanged(object sender)
        {
            UpdateChartcInfo();
        }

        #region Log measure tool method
        private void UpdateChartcInfo()
        {
            var displayingCount = (double)SourceManagerImpl.Current.DisplayItemsCount();
            LogCount = (double)SourceManagerImpl.Current.RawItemsCount();
            double per = 0d;

            if (SourceManagerImpl.Current.RawItemsCount() > 0)
            {
                if (IsVerboseChecked)
                {
                    per = (double)SourceManagerImpl.Current.VerboseItemsCount();
                }
                else if (IsInfoChecked)
                {
                    per = (double)SourceManagerImpl.Current.InfoItemsCount();
                }
                else if (IsDebugChecked)
                {
                    per = (double)SourceManagerImpl.Current.DebugItemsCount();
                }
                else if (IsErrorChecked)
                {
                    per = (double)SourceManagerImpl.Current.ErrorItemsCount();
                }
                else if (IsWarningChecked)
                {
                    per = (double)SourceManagerImpl.Current.WarningItemsCount();
                }
                else if (IsFatalChecked)
                {
                    per = (double)SourceManagerImpl.Current.FatalItemsCount();
                }

                LogValue = Math.Round(per / LogCount * 100, 2);
            }
            LogLevelCount = per + " line(s)";
            TotalLogCount = LogCount + " line(s)";
            DisplayingLogCount = displayingCount + " line(s)";
        }
        private void UpdateCurrentShowProcess(string level, bool show)
        {
            if (show)
            {
                switch (level)
                {
                    case "V":
                        _currentLogLevel = "Verbose log";
                        _isVerboseChecked = true;
                        _isDebugChecked = false;
                        _isErrorChecked = false;
                        _isInfoChecked = false;
                        _isWarningChecked = false;
                        _isFatalChecked = false;
                        break;
                    case "D":
                        _currentLogLevel = "Debug log";
                        _isDebugChecked = true;
                        _isVerboseChecked = false;
                        _isErrorChecked = false;
                        _isInfoChecked = false;
                        _isWarningChecked = false;
                        _isFatalChecked = false;
                        break;
                    case "E":
                        _currentLogLevel = "Error log";
                        _isDebugChecked = false;
                        _isVerboseChecked = false;
                        _isErrorChecked = true;
                        _isInfoChecked = false;
                        _isWarningChecked = false;
                        _isFatalChecked = false;
                        break;
                    case "I":
                        _currentLogLevel = "Info log";
                        _isDebugChecked = false;
                        _isVerboseChecked = false;
                        _isErrorChecked = false;
                        _isInfoChecked = true;
                        _isWarningChecked = false;
                        _isFatalChecked = false;
                        break;
                    case "W":
                        _currentLogLevel = "Warning log";
                        _isDebugChecked = false;
                        _isVerboseChecked = false;
                        _isErrorChecked = false;
                        _isInfoChecked = false;
                        _isWarningChecked = true;
                        _isFatalChecked = false;
                        break;
                    case "F":
                        _currentLogLevel = "Fatal log";
                        _isDebugChecked = false;
                        _isVerboseChecked = false;
                        _isErrorChecked = false;
                        _isInfoChecked = false;
                        _isWarningChecked = false;
                        _isFatalChecked = true;
                        break;
                }
                RefreshViewModel();
                UpdateChartcInfo();
            }
            else
            {
                if (_isVerboseChecked == false
                && _isDebugChecked == false
                && _isErrorChecked == false
                && _isInfoChecked == false
                && _isWarningChecked == false
                && _isFatalChecked == false)
                {
                    _currentLogLevel = "Info log";
                    _isInfoChecked = true;
                }
                RefreshViewModel();
                UpdateChartcInfo();
            }
        }

        #endregion
    }
}
