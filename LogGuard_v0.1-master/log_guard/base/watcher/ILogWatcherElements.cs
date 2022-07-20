using log_guard.definitions;
using System.Drawing;

namespace log_guard.@base.watcher
{
    internal interface ILogWatcherElements
    {
        Color? TrackColor { get; }

        Color? ErrorColor { get; }

        /// <summary>
        /// thứ tự dòng của phần tử trong log watcher
        /// </summary>
        int LineNumber { get; set; }

        /// <summary>
        /// Chế độ hiển thị của mỗi dòng log
        /// </summary>
        ElementViewType ViewType { get; set; }

    }

    
}
