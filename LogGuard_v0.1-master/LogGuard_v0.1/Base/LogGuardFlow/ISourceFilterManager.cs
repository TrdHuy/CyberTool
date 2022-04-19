﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.LogGuardFlow
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

        ISourceFilter LogTagFilter { get; set; }
        ISourceFilter LogMessageFilter { get; set; }
        ISourceFilter LogPidFilter { get; set; }
        ISourceFilter LogTidFilter { get; set; }
        ISourceFilter LogTagRemoveFilter { get; set; }
        ISourceFilter LogStartTimeFilter { get; set; }
        ISourceFilter LogEndTimeFilter { get; set; }
        void NotifyFilterPropertyChanged(ISourceFilter sender, object e);
    }

    public delegate void SourceFilterConditionChangedHandler(object sender, ConditionChangedEventArgs e);

    public class ConditionChangedEventArgs
    {
    }
}