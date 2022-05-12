using log_guard.@base.flow.highlight;
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

        List<MatchedWord> GetMatchWords();

        bool IsMatchLstEmpty { get; }

        string ComparableSource { get; }

        string HelperContent { get; }

        event OnComparableSourceUpdatedHandler ComparableSourceUpdated;
    }

    public delegate void OnComparableSourceUpdatedHandler (object sender, object args);

    public class MatchedWord : IHighlightable
    {
        public string SearchWord { get; }
        public int StartIndex { get; }
        public int WordLength { get; }
        public string RawWord { get; }

        public MatchedWord(int startIndex, string word, string rawWord)
        {
            StartIndex = startIndex;
            WordLength = word.Length;
            SearchWord = word;
            RawWord = rawWord;
        }
    }
}
