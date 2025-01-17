﻿using cyber_base.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.action.executer
{
    public interface ICommandExecuter : IAction, IDestroyable, ICancelable
    {
        /// <summary>
        ///  Dữ liệu được truyền vào trong lệnh
        /// </summary>
        IList<object>? DataTransfer { get; }

        /// <summary>
        /// Kiểm tra liệu lệnh này đã được thực thi thành công chưa 
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Kiểm tra liệu lệnh này có bị hủy trong lúc đang thực thi hay không 
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// Triển khai action thay thế cho 1 đối tượng  được định nghĩa trước
        /// </summary>
        /// <returns></returns>
        bool AlterExecute(object? dataTransfer);
    }

}
