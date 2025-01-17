﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_core.@base.module
{
    internal interface ICyberCoreModule
    {
        /// <summary>
        /// Sự kiện xảy ra khi bắt đầu khởi tạo module
        /// </summary>
        void OnModuleInit();

        /// <summary>
        /// Sự kiện xảy ra sau khi khởi tạo module
        /// </summary>
        void OnModuleStart();

        /// <summary>
        /// Sự kiện xảy ra sau khi IFace window hiển thị
        /// </summary>
        void OnIFaceWindowShowed();

        /// <summary>
        /// Sự kiện xảy ra khi module bị phá bỏ
        /// </summary>
        void OnModuleDestroy();
    }
}
