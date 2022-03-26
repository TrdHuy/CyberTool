using LogGuard_v0._1.AppResources.AttachedProperties;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.FilterEngines;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.UserControls.UCAdvanceFilter
{
    public abstract class ChildOfAdvanceFilterUCViewModel : BaseViewModel, ISourceFilter
    {
        protected IFilterEngine CurrentEngine { get; set; }

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
                _isFilterEnable = value;
                SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, value);
                UpdateHelperContent();
                InvalidateOwn();
            }
        }
        [Bindable(true)]
        public string HelperContent
        {
            get
            {
                return _helperContent;
            }
            set
            {
                _helperContent = value;
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
                UpdateEngingeComparableSource(value);
                NotifyFilterContentChanged(value);
                InvalidateOwn();
            }
        }

        private string _helperContent = "";
        private string _filterContent = "";
        protected bool _isFilterEnable = false;
        private FilterType _currentFilterMode = FilterType.Simple;
        private NormalFilterEngine _normalEngine = new NormalFilterEngine();
        private SyntaxFilterEngine _syntaxEngine = new SyntaxFilterEngine();
        private Thread _notifyFilterEngineChangedMessage;

        public ChildOfAdvanceFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            UpdateEngine();
        }

        public abstract bool Filter(object obj);
        protected abstract bool IsUseFilterEngine { get; }
        public void UpdateEngine()
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

        public void UpdateEngingeComparableSource(string source)
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
            var engineHelper = "";
            if (IsUseFilterEngine)
            {
                engineHelper = "\nRight click to change filter mode" +
                "\nFilter mode: " + CurrentFilterMode.ToString();
            }
            HelperContent = "Left click to " + turnHelper + " filter" + engineHelper;
        }

        protected void NotifyFilterContentChanged(string conditionChanged)
        {
            if (IsFilterEnable)
                SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, conditionChanged);
        }
    }
}
