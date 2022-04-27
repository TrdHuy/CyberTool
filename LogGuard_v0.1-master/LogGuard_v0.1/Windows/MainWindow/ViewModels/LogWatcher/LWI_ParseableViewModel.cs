using LogGuard_v0._1.Base.AndroidLog;
using LogGuard_v0._1.Base.Command;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher
{
    public class LWI_ParseableViewModel : LogWatcherItemViewModel
    {
        private ICommand _tagLeftDoubleClickCommand;
        private LogInfo _logInfo;

        private IEnumerable<Base.LogGuardFlow.MatchedWord> _pidSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _tidSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _tagSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _mesSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _extraMesSource;

        public ICommand TagLeftDoubleClickCommand { get => _tagLeftDoubleClickCommand; set => _tagLeftDoubleClickCommand = value; }

        [Bindable(true)]
        public object HighlightTidSource
        {
            get
            {
                return _tidSource;
            }
            set
            {
                _tidSource = (IEnumerable<Base.LogGuardFlow.MatchedWord>)value;
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
                _pidSource = (IEnumerable<Base.LogGuardFlow.MatchedWord>)value;
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
                _mesSource = (IEnumerable<Base.LogGuardFlow.MatchedWord>)value;
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
                _extraMesSource = (IEnumerable<Base.LogGuardFlow.MatchedWord>)value;
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
                _tagSource = (IEnumerable<Base.LogGuardFlow.MatchedWord>)value;
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
            ViewType = LogGuard.Base.ElementViewType.LogView;

            var lgVM = parent as LogGuardPageViewModel;
            if(parent != null)
            {
                _tagLeftDoubleClickCommand = lgVM.GestureViewModel.LogTagDoubleClickCommand;
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
