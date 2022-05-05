using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.LogGuardFlow.SourceFilter;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.ViewModels;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager
{
    public class TagManagerItemViewModel : TrippleToggleItemViewModel
    {
        private IMechanicalSourceFilter _tagShowFilter;
        private IMechanicalSourceFilter _tagRemoveFilter;

        private ISeparableSourceFilterEngine _tagShowFilterEngineCache;
        private ISeparableSourceFilterEngine _tagRemoveFilterEngineCache;

        public TagManagerItemViewModel(BaseViewModel parents, TrippleToggleItemVO vo) : base(parents, vo)
        {
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
        }

        protected override void OnTagItemStatChanged(TrippleToggleItemVO.Status oldStat, TrippleToggleItemVO.Status newStat)
        {
            switch (newStat)
            {
                case TrippleToggleItemVO.Status.Show:
                    _tagShowFilter.CurrentFilterMode = AppResources.AttachedProperties.FilterType.Syntax;
                    if (_tagShowFilterEngineCache == null)
                    {
                        _tagShowFilterEngineCache = _tagShowFilter.CurrentEngine as ISeparableSourceFilterEngine;
                    }

                    if (oldStat == TrippleToggleItemVO.Status.Remove
                        && _tagRemoveFilterEngineCache != null)
                    {
                        _tagRemoveFilterEngineCache.SourceParts.Remove(Content);
                    }

                    if (_tagShowFilterEngineCache != null)
                    {
                        _tagShowFilterEngineCache.SourceParts.Add(Content);
                    }
                    break;
                case TrippleToggleItemVO.Status.None:
                    if (oldStat == TrippleToggleItemVO.Status.Remove
                        && _tagRemoveFilterEngineCache != null)
                    {
                        _tagRemoveFilterEngineCache.SourceParts.Remove(Content);
                    }
                    else if (oldStat == TrippleToggleItemVO.Status.Show
                        && _tagShowFilterEngineCache != null)
                    {
                        _tagShowFilterEngineCache.SourceParts.Remove(Content);
                    }
                    break;
                case TrippleToggleItemVO.Status.Remove:
                    _tagRemoveFilter.CurrentFilterMode = AppResources.AttachedProperties.FilterType.Syntax;
                    if (_tagRemoveFilterEngineCache == null)
                    {
                        _tagRemoveFilterEngineCache = _tagRemoveFilter.CurrentEngine as ISeparableSourceFilterEngine;
                    }

                    if (oldStat == TrippleToggleItemVO.Status.Show
                        && _tagShowFilterEngineCache != null)
                    {
                        _tagShowFilterEngineCache.SourceParts.Remove(Content);
                    }

                    if (_tagRemoveFilterEngineCache != null)
                    {
                        _tagRemoveFilterEngineCache.SourceParts.Add(Content);
                    }
                    break;
            }
        }
        private void LMUCViewModelGenerated(object sender, LogManagerUCViewModel vm)
        {
            EditContentItemCommand = vm.CommandViewModel.EditTagItemButtonCommand;
            DeleteContentItemCommand = vm.CommandViewModel.DeleteTagItemButtonCommand;

            ViewModelHelper.Current.LogManagerUCViewModelGenerated -= LMUCViewModelGenerated;
        }

        private void AFUCViewModelGenerated(object p, AdvanceFilterUCViewModel vm)
        {
            _tagShowFilter = vm.TagFilterContent;
            _tagRemoveFilter = vm.TagRemoveContent;

            OnTagItemStatChanged(TrippleToggleItemVO.Status.None, itemVO.Stat);
            ViewModelHelper.Current.AdvanceFilterUCViewModelGenerated -= AFUCViewModelGenerated;
        }
    }
}
