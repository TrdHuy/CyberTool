using LogGuard_v0._1.Base.LogGuardFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.LogGuardFlow.FilterEngines
{
    public class NormalFilterEngine : IFilterEngine
    {
        protected string ComparableSource { get; set; }
        protected List<string> MatchedWord { get; set; }

        public NormalFilterEngine()
        {
            MatchedWord = new List<string>();
        }

        public virtual bool Contain(string input)
        {
            return input.IndexOf(ComparableSource, StringComparison.InvariantCulture) != -1;
        }

        public virtual bool ContainIgnoreCase(string input)
        {
            var contain = input.IndexOf(ComparableSource, StringComparison.InvariantCultureIgnoreCase) != -1;
            return contain;
        }

        public List<string> GetMatchWords(string input)
        {
            return MatchedWord;
        }

        public virtual bool IsVaild()
        {
            return true;
        }

        public virtual void UpdateComparableSource(string source)
        {
            ComparableSource = source;
        }
    }
}
