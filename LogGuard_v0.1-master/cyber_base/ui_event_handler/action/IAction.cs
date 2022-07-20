using cyber_base.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.action
{
    public interface IAction
    {
        /// <summary>
        /// Triển khai action cho 1 đối tượng  được định nghĩa trước
        /// </summary>
        bool Execute(object? dataTransfer);

        /// <summary>
        /// ID of Action
        /// </summary>
        string ActionID { get; }

        /// <summary>
        /// Name of action
        /// </summary>
        string ActionName { get; }

        /// <summary>
        /// Builder id of Action
        /// </summary>
        string BuilderID { get; }

        /// <summary>
        /// action logger
        /// </summary>
        ILogger? Logger { get; }
    }
}
