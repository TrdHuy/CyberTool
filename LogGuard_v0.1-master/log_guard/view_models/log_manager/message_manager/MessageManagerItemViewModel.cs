using cyber_base.view_model;
using log_guard.@base.flow.source_filter;
using log_guard.@base.flow.source_filterr;
using log_guard.implement.flow.view_model;
using log_guard.models.vo;
using log_guard.view_models.advance_filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.log_manager.message_manager
{
    internal class MessageManagerItemViewModel : TrippleToggleItemViewModel
    {
        private IMechanicalSourceFilter _messageShowFilter;
        private IMechanicalSourceFilter _messageRemoveFilter;

        private ISeparableSourceFilterEngine? _messageShowFilterEngineCache;
        private ISeparableSourceFilterEngine? _messageRemoveFilterEngineCache;

        public MessageManagerItemViewModel(BaseViewModel parents, TrippleToggleItemVO vo) : base(parents, vo)
        {
            EditContentItemCommand = ViewModelManager.Current.LogManagerUCViewModel.CommandViewModel.EditMessageItemButtonCommand;
            DeleteContentItemCommand = ViewModelManager.Current.LogManagerUCViewModel.CommandViewModel.DeleteMessageItemButtonCommand;
            _messageShowFilter = ViewModelManager.Current.AdvanceFilterUCViewModel.MessageFilterContent;
            _messageRemoveFilter = ViewModelManager.Current.AdvanceFilterUCViewModel.MessageRemoveFilterContent;

            OnTagItemStatChanged(TrippleToggleItemVO.Status.None, itemVO.Stat);
        }

        protected override void OnTagItemStatChanged(TrippleToggleItemVO.Status oldStat, TrippleToggleItemVO.Status newStat)
        {
            switch (newStat)
            {
                case TrippleToggleItemVO.Status.Show:
                    _messageShowFilter.CurrentFilterMode = FilterType.Syntax;
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
                    _messageRemoveFilter.CurrentFilterMode = FilterType.Syntax;
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
    }
}
