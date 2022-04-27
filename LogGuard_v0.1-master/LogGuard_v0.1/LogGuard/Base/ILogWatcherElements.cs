using LogGuard_v0._1.Base.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogGuard_v0._1.LogGuard.Base
{
    public interface ILogWatcherElements
    {
        Color? TrackColor { get; }

        Color? ErrorColor { get; }

        /// <summary>
        /// thứ tự dòng của phần tử trong log watcher
        /// </summary>
        int LineNumber { get; }

        /// <summary>
        /// Chế độ hiển thị của mỗi dòng log
        /// </summary>
        ElementViewType ViewType { get; set; }

    }

    public enum ElementViewType
    {
        /// <summary>
        /// kiểu view cho android log
        /// </summary>
        LogView = 0,

        /// <summary>
        /// kiểu view cho row có thể mở rộng 
        /// </summary>
        ExpandableRowView = 1,
    }
}
