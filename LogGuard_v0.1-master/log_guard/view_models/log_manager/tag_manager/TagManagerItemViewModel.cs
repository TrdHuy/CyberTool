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

namespace log_guard.view_models.log_manager.tag_manager
{
    internal class TagManagerItemViewModel : TrippleToggleItemViewModel
    {
        private IMechanicalSourceFilter _tagShowFilter;
        private IMechanicalSourceFilter _tagRemoveFilter;

        private ISeparableSourceFilterEngine? _tagShowFilterEngineCache;
        private ISeparableSourceFilterEngine? _tagRemoveFilterEngineCache;

        public TagManagerItemViewModel(BaseViewModel parents, TrippleToggleItemVO vo) : base(parents, vo)
        {
            EditContentItemCommand = ViewModelManager.Current.LogManagerUCViewModel.CommandViewModel.EditTagItemButtonCommand;
            DeleteContentItemCommand = ViewModelManager.Current.LogManagerUCViewModel.CommandViewModel.DeleteTagItemButtonCommand;

            _tagShowFilter = ViewModelManager.Current.AdvanceFilterUCViewModel.TagFilterContent;
            _tagRemoveFilter = ViewModelManager.Current.AdvanceFilterUCViewModel.TagRemoveContent;

            OnTagItemStatChanged(TrippleToggleItemVO.Status.None, itemVO.Stat);
        }

        protected override void OnTagItemStatChanged(TrippleToggleItemVO.Status oldStat, TrippleToggleItemVO.Status newStat)
        {
            switch (newStat)
            {
                case TrippleToggleItemVO.Status.Show:
                    _tagShowFilter.CurrentFilterMode = FilterType.Syntax;
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
                    _tagRemoveFilter.CurrentFilterMode = FilterType.Syntax;
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
    }
}
