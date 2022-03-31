using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.LogGuardFlow
{
    public interface ISourceFilterManager : ISourceFilter
    {
        event SourceFilterConditionChangedHandler FilterConditionChanged;
        ISourceFilter LogTagFilter { get; set; }
        ISourceFilter LogMessageFilter { get; set; }
        ISourceFilter LogPidFilter { get; set; }
        ISourceFilter LogTidFilter { get; set; }
        ISourceFilter LogTagRemoveFilter { get; set; }
        void NotifyFilterPropertyChanged(ISourceFilter sender, object e);
    }

    public delegate void SourceFilterConditionChangedHandler(object sender, ConditionChangedEventArgs e);

    public class ConditionChangedEventArgs
    {
    }
}
