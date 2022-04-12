using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.LogGuard.Base;
using LogGuard_v0._1.Windows.MainWindow.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher
{
    public class LogWatcherItemViewModel : BaseViewModel, ILogWatcherElements
    {
        private LogInfo _logInfo;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _pidSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _tidSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _tagSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _mesSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _extraMesSource;
        public LogWatcherItemViewModel(LogInfo logInfo)
        {
            this._logInfo = logInfo;
        }

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
                return _logInfo[LogInfo.KEY_LINE];
            }
        }

        [Bindable(true)]
        public object Date
        {
            get
            {
                return _logInfo[LogInfo.KEY_DATE];
            }
        }

        [Bindable(true)]
        public object Time
        {
            get
            {
                return _logInfo[LogInfo.KEY_TIME];
            }
        }

        [Bindable(true)]
        public object Pid
        {
            get
            {
                return _logInfo[LogInfo.KEY_PID];
            }
        }

        [Bindable(true)]
        public object Tid
        {
            get
            {
                return _logInfo[LogInfo.KEY_TID];
            }
        }

        [Bindable(true)]
        public object Package
        {
            get
            {
                return _logInfo[LogInfo.KEY_PACKAGE];
            }
        }

        [Bindable(true)]
        public object Tag
        {
            get
            {
                return _logInfo[LogInfo.KEY_TAG].ToString();
            }
            set
            {
                _logInfo[LogInfo.KEY_TAG] = value;
            }
        }

        [Bindable(true)]
        public object Level
        {
            get
            {
                return _logInfo[LogInfo.KEY_LEVEL];
            }
        }

        [Bindable(true)]
        public object Message
        {
            get
            {
                return _logInfo[LogInfo.KEY_MESSAGE];
            }
        }

        [Bindable(true)]
        public object Color
        {
            get
            {
                return ((SolidColorBrush)(new BrushConverter().ConvertFrom(_logInfo[LogInfo.KEY_COLOR]))).Color;
            }
        }

        [Bindable(true)]
        public object ColorHex
        {
            get
            {
                return _logInfo[LogInfo.KEY_COLOR];
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

        string ILogWatcherElements.Level { get => Level.ToString(); set { } }

    }
}
