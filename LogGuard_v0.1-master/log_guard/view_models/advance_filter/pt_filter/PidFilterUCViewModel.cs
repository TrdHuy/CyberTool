using cyber_base.view_model;
using log_guard.models.vo;
using log_guard.view_models.watcher;
using System;
using System.Collections.Generic;

namespace log_guard.view_models.advance_filter.pt_filter
{
    internal class PidFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        private List<MatchedWordVO> matchedWords;

        public override bool IsUseFilterEngine => false;

        public PidFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            matchedWords = new List<MatchedWordVO>();
        }

        public override bool Filter(object obj)
        {
            matchedWords.Clear();

            if (string.IsNullOrEmpty(FilterContent))
            {
                return true;
            }

            var data = obj as LWI_ParseableViewModel;
            data.HighlightPidSource = null;

            if (IsFilterEnable && data?.Pid != null)
            {
                var contain = data
                    .Pid
                    .ToString()
                    .IndexOf(FilterContent, StringComparison.InvariantCultureIgnoreCase);
                if (contain != -1)
                {
                    matchedWords.Add(new MatchedWordVO(contain, FilterContent, data.Pid.ToString()));
                }
                data.HighlightPidSource = matchedWords.ToArray();
                return contain != -1;
            }

            return true;
        }

    }
}
