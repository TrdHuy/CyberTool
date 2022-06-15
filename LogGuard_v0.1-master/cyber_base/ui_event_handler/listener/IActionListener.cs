using cyber_base.ui_event_handler.action;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.utils;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.listener
{
    public interface IActionListener
    {
        /// <summary>
        /// Thực hiện hành động click vao 1 button
        /// </summary>
        /// <typeparam name="windowTag">là chuỗi key để xác định window nào đang gọi</typeparam>
        /// <typeparam name="keyFeature">là chuỗi key để xác định đó là feature gì</typeparam>
        /// <typeparam name="obj">data transfer giữa các class</typeparam>
        IAction OnKey(string builderTag, string keyFeature, object obj);

        /// <summary>
        /// Thực hiện hành động click vao 1 button
        /// </summary>
        /// <typeparam name="windowTag">là chuỗi key để xác định window nào đang gọi</typeparam>
        /// <typeparam name="keyFeature">là chuỗi key để xác định đó là feature gì</typeparam>
        /// <typeparam name="obj">data transfer giữa các class</typeparam>
        /// <typeparam name="locker">khóa factory sau khi tạo action</typeparam>
        IAction OnKey(string builderTag, string keyFeature, object obj, BuilderLocker locker);

        /// <summary>
        /// Thực hiện hành động click vao 1 button
        /// </summary>
        /// <typeparam name="viewModel">view model đang gọi onkey</typeparam>
        /// <typeparam name="windowTag">là chuỗi key để xác định window nào đang gọi</typeparam>
        /// <typeparam name="logger">ghi log</typeparam>
        /// <typeparam name="keyFeature">là chuỗi key để xác định đó là feature gì</typeparam>
        /// <typeparam name="obj">data transfer giữa các class</typeparam>
        IAction OnKey(BaseViewModel? viewModel, ILogger logger, string builderTag, string keyFeature, object obj);

        /// <summary>
        /// Thực hiện hành động click vao 1 button
        /// </summary>
        /// <typeparam name="viewModel">view model đang gọi onkey</typeparam>
        /// <typeparam name="logger">ghi log</typeparam>
        /// <typeparam name="obj">data transfer giữa các class</typeparam>
        /// <typeparam name="locker">khóa factory sau khi tạo action</typeparam>
        IAction OnKey(BaseViewModel? viewModel, ILogger logger, string builderTag, string keyFeature, object obj, BuilderLocker locker);
    }
}
