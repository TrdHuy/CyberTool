using cyber_base.view_model;
using log_guard.models.vo;
using log_guard.view_models.watcher;
using System;
using System.Collections.Generic;


namespace log_guard.view_models.advance_filter.pt_filter
{
    internal class TidFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        private List<MatchedWordVO> matchedWords;

        public TidFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            matchedWords = new List<MatchedWordVO>();
        }
        public override bool IsUseFilterEngine => false;

        public override bool Filter(object obj)
        {
            matchedWords.Clear();
            if (string.IsNullOrEmpty(FilterContent))
            {
                return true;
            }

            var data = obj as LWI_ParseableViewModel;
            data.HighlightTidSource = null;

            if (IsFilterEnable && data?.Tid != null)
            {
                var contain = data
                    .Tid
                    .ToString()
                    .IndexOf(FilterContent, StringComparison.InvariantCultureIgnoreCase);
                if(contain != -1)
                {
                    matchedWords.Add(new MatchedWordVO(contain, FilterContent, data.Tid.ToString()));
                }
                data.HighlightTidSource = matchedWords.ToArray();
                return contain != -1;
            }

            return true;
        }

    }
}
