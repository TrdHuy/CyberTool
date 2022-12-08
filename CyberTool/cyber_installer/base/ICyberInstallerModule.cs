using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.@base
{
    internal interface ICyberInstallerModule
    {
        /// <summary>
        /// Sự kiện xảy ra khi bắt đầu khởi tạo module
        /// </summary>
        void OnModuleCreate();

        /// <summary>
        /// Sự kiện xảy ra sau khi khởi tạo module
        /// </summary>
        void OnModuleStart();

        /// <summary>
        /// Sự kiện xảy ra sau khi main window hiển thị
        /// </summary>
        void OnMainWindowShowed();

        /// <summary>
        /// Sự kiện xảy ra khi module bị phá bỏ
        /// </summary>
        void OnModuleDestroy();
    }
}
