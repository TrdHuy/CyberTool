using LogGuard_v0._1.AppResources.AttachedProperties;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.FilterEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.UserControls.UCAdvanceFilter
{
    public abstract class ChildOfAdvanceFilterUCViewModel : BaseViewModel, ISourceFilter
    {
        protected IFilterEngine CurrentEngine { get; set; }
        protected FilterType CurrentFilterMode
        {
            get
            {
                return _currentFilterMode;
            }
            set
            {
                _currentFilterMode = value;
                UpdateEngine();
            }
        }

        private FilterType _currentFilterMode = FilterType.Simple;
        private NormalFilterEngine _normalEngine = new NormalFilterEngine();
        private SyntaxFilterEngine _syntaxEngine = new SyntaxFilterEngine();

        public ChildOfAdvanceFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            UpdateEngine();
        }

        public abstract bool Filter(object obj);

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
        }

        public virtual void UpdateEngingeComparableSource(string source)
        {
            CurrentEngine.UpdateComparableSource(source);
        }
    }
}
