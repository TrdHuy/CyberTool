using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.@base
{
    internal interface IExtensionManagerModule
    {
        /// <summary>
        /// Sự kiện xảy ra sau khi khởi tạo module
        /// </summary>
        void OnModuleStart();

        /// <summary>
        /// Sự kiện xảy ra sau khi service view được khởi tạo nhưng chưa load
        /// </summary>
        void OnViewInstantiated();

        /// <summary>
        /// Sự kiện xảy ra khi service bị hủy
        /// </summary>
        void OnDestroy();
    }
}
