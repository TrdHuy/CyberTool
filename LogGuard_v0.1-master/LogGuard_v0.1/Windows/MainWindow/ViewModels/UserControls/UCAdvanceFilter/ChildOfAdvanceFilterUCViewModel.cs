using LogGuard_v0._1._Config;
using LogGuard_v0._1.AppResources.AttachedProperties;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.FilterEngines;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using LogGuard_v0._1.Implement.UIEventHandler;
using System.ComponentModel;
using System.Threading;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.UserControls.UCAdvanceFilter
{
    public abstract class ChildOfAdvanceFilterUCViewModel : BaseViewModel, ISourceFilter
    {
        private bool _isFilterBusy = false;
        private SearchBehavior _searchType = SearchBehavior.None;
        protected IFilterEngine CurrentEngine { get; set; }

        [Bindable(true)]
        public CommandExecuterModel FilterLeftClickCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterModel FilterRightClickCommand { get; set; }

        [Bindable(true)]
        public SearchBehavior Search
        {
            get
            {
                return _searchType;
            }
            set
            {
                _searchType = value;
                InvalidateOwn();
            }
        }

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
        public FilterType CurrentFilterMode
        {
            get
            {
                return _currentFilterMode;
            }
            set
            {
                _currentFilterMode = value;
                UpdateEngine();
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsFilterEnable
        {
            get
            {
                return _isFilterEnable;
            }
            set
            {
                var oldVal = _isFilterEnable;
                _isFilterEnable = value;

                if (oldVal != value)
                {
                    OnFilterEnableChanged(value);
                    UpdateHelperContent();
                    InvalidateOwn();
                }
            }
        }

        [Bindable(true)]
        public string EngineHelperContent
        {
            get
            {
                return _engineHelperContent;
            }
            set
            {
                _engineHelperContent = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string FilterHelperContent
        {
            get
            {
                return _filterHelperContent;
            }
            set
            {
                _filterHelperContent = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string FilterContent
        {
            get
            {
                return _filterContent;
            }
            set
            {
                _filterContent = value;
                OnFilterContentChanged(value);
                InvalidateOwn();
            }
        }

        /// <summary>
        /// Lọc các giá trị theo điều kiện, trả về kiểu bool
        /// </summary>
        /// <param name="obj">đối tượng cần kiểm tra điều kiện để lọc</param>
        /// <returns></returns>
        public abstract bool Filter(object obj);

        /// <summary>
        /// Dùng để kiểm tra bộ lọc hiện tại có sử dụng engine để lọc không
        /// </summary>
        protected abstract bool IsUseFilterEngine { get; }

        private string _engineHelperContent = "";
        private string _filterHelperContent = "";
        private string _filterContent = "";
        protected bool _isFilterEnable = false;
        private FilterType _currentFilterMode = FilterType.Simple;
        private NormalFilterEngine _normalEngine = new NormalFilterEngine();
        private SyntaxFilterEngine _syntaxEngine = new SyntaxFilterEngine();
        private Thread _notifyFilterEngineChangedMessage;

        public ChildOfAdvanceFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            if (RUNE.IS_SUPPORT_QUICK_FILTER_TEXT_BOX)
            {
                Search = SearchBehavior.QuickSearch;
            }
            else
            {
                Search = SearchBehavior.NormalSearch;
            }

            FilterRightClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsFilterEnable = !IsFilterEnable;
                return null;
            });

            UpdateEngine();

            SourceManagerImpl.Current.SourceFilteredAndDisplayed -= OnSourceFilteredAndDisplayed;
            SourceManagerImpl.Current.SourceFilteredAndDisplayed += OnSourceFilteredAndDisplayed;
        }


        /// <summary>
        /// Highlight các giá trị theo điều kiện, trả về kiểu bool
        /// </summary>
        /// <param name="obj">đối tượng cần highlight</param>
        /// <returns>true: nếu có chuỗi đã được highlight</returns>
        /// <returns>false: nếu không có chuỗi nào được highlight</returns>
        public bool Highlight(object obj)
        {
            return DoHighlight(obj);
        }

        /// <summary>
        /// Bỏ toàn bộ các đối tượng đang highlight trong 1 chuỗi
        /// </summary>
        /// <param name="obj">đối tượng cần bỏ highlight</param>
        public void Clean(object obj)
        {
            DoCleanHighlightSource(obj);
        }

        protected void UpdateEngine()
        {
            switch (CurrentFilterMode)
            {
                case FilterType.Simple:
                    CurrentEngine = _normalEngine;
                    break;
                case FilterType.Advance:
                case FilterType.Syntax:
                    CurrentEngine = _syntaxEngine;
                    break;
            }
            UpdateHelperContent();
        }

        protected void UpdateEngingeComparableSource(string source)
        {
            if (!IsUseFilterEngine)
            {
                return;
            }

            if (_notifyFilterEngineChangedMessage != null
                && _notifyFilterEngineChangedMessage.IsAlive)
            {
                _notifyFilterEngineChangedMessage.Interrupt();
                _notifyFilterEngineChangedMessage.Abort();
            }

            _notifyFilterEngineChangedMessage = new Thread(() =>
            {
                CurrentEngine.UpdateComparableSource(source);
            });
            _notifyFilterEngineChangedMessage.Start();
        }

        protected void UpdateHelperContent()
        {
            var turnHelper = IsFilterEnable ? "disable" : "enable";
            if (IsUseFilterEngine)
            {
                EngineHelperContent = "Left click to change filter mode" +
                "\nFilter mode: " + CurrentFilterMode.ToString();
            }
            FilterHelperContent = "Left click to " + turnHelper + " filter";

        }

        protected void NotifyFilterContentChanged(object conditionChanged)
        {
            switch (conditionChanged)
            {
                case string changed:
                    if (IsFilterEnable)
                    {
                        IsFilterBusy = true;
                        SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, changed);
                    }
                    break;
                case bool changed:
                    if (FilterContent != "")
                    {
                        IsFilterBusy = true;
                        SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, changed);
                    }
                    break;
            }

        }

        protected virtual void OnFilterContentChanged(string value)
        {
            UpdateEngingeComparableSource(value);
            NotifyFilterContentChanged(value);
        }

        protected virtual void OnFilterEnableChanged(bool value)
        {
            NotifyFilterContentChanged(value);
        }

        protected virtual bool DoHighlight(object obj)
        {
            return false;
        }

        protected virtual void DoCleanHighlightSource(object obj)
        {

        }


        private void OnSourceFilteredAndDisplayed(object sender)
        {
            IsFilterBusy = false;
        }

    }
}
