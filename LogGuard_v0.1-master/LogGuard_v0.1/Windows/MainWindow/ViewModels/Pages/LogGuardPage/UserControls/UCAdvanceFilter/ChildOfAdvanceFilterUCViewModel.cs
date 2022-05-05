using LogGuard_v0._1._Config;
using LogGuard_v0._1.AppResources.AttachedProperties;
using LogGuard_v0._1.Base.LogGuardFlow.SourceFilter;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.FilterEngines;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using LogGuard_v0._1.Implement.UIEventHandler;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter
{
    public abstract class ChildOfAdvanceFilterUCViewModel : BaseViewModel, IMechanicalSourceFilter
    {
        private bool _isFilterBusy = false;
        private SearchBehavior _searchType = SearchBehavior.None;

        public IFilterEngine CurrentEngine { get; protected set; }

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
        public string FilterConditionHelperContent
        {
            get
            {
                return _filterConditionHelperContent;
            }
            set
            {
                _filterConditionHelperContent = value;
                InvalidateOwn();
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

        public abstract bool Filter(object obj);

        public abstract bool IsUseFilterEngine { get; }

        private string _engineHelperContent = "";
        private string _filterConditionHelperContent = "";
        private string _filterHelperContent = "";
        private string _filterContent = "";
        protected bool _isFilterEnable = false;
        private FilterType _currentFilterMode = FilterType.Simple;
        private NormalFilterEngine _normalEngine = new NormalFilterEngine();
        private AdvanceSyntaxFilterEngine _advanceSyntaxEngine = new AdvanceSyntaxFilterEngine();
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

            _syntaxEngine.PartsCollectionChanged -= OnSourcePartsCollectionChanged;
            _syntaxEngine.PartsCollectionChanged += OnSourcePartsCollectionChanged;

            _normalEngine.ComparableSourceUpdated -= OnComparableSourceUpdated;
            _normalEngine.ComparableSourceUpdated += OnComparableSourceUpdated;
            _syntaxEngine.ComparableSourceUpdated -= OnComparableSourceUpdated;
            _syntaxEngine.ComparableSourceUpdated += OnComparableSourceUpdated;
            _advanceSyntaxEngine.ComparableSourceUpdated -= OnComparableSourceUpdated;
            _advanceSyntaxEngine.ComparableSourceUpdated += OnComparableSourceUpdated;
        }

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
                    CurrentEngine = _advanceSyntaxEngine;
                    break;
                case FilterType.Syntax:
                    CurrentEngine = _syntaxEngine;
                    break;
            }
            UpdateHelperContent();
            UpdateFilterConditionHelperContent();
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

        protected virtual void UpdateFilterConditionHelperContent()
        {
            if (CurrentEngine.HelperContent == "")
            {
                FilterConditionHelperContent = "Type a few words for helpful hints!";
            }
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
            if (IsUseFilterEngine)
            {
                UpdateEngingeComparableSource(value);
            }
            else
            {
                NotifyFilterContentChanged(value);
            }
        }

        protected virtual void OnFilterEnableChanged(bool value)
        {
            NotifyFilterContentChanged(value);
        }

        protected virtual bool DoHighlight(object obj)
        {
            return false;
        }

        protected virtual void OnComparableSourceUpdated(object sender, object args)
        {
            UpdateFilterConditionHelperContent();
            NotifyFilterContentChanged(args);
        }
        protected virtual void DoCleanHighlightSource(object obj)
        {

        }

        private void OnSourceFilteredAndDisplayed(object sender)
        {
            IsFilterBusy = false;
        }

        private void OnSourcePartsCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var newFilterContent = "";
            int i = 0;
            foreach (var part in _syntaxEngine.SourceParts)
            {
                if (i == _syntaxEngine.SourceParts.Count - 1)
                {
                    newFilterContent += part;
                }
                else
                {
                    newFilterContent += part + " | ";
                }
                i++;
            }
            _filterContent = newFilterContent;
            UpdateFilterConditionHelperContent();
            NotifyFilterContentChanged(_filterContent);
            Invalidate("FilterContent");

        }

    }
}
