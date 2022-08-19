using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogGuard_v0._1.LogGuard.Base
{
    public interface IExpandableElements : ILogWatcherElements
    {
        /// <summary>
        /// Tập con của view có thể mở rộng
        /// </summary>
        List<ILogWatcherElements> Childs { get; set; }

        /// <summary>
        /// Command khi click nút mở rộng trong chế độ view Expandable
        /// </summary>
        ICommand ExpandButtonCommand { get; set; }

        /// <summary>
        /// Command khi click nút xóa trong chế độ view Expandable
        /// </summary>
        ICommand DeleteButtonCommand { get; set; }
    }
}
