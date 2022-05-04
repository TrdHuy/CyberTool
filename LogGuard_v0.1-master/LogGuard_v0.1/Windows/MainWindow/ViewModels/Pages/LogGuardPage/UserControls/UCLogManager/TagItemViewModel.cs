using LogGuard_v0._1.Base.Command;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.LogGuardFlow.SourceFilter;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Implement.ViewModels;
using LogGuard_v0._1.LogGuard.Control;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager
{
    public class TagItemViewModel : BaseViewModel
    {
        private LogTagVO tagVO;
        private bool _isEditMode;
        private IMechanicalSourceFilter _tagShowFilter;
        private IMechanicalSourceFilter _tagRemoveFilter;

        public TagItemViewModel(BaseViewModel parents, LogTagVO vo)
        {
            tagVO = vo;
            if (ViewModelHelper.Current.LogManagerUCViewModel == null)
            {
                ViewModelHelper.Current.LogManagerUCViewModelGenerated -= LMUCViewModelGenerated;
                ViewModelHelper.Current.LogManagerUCViewModelGenerated += LMUCViewModelGenerated;
            }
            else
            {
                LMUCViewModelGenerated(null, ViewModelHelper.Current.LogManagerUCViewModel);
            }

            if (ViewModelHelper.Current.AdvanceFilterUCViewModel == null)
            {
                ViewModelHelper.Current.AdvanceFilterUCViewModelGenerated -= AFUCViewModelGenerated;
                ViewModelHelper.Current.AdvanceFilterUCViewModelGenerated += AFUCViewModelGenerated;
            }
            else
            {
                AFUCViewModelGenerated(null, ViewModelHelper.Current.AdvanceFilterUCViewModel);
            }

            ShowTagItemCommand = new BaseDotNetCommandImpl((s) =>
            {
                Stat = DotStatus.DotOn;
            });
            RemoveTagItemCommand = new BaseDotNetCommandImpl((s) =>
            {
                Stat = DotStatus.DotOff;
            });
            OffTagItemCommand = new BaseDotNetCommandImpl((s) =>
            {
                Stat = DotStatus.DotNormal;
            });
        }

        private void AFUCViewModelGenerated(object sender, AdvanceFilterUCViewModel vm)
        {
            _tagShowFilter = vm.TagFilterContent;
            _tagRemoveFilter = vm.TagRemoveContent;

            OnTagItemStatChanged(LogTagVO.Status.None, tagVO.Stat);
            ViewModelHelper.Current.AdvanceFilterUCViewModelGenerated -= AFUCViewModelGenerated;
        }

        private void LMUCViewModelGenerated(object sender, LogManagerUCViewModel vm)
        {
            EditTagItemCommand = vm.CommandViewModel.EditTagItemButtonCommand;
            DeleteTagItemCommand = vm.CommandViewModel.DeleteTagItemButtonCommand;

            ViewModelHelper.Current.LogManagerUCViewModelGenerated -= LMUCViewModelGenerated;
        }

        public LogTagVO TagVO
        {
            get
            {
                return tagVO;
            }
        }

        [Bindable(true)]
        public ICommand ShowTagItemCommand { get; set; }

        [Bindable(true)]
        public ICommand RemoveTagItemCommand { get; set; }

        [Bindable(true)]
        public ICommand OffTagItemCommand { get; set; }


        [Bindable(true)]
        public ICommand DeleteTagItemCommand { get; set; }

        [Bindable(true)]
        public ICommand EditTagItemCommand { get; set; }

        [Bindable(true)]
        public bool IsEditMode
        {
            get
            {
                return _isEditMode;
            }
            set
            {
                _isEditMode = value;
                InvalidateOwn();
            }
        }


        [Bindable(true)]
        public string Tag
        {
            get
            {
                return tagVO.Tag;
            }
            set
            {
                tagVO.Tag = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public DotStatus Stat
        {
            get
            {
                switch (tagVO.Stat)
                {
                    case LogTagVO.Status.None:
                        return DotStatus.DotNormal;
                    case LogTagVO.Status.Show:
                        return DotStatus.DotOn;
                    case LogTagVO.Status.Remove:
                        return DotStatus.DotOff;
                    default:
                        return DotStatus.DotNormal;
                }
            }
            set
            {
                var oldValue = tagVO.Stat;
                switch (value)
                {
                    case DotStatus.DotOn:
                        tagVO.Stat = LogTagVO.Status.Show;
                        break;
                    case DotStatus.DotNormal:
                        tagVO.Stat = LogTagVO.Status.None;
                        break;
                    case DotStatus.DotOff:
                        tagVO.Stat = LogTagVO.Status.Remove;
                        break;
                    default:
                        tagVO.Stat = LogTagVO.Status.None;
                        break;
                }
                OnTagItemStatChanged(oldValue, tagVO.Stat);
                InvalidateOwn();
            }
        }

        private ISeparableSourceFilterEngine _tagShowFilterEngineCache;
        private ISeparableSourceFilterEngine _tagRemoveFilterEngineCache;

        private void OnTagItemStatChanged(LogTagVO.Status oldStat, LogTagVO.Status newStat)
        {
            switch (newStat)
            {
                case LogTagVO.Status.Show:
                    _tagShowFilter.CurrentFilterMode = AppResources.AttachedProperties.FilterType.Syntax;
                    if (_tagShowFilterEngineCache == null)
                    {
                        _tagShowFilterEngineCache = _tagShowFilter.CurrentEngine as ISeparableSourceFilterEngine;
                    }

                    if (oldStat == LogTagVO.Status.Remove
                        && _tagRemoveFilterEngineCache != null)
                    {
                        _tagRemoveFilterEngineCache.SourceParts.Remove(Tag);
                    }

                    if (_tagShowFilterEngineCache != null)
                    {
                        _tagShowFilterEngineCache.SourceParts.Add(Tag);
                    }
                    break;
                case LogTagVO.Status.None:
                    if (oldStat == LogTagVO.Status.Remove
                        && _tagRemoveFilterEngineCache != null)
                    {
                        _tagRemoveFilterEngineCache.SourceParts.Remove(Tag);
                    }
                    else if (oldStat == LogTagVO.Status.Show
                        && _tagShowFilterEngineCache != null)
                    {
                        _tagShowFilterEngineCache.SourceParts.Remove(Tag);
                    }
                    break;
                case LogTagVO.Status.Remove:
                    _tagRemoveFilter.CurrentFilterMode = AppResources.AttachedProperties.FilterType.Syntax;
                    if (_tagRemoveFilterEngineCache == null)
                    {
                        _tagRemoveFilterEngineCache = _tagRemoveFilter.CurrentEngine as ISeparableSourceFilterEngine;
                    }

                    if (oldStat == LogTagVO.Status.Show
                        && _tagShowFilterEngineCache != null)
                    {
                        _tagShowFilterEngineCache.SourceParts.Remove(Tag);
                    }

                    if (_tagRemoveFilterEngineCache != null)
                    {
                        _tagRemoveFilterEngineCache.SourceParts.Add(Tag);
                    }
                    break;
            }
        }
    }
}
