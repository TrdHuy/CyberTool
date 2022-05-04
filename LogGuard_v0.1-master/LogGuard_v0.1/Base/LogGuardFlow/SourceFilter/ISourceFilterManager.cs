using LogGuard_v0._1.Base.LogGuardFlow.SourceFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.LogGuardFlow.SourceFilter
{
    public interface ISourceFilterManager
    {
        event SourceFilterConditionChangedHandler FilterConditionChanged;

        /// <summary>
        /// Lọc các giá trị theo điều kiện, trả về kiểu bool
        /// </summary>
        /// <param name="obj">đối tượng cần kiểm tra điều kiện để lọc</param>
        /// <returns></returns>
        bool Filter(object obj);

        IMechanicalSourceFilter LogTagFilter { get; set; }
        IMechanicalSourceFilter LogMessageFilter { get; set; }
        IMechanicalSourceFilter LogMessageRemoveFilter { get; set; }
        IMechanicalSourceFilter LogPidFilter { get; set; }
        IMechanicalSourceFilter LogTidFilter { get; set; }
        IMechanicalSourceFilter LogTagRemoveFilter { get; set; }
        IMechanicalSourceFilter LogStartTimeFilter { get; set; }
        IMechanicalSourceFilter LogEndTimeFilter { get; set; }
        ISourceFilter LogLevelFilter { get; set; }
        void NotifyFilterPropertyChanged(ISourceFilter sender, object e);
    }

    public delegate void SourceFilterConditionChangedHandler(object sender, ConditionChangedEventArgs e);

    public class ConditionChangedEventArgs
    {
    }
}
