using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.module
{
    internal interface ILogGuardModule
    {

        /// <summary>
        /// Sự kiện xảy ra sau khi khởi tạo module
        /// </summary>
        void OnModuleStart();
    }
}
