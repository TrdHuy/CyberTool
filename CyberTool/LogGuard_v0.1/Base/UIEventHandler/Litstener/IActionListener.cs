﻿using LogGuard_v0._1.Base.UIEventHandler.Action;
using LogGuard_v0._1.Base.UIEventHandler.Action.Builder;
using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.UIEventHandler.Litstener
{
    interface IActionListener
    {
        /// <summary>
        /// Thực hiện hành động click vao 1 button
        /// </summary>
        /// <typeparam name="windowTag">là chuỗi key để xác định window nào đang gọi</typeparam>
        /// <typeparam name="keyFeature">là chuỗi key để xác định đó là feature gì</typeparam>
        /// <typeparam name="obj">data transfer giữa các class</typeparam>
        IAction OnKey(string windowTag, string keyFeature, object obj);

        /// <summary>
        /// Thực hiện hành động click vao 1 button
        /// </summary>
        /// <typeparam name="windowTag">là chuỗi key để xác định window nào đang gọi</typeparam>
        /// <typeparam name="keyFeature">là chuỗi key để xác định đó là feature gì</typeparam>
        /// <typeparam name="obj">data transfer giữa các class</typeparam>
        /// <typeparam name="locker">khóa factory sau khi tạo action</typeparam>
        IAction OnKey(string windowTag, string keyFeature, object obj, BuilderLocker locker);

        /// <summary>
        /// Thực hiện hành động click vao 1 button
        /// </summary>
        /// <typeparam name="viewModel">view model đang gọi onkey</typeparam>
        /// <typeparam name="windowTag">là chuỗi key để xác định window nào đang gọi</typeparam>
        /// <typeparam name="logger">ghi log</typeparam>
        /// <typeparam name="keyFeature">là chuỗi key để xác định đó là feature gì</typeparam>
        /// <typeparam name="obj">data transfer giữa các class</typeparam>
        IAction OnKey(BaseViewModel viewModel, ILogger logger, string windowTag, string keyFeature, object obj);

        /// <summary>
        /// Thực hiện hành động click vao 1 button
        /// </summary>
        /// <typeparam name="viewModel">view model đang gọi onkey</typeparam>
        /// <typeparam name="logger">ghi log</typeparam>
        /// <typeparam name="obj">data transfer giữa các class</typeparam>
        /// <typeparam name="locker">khóa factory sau khi tạo action</typeparam>
        IAction OnKey(BaseViewModel viewModel, ILogger logger, string windowTag, string keyFeature, object obj, BuilderLocker locker);
    }
}
