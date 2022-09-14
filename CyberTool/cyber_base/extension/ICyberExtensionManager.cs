using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.extension
{
    public interface ICyberExtensionManager
    {
        /// <summary>
        /// Đồng bộ các file extension dll vs dữ liệu của người dùng
        /// Có thể là nạp chương trình vào app
        /// Có thể là xóa và gỡ chương trình ra khỏi app
        /// </summary>
        void SyncAllPluginDllFromUserData();

        string GetPluginInstallLocationPath(string pluginKey, string pluginVersion);
    }
}
