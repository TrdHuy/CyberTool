using log_guard.@base.flow.highlight;
using log_guard.models.vo;
using System.Collections.Generic;

namespace log_guard.@base.flow.source_filter
{
    internal interface IFilterEngine
    {
        bool Contain(string input);

        bool ContainIgnoreCase(string input);

        void UpdateComparableSource(string source);

        bool IsVaild();

        void Refresh();

        List<MatchedWordVO> GetMatchWords();

        bool IsMatchLstEmpty { get; }

        string ComparableSource { get; }

        string HelperContent { get; }

        event OnComparableSourceUpdatedHandler ComparableSourceUpdated;
    }

    public delegate void OnComparableSourceUpdatedHandler (object sender, object args);

    
}
