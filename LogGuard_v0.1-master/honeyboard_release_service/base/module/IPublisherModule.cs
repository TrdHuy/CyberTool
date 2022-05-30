using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.@base.module
{
    internal interface IPublisherModule
    {
        /// <summary>
        /// Sự kiện xảy ra sau khi khởi tạo module
        /// </summary>
        void OnModuleStart();

        /// <summary>
        /// Sự kiện xảy ra sau khi service view được khởi tạo nhưng chưa load
        /// </summary>
        void OnViewInstantiated();
    }
}
