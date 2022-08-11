using log_guard.@base.flow.highlight;

namespace log_guard.models.vo
{
    internal class MatchedWordVO : IHighlightable
    {
        public string SearchWord { get; }
        public int StartIndex { get; }
        public int WordLength { get; }
        public string RawWord { get; }

        public MatchedWordVO(int startIndex, string word, string rawWord)
        {
            StartIndex = startIndex;
            WordLength = word.Length;
            SearchWord = word;
            RawWord = rawWord;
        }
    }
}
