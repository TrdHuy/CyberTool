using cyber_base.view_model;
using log_guard.@base.watcher;
using log_guard.definitions;
using log_guard.models.info;
using log_guard.models.vo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace log_guard.view_models.watcher
{
    public class LWI_ParseableViewModel : LogWatcherItemViewModel
    {
        private ICommand _tagLeftDoubleClickCommand;
        private ICommand _mesLeftDoubleClickCommand;
        private LogInfo _logInfo;

        private IEnumerable<MatchedWordVO> _pidSource;
        private IEnumerable<MatchedWordVO> _tidSource;
        private IEnumerable<MatchedWordVO> _tagSource;
        private IEnumerable<MatchedWordVO> _extraTagSource;
        private IEnumerable<MatchedWordVO> _mesSource;
        private IEnumerable<MatchedWordVO> _extraMesSource;

        public ICommand TagLeftDoubleClickCommand { get => _tagLeftDoubleClickCommand; set => _tagLeftDoubleClickCommand = value; }
        public ICommand MessageLeftDoubleClickCommand { get => _mesLeftDoubleClickCommand; set => _mesLeftDoubleClickCommand = value; }

        [Bindable(true)]
        public object HighlightTidSource
        {
            get
            {
                return _tidSource;
            }
            set
            {
                _tidSource = (IEnumerable<MatchedWordVO>)value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object HighlightPidSource
        {
            get
            {
                return _pidSource;
            }
            set
            {
                _pidSource = (IEnumerable<MatchedWordVO>)value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object HighlightMessageSource
        {
            get
            {
                return _mesSource;
            }
            set
            {
                _mesSource = (IEnumerable<MatchedWordVO>)value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object ExtraHighlightMessageSource
        {
            get
            {
                return _extraMesSource;
            }
            set
            {
                _extraMesSource = (IEnumerable<MatchedWordVO>)value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object HighlightTagSource
        {
            get
            {
                return _tagSource;
            }
            set
            {
                _tagSource = (IEnumerable<MatchedWordVO>)value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object ExtraHighlightTagSource
        {
            get
            {
                return _extraTagSource;
            }
            set
            {
                _extraTagSource = (IEnumerable<MatchedWordVO>)value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object Line
        {
            get
            {
                return _logInfo?[LogInfo.KEY_LINE];
            }
        }

        [Bindable(true)]
        public object Date
        {
            get
            {
                return _logInfo?[LogInfo.KEY_DATE];
            }
        }

        [Bindable(true)]
        public object Time
        {
            get
            {
                return _logInfo?[LogInfo.KEY_TIME];
            }
        }

        [Bindable(true)]
        public object Pid
        {
            get
            {
                return _logInfo?[LogInfo.KEY_PID];
            }
        }

        [Bindable(true)]
        public object Tid
        {
            get
            {
                return _logInfo?[LogInfo.KEY_TID];
            }
        }

        [Bindable(true)]
        public object Package
        {
            get
            {
                return _logInfo?[LogInfo.KEY_PACKAGE];
            }
        }

        [Bindable(true)]
        public object Tag
        {
            get
            {
                return _logInfo?[LogInfo.KEY_TAG];
            }
        }

        [Bindable(true)]
        public object Level
        {
            get
            {
                return _logInfo?[LogInfo.KEY_LEVEL];
            }
        }

        [Bindable(true)]
        public object Message
        {
            get
            {
                return _logInfo?[LogInfo.KEY_MESSAGE];
            }
        }

        [Bindable(true)]
        public string LogTagString
        {
            get
            {
                return _logInfo[LogInfo.KEY_LINE] + " " + _logInfo[LogInfo.KEY_TIME] + "  " + _logInfo[LogInfo.KEY_TAG] + " " + _logInfo[LogInfo.KEY_MESSAGE];
            }
        }

        [Bindable(true)]
        public object LogDateTimeString
        {
            get
            {
                return _logInfo[LogInfo.KEY_DATE_TIME_S];
            }
        }

        public DateTime LogDateTime
        {
            get
            {
                return (DateTime)_logInfo[LogInfo.KEY_DATE_TIME];
            }
        }

        /// <summary>
        /// These 2 property was used to handle binding exception
        /// Issue detail:
        /// https://mobilerndhub.sec.samsung.net/wiki/display/~huy.td1/%5BI-260401%5D+Binding+issue+after+delete+line+and+scroll+to+other+position+on+log+watcher
        /// </summary>
        public ICommand ExpandButtonCommand
        {
            get
            {
                return null;
            }
        }
        public ICommand DeleteButtonCommand
        {
            get
            {
                return null;
            }
        }


        public LWI_ParseableViewModel(BaseViewModel parent, LogInfo logInfo)
        {
            _logInfo = logInfo;
            ViewType = ElementViewType.LogView;

            var lgVM = parent as LogGuardViewModel;

            if(parent != null)
            {
                _tagLeftDoubleClickCommand = lgVM.GestureViewModel.LogTagDoubleClickCommand;
                _mesLeftDoubleClickCommand = lgVM.GestureViewModel.LogMessageDoubleClickCommand;

            }

            switch (logInfo[LogInfo.KEY_LEVEL])
            {
                case "V":
                    TrackColor = LogInfo.COLOR_VERBOSE;
                    ErrorColor = LogInfo.COLOR_DEFAULT;
                    break;
                case "D":
                    TrackColor = LogInfo.COLOR_DEBUG;
                    ErrorColor = LogInfo.COLOR_DEFAULT;
                    break;
                case "E":
                    TrackColor = LogInfo.COLOR_ERROR;
                    ErrorColor = LogInfo.COLOR_ERROR;
                    break;
                case "F":
                    TrackColor = LogInfo.COLOR_FATAL;
                    ErrorColor = LogInfo.COLOR_DEFAULT;
                    break;
                case "W":
                    TrackColor = LogInfo.COLOR_WARN;
                    ErrorColor = LogInfo.COLOR_DEFAULT;
                    break;
                case "I":
                    TrackColor = LogInfo.COLOR_INFO;
                    ErrorColor = LogInfo.COLOR_DEFAULT;
                    break;
                default:
                    TrackColor = LogInfo.COLOR_DEFAULT;
                    ErrorColor = LogInfo.COLOR_DEFAULT;
                    break;
            }
        }

       
    }
}
