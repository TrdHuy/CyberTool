using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.flow.highlight
{
    internal interface IHighlightable
    {
        string SearchWord { get; }
        int StartIndex { get; }
        int WordLength { get; }
        string RawWord { get; }
    }
}
