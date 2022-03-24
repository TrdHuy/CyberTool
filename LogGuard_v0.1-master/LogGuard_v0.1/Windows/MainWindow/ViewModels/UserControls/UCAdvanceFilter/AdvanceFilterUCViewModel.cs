using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.UserControls.UCAdvanceFilter
{
    public class AdvanceFilterUCViewModel : BaseViewModel
    {
        private string _detailContent = "line(s)";
        private string _extraContent = "Total: line(s)";
        private string _currentLogLevel = "Info log";
        private string _currentLogLevelShorcutS = "I";
        private double _logCount;
        private double _logValuePercent;
        private bool _isInfoChecked;
        private bool _isErrorChecked;
        private bool _isDebugChecked;
        private bool _isVerboseChecked;
        private bool _isWarningChecked;
        private bool _isFatalChecked;

        private TagFilterUCViewModel _tagFilterVM;
        private MessageFilterUCViewModel _messageFilterVM;
        private PidTidFilterUCViewModel _pidTidFilterVM;

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
                }
            }
        }

        [Bindable(true)]
        public string DetailContent
        {
            get
            {
                return _detailContent;
            }
            set
            {
                _detailContent = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string ExtraContent
        {
            get
            {
                return _extraContent;
            }
            set
            {
                _extraContent = value;
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
        public TagFilterUCViewModel TagFilterContent
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
        public PidTidFilterUCViewModel PidTidFilterContent
        {
            get
            {
                return _pidTidFilterVM;
            }
            set
            {
                _pidTidFilterVM = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public MessageFilterUCViewModel MessageFilterContent
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
            TagFilterContent = new TagFilterUCViewModel(this);
            MessageFilterContent = new MessageFilterUCViewModel(this);
            PidTidFilterContent = new PidTidFilterUCViewModel(this);

            SourceFilterManagerImpl.Current.LogTagFilter = TagFilterContent;
            SourceFilterManagerImpl.Current.LogMessageFilter = MessageFilterContent;
            SourceFilterManagerImpl.Current.LogPidTidFilter = PidTidFilterContent;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LogCount = 0;
            LogValue = 0;
            DetailContent = _currentLogLevelShorcutS + ": " + 0 + " line(s)";
            ExtraContent = "Total: " + LogCount + " line(s)";
            SourceManagerImpl.Current.SourceCollectionChanged -= OnLogSourceCollectionChanged;
        }

        private void OnLogSourceCollectionChanged(object sender)
        {
            UpdateChartcInfo();
        }

        #region Log measure tool method
        private void UpdateChartcInfo()
        {
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
            DetailContent = _currentLogLevelShorcutS + ": " + per + " line(s)";
            ExtraContent = "Total: " + LogCount + " line(s)";
        }
        private void UpdateCurrentShowProcess(string level, bool show)
        {
            if (show)
            {
                switch (level)
                {
                    case "V":
                        _currentLogLevel = "Verbose log";
                        _currentLogLevelShorcutS = "V";
                        _isVerboseChecked = true;
                        _isDebugChecked = false;
                        _isErrorChecked = false;
                        _isInfoChecked = false;
                        _isWarningChecked = false;
                        _isFatalChecked = false;
                        break;
                    case "D":
                        _currentLogLevel = "Debug log";
                        _currentLogLevelShorcutS = "D";
                        _isDebugChecked = true;
                        _isVerboseChecked = false;
                        _isErrorChecked = false;
                        _isInfoChecked = false;
                        _isWarningChecked = false;
                        _isFatalChecked = false;
                        break;
                    case "E":
                        _currentLogLevel = "Error log";
                        _currentLogLevelShorcutS = "E";
                        _isDebugChecked = false;
                        _isVerboseChecked = false;
                        _isErrorChecked = true;
                        _isInfoChecked = false;
                        _isWarningChecked = false;
                        _isFatalChecked = false;
                        break;
                    case "I":
                        _currentLogLevel = "Info log";
                        _currentLogLevelShorcutS = "I";
                        _isDebugChecked = false;
                        _isVerboseChecked = false;
                        _isErrorChecked = false;
                        _isInfoChecked = true;
                        _isWarningChecked = false;
                        _isFatalChecked = false;
                        break;
                    case "W":
                        _currentLogLevel = "Warning log";
                        _currentLogLevelShorcutS = "W";
                        _isDebugChecked = false;
                        _isVerboseChecked = false;
                        _isErrorChecked = false;
                        _isInfoChecked = false;
                        _isWarningChecked = true;
                        _isFatalChecked = false;
                        break;
                    case "F":
                        _currentLogLevel = "Fatal log";
                        _currentLogLevelShorcutS = "F";
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
