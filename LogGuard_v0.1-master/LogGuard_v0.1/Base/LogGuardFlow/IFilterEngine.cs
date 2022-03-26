using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.LogGuardFlow
{
    public interface IFilterEngine
    {
        bool Contain(string input);

        bool ContainIgnoreCase(string input);

        void UpdateComparableSource(string source);

        bool IsVaild();

        List<string> GetMatchWords(string input);
    }
}
