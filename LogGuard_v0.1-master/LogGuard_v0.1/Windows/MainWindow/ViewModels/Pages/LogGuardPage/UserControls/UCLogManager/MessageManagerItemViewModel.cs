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
    public class MessageManagerItemViewModel : TrippleToggleItemViewModel
    {
        private IMechanicalSourceFilter _messageShowFilter;
        private IMechanicalSourceFilter _messageRemoveFilter;

        private ISeparableSourceFilterEngine _messageShowFilterEngineCache;
        private ISeparableSourceFilterEngine _messageRemoveFilterEngineCache;

        public MessageManagerItemViewModel(BaseViewModel parents, TrippleToggleItemVO vo) : base(parents, vo)
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
                    _messageShowFilter.CurrentFilterMode = AppResources.AttachedProperties.FilterType.Syntax;
                    if (_messageShowFilterEngineCache == null)
                    {
                        _messageShowFilterEngineCache = _messageShowFilter.CurrentEngine as ISeparableSourceFilterEngine;
                    }

                    if (oldStat == TrippleToggleItemVO.Status.Remove
                        && _messageRemoveFilterEngineCache != null)
                    {
                        _messageRemoveFilterEngineCache.SourceParts.Remove(Content);
                    }

                    if (_messageShowFilterEngineCache != null)
                    {
                        _messageShowFilterEngineCache.SourceParts.Add(Content);
                    }
                    break;
                case TrippleToggleItemVO.Status.None:
                    if (oldStat == TrippleToggleItemVO.Status.Remove
                        && _messageRemoveFilterEngineCache != null)
                    {
                        _messageRemoveFilterEngineCache.SourceParts.Remove(Content);
                    }
                    else if (oldStat == TrippleToggleItemVO.Status.Show
                        && _messageShowFilterEngineCache != null)
                    {
                        _messageShowFilterEngineCache.SourceParts.Remove(Content);
                    }
                    break;
                case TrippleToggleItemVO.Status.Remove:
                    _messageRemoveFilter.CurrentFilterMode = AppResources.AttachedProperties.FilterType.Syntax;
                    if (_messageRemoveFilterEngineCache == null)
                    {
                        _messageRemoveFilterEngineCache = _messageRemoveFilter.CurrentEngine as ISeparableSourceFilterEngine;
                    }

                    if (oldStat == TrippleToggleItemVO.Status.Show
                        && _messageShowFilterEngineCache != null)
                    {
                        _messageShowFilterEngineCache.SourceParts.Remove(Content);
                    }

                    if (_messageRemoveFilterEngineCache != null)
                    {
                        _messageRemoveFilterEngineCache.SourceParts.Add(Content);
                    }
                    break;
            }
        }
        private void LMUCViewModelGenerated(object sender, LogManagerUCViewModel vm)
        {
            EditContentItemCommand = vm.CommandViewModel.EditMessageItemButtonCommand;
            DeleteContentItemCommand = vm.CommandViewModel.DeleteMessageItemButtonCommand;

            ViewModelHelper.Current.LogManagerUCViewModelGenerated -= LMUCViewModelGenerated;
        }

        private void AFUCViewModelGenerated(object p, AdvanceFilterUCViewModel vm)
        {
            _messageShowFilter = vm.MessageFilterContent;
            _messageRemoveFilter = vm.MessageRemoveFilterContent;

            OnTagItemStatChanged(TrippleToggleItemVO.Status.None, itemVO.Stat);
            ViewModelHelper.Current.AdvanceFilterUCViewModelGenerated -= AFUCViewModelGenerated;
        }
    }
}
