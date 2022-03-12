using LogGuard_v0._1.Base.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.UIEventHandler.Action
{
    public interface IAction
    {
        /// <summary>
        /// Triển khai action cho 1 đối tượng  được định nghĩa trước
        /// </summary>
        bool Execute(object dataTransfer);

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
        ILogger Logger { get; }
    }
}
