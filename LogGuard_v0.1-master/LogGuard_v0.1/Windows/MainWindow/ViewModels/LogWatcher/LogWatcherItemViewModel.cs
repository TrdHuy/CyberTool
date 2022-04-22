using LogGuard_v0._1.Base.AndroidLog;
using LogGuard_v0._1.Base.Command;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.LogGuard.Base;
using LogGuard_v0._1.Windows.MainWindow.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher
{
    public class LogWatcherItemViewModel : BaseViewModel, ILogWatcherElements
    {
        private LogInfo _logInfo;
        private ElementViewType _viewType;
        private ICommand _expandButtonCommand;
        private ICommand _deleteButtonCommand;
        private int _lineNumber = -1;
        private List<ILogWatcherElements> _childs;

        private IEnumerable<Base.LogGuardFlow.MatchedWord> _pidSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _tidSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _tagSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _mesSource;
        private IEnumerable<Base.LogGuardFlow.MatchedWord> _extraMesSource;
        public LogWatcherItemViewModel()
        {
            _viewType = ElementViewType.ExpandableRowView;
            _childs = new List<ILogWatcherElements>();
        }

        public LogWatcherItemViewModel(LogInfo logInfo)
        {
            _viewType = ElementViewType.LogView;
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
                return _logInfo?[LogInfo.KEY_TAG].ToString();
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

        public ElementViewType ViewType { get => _viewType; set => _viewType = value; }
        public ICommand ExpandButtonCommand { get => _expandButtonCommand; set => _expandButtonCommand = value; }
        public ICommand DeleteButtonCommand { get => _deleteButtonCommand; set => _deleteButtonCommand = value; }

        /// <summary>
        /// Thuộc tính quan trong nhất trong tính năng delete log
        /// Thuộc tính này khác với thuộc tính Line
        /// Line là vị trí của dòng log trong file raw text hoặc từ process capture log
        /// Thuộc tính này chỉ ra vị trí hiển thị của dòng log hiện tại 
        /// đang ở vị trí nào trong LogWatcher
        /// 
        /// Chỉ cập nhật lại thuộc tính này khi dòng log đươc đưa vào lại display source
        /// (source này dưới sự quản lý của SourceManagerImpl)
        /// </summary>
        public int LineNumber { get => _lineNumber; set => _lineNumber = value; }

        public List<ILogWatcherElements> Childs { get => _childs; set => _childs = value; }

        string ILogWatcherElements.Level { get => Level?.ToString(); set { } }

    }
}
