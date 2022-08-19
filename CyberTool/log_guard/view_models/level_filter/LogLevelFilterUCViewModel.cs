using cyber_base.view_model;
using log_guard.@base.flow.source_filter;
using log_guard.implement.flow.source_filter_manager;
using log_guard.implement.flow.source_manager;
using log_guard.models.info;
using log_guard.view_models.watcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.level_filter
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

            SourceFilterManager.Current.LogLevelFilter = this;

            SourceManager.Current.SourceFilteredAndDisplayed -= OnSourceFilteredAndDisplayed;
            SourceManager.Current.SourceFilteredAndDisplayed += OnSourceFilteredAndDisplayed;
        }

        public void Clean(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Filter(object obj)
        {
            var data = obj as LWI_ParseableViewModel;
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
            SourceFilterManager.Current.NotifyFilterPropertyChanged(this, changed);
        }

    }
}
