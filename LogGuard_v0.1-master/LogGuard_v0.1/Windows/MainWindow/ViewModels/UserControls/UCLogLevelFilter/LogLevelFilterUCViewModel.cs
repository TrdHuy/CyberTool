using LogGuard_v0._1.Base.AndroidLog;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.UserControls.UCLogLevelFilter
{
    public class LogLevelFilterUCViewModel : BaseViewModel, ISourceFilter
    {
        private bool _isFilterBusy = false;
        private bool _isVerboseEnable = true;
        private bool _isDebugEnable = true;
        private bool _isInfoEnable = true;
        private bool _isFatalEnable = true;
        private bool _isErrorEnable = true;
        private bool _isWarningEnable = true;

        private bool[] _logLevelEnabler;

        [Bindable(true)]
        public bool IsFilterBusy
        {
            get
            {
                return _isFilterBusy;
            }
            set
            {
                _isFilterBusy = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsVerboseEnable
        {
            get
            {
                return _isVerboseEnable;
            }
            set
            {
                _isVerboseEnable = value;
                _logLevelEnabler[LogInfo.LEVEL_VERBOSE_INDEX] = value;
                OnNotifyFilterPropertyChanged(value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsDebugEnable
        {
            get
            {
                return _isDebugEnable;
            }
            set
            {
                _isDebugEnable = value;
                _logLevelEnabler[LogInfo.LEVEL_DEBUG_INDEX] = value;
                OnNotifyFilterPropertyChanged(value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsInfoEnable
        {
            get
            {
                return _isInfoEnable;
            }
            set
            {
                _isInfoEnable = value;
                _logLevelEnabler[LogInfo.LEVEL_INFO_INDEX] = value;
                OnNotifyFilterPropertyChanged(value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsErrorEnable
        {
            get
            {
                return _isErrorEnable;
            }
            set
            {
                _isErrorEnable = value;
                _logLevelEnabler[LogInfo.LEVEL_ERROR_INDEX] = value;
                OnNotifyFilterPropertyChanged(value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsWarningEnable
        {
            get
            {
                return _isWarningEnable;
            }
            set
            {
                _isWarningEnable = value;
                _logLevelEnabler[LogInfo.LEVEL_WARNING_INDEX] = value;
                OnNotifyFilterPropertyChanged(value);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsFatalEnable
        {
            get
            {
                return _isFatalEnable;
            }
            set
            {
                _isFatalEnable = value;
                _logLevelEnabler[LogInfo.LEVEL_FATAL_INDEX] = value;
                OnNotifyFilterPropertyChanged(value);
                InvalidateOwn();
            }
        }

        public LogLevelFilterUCViewModel()
        {
         
        }

        public LogLevelFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            _logLevelEnabler = new bool[LogInfo.LOG_LEVEL_COUNT];

            for (int i = 0; i < LogInfo.LOG_LEVEL_COUNT; i++)
            {
                _logLevelEnabler[i] = true;
            }

            SourceFilterManagerImpl.Current.LogLevelFilter = this;

            SourceManagerImpl.Current.SourceFilteredAndDisplayed -= OnSourceFilteredAndDisplayed;
            SourceManagerImpl.Current.SourceFilteredAndDisplayed += OnSourceFilteredAndDisplayed;
        }

        public void Clean(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Filter(object obj)
        {
            var data = obj as LogWatcherItemViewModel;
            if (data != null)
            {
                switch (data.Level)
                {
                    case "V":
                        return _logLevelEnabler[LogInfo.LEVEL_VERBOSE_INDEX];
                    case "D":
                        return _logLevelEnabler[LogInfo.LEVEL_DEBUG_INDEX];
                    case "I":
                        return _logLevelEnabler[LogInfo.LEVEL_INFO_INDEX];
                    case "E":
                        return _logLevelEnabler[LogInfo.LEVEL_ERROR_INDEX];
                    case "W":
                        return _logLevelEnabler[LogInfo.LEVEL_WARNING_INDEX];
                    case "F":
                        return _logLevelEnabler[LogInfo.LEVEL_FATAL_INDEX];
                    default:
                        return true;
                }
            }
            return true;
        }

        public bool Highlight(object obj)
        {
            throw new NotImplementedException();
        }

        private void OnSourceFilteredAndDisplayed(object sender)
        {
            IsFilterBusy = false;
        }

        private void OnNotifyFilterPropertyChanged(bool changed)
        {
            IsFilterBusy = true;
            SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, changed);
        }

    }
}
