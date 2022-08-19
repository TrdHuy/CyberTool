using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.device
{
    interface IDeviceCmdExecuter
    {
        /// <summary>
        /// Tạo một process với lệnh cmd 
        /// </summary>
        /// <param name="cmd">lệnh cmd truyền vào</param>
        /// <returns></returns>
        Process CreateProcess(string cmd);

        /// <summary>
        /// Tạo một lệnh cmd trên ADB
        /// </summary>
        /// <param name="command">lệnh command cần tạo </param>
        /// <param name="type">kiểu lệnh </param>
        /// <param name="asroot">chạy lệnh với quyền root </param>
        /// <param name="multiDevice">chạy lệnh với nhiều device </param>
        /// <param name="serialNumber">id của device để chạy lệnh </param>
        /// <returns></returns>
        string CreateCommandADB(string command, int type, bool asroot, bool multiDevice, string serialNumber);
    }
}
