using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.models.user_data
{
    internal class PluginVersionUD
    {
        public string Version { get; set; } = "";
        public string DownloadFilePath { get; set; } = "";
        public string ExecutePath { get; set; } = "";
        public string MainClassName { get; set; } = "";
        public PluginVersionStatus VersionStatus { get; set; } = PluginVersionStatus.None;
    }

    internal enum PluginVersionStatus
    {
        None = 0,
        VersionDownloadedButWithoutInstalled = 1,
        VersionInstalled = 2,
    }
}
