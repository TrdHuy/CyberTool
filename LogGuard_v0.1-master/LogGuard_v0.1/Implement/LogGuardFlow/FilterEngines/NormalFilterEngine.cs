using LogGuard_v0._1.Base.LogGuardFlow.SourceFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.LogGuardFlow.FilterEngines
{
    public class NormalFilterEngine : IFilterEngine
    {
        public string ComparableSource { get; protected set; }
        protected List<MatchedWord> MatchedWords { get; set; }

        public bool IsMatchLstEmpty { get; protected set; }

        public string HelperContent { get; protected set; }

        public NormalFilterEngine()
        {
            MatchedWords = new List<MatchedWord>();
        }

        public virtual bool Contain(string input)
        {
            IsMatchLstEmpty = true;
            MatchedWords.Clear();
            var contain = input.IndexOf(ComparableSource, StringComparison.InvariantCulture);
            if (contain != -1)
            {
                MatchedWords.Add(new MatchedWord(contain, ComparableSource, input));
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
                MatchedWords.Add(new MatchedWord(contain, ComparableSource, input));
                IsMatchLstEmpty = false;
            }
            return contain != -1;
        }

        public List<MatchedWord> GetMatchWords()
        {
            return MatchedWords;
        }

        public virtual bool IsVaild()
        {
            return !string.IsNullOrEmpty(ComparableSource);
        }

        public virtual void UpdateComparableSource(string source)
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
