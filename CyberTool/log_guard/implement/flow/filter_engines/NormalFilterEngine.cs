using log_guard.@base.flow.source_filter;
using log_guard.models.vo;
using System;
using System.Collections.Generic;

namespace log_guard.implement.flow.filter_engines
{
    internal class NormalFilterEngine : IFilterEngine
    {
        public string ComparableSource { get; protected set; }
        protected List<MatchedWordVO> MatchedWords { get; set; }

        public bool IsMatchLstEmpty { get; protected set; }

        public string HelperContent { get; protected set; }

        public NormalFilterEngine()
        {
            MatchedWords = new List<MatchedWordVO>();
        }

        public event OnComparableSourceUpdatedHandler ComparableSourceUpdated;

        public virtual bool Contain(string input)
        {
            IsMatchLstEmpty = true;
            MatchedWords.Clear();
            var contain = input.IndexOf(ComparableSource, StringComparison.InvariantCulture);
            if (contain != -1)
            {
                MatchedWords.Add(new MatchedWordVO(contain, ComparableSource, input));
                IsMatchLstEmpty = false;
            }
            return contain != -1;
        }

        public virtual bool ContainIgnoreCase(string input)
        {
            IsMatchLstEmpty = true;
            MatchedWords.Clear();
            var contain = input.IndexOf(ComparableSource, StringComparison.InvariantCultureIgnoreCase);
            if (contain != -1)
            {
                MatchedWords.Add(new MatchedWordVO(contain, ComparableSource, input));
                IsMatchLstEmpty = false;
            }
            return contain != -1;
        }

        public List<MatchedWordVO> GetMatchWords()
        {
            return MatchedWords;
        }

        public virtual bool IsVaild()
        {
            return !string.IsNullOrEmpty(ComparableSource);
        }

        public void UpdateComparableSource(string source)
        {
            UpdatingSource(source);
            ComparableSourceUpdated?.Invoke(this, source);
        }

        protected virtual void UpdatingSource(string source)
        {
            ComparableSource = source;
            if (string.IsNullOrEmpty(ComparableSource))
            {
                HelperContent = "";
            }
            else
            {
                HelperContent = "\"" + ComparableSource + "\"";
            }
        }

        public void Refresh()
        {
            MatchedWords.Clear();
            IsMatchLstEmpty = true;
        }
    }
}
