using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.model
{
    internal struct UserData
    {
        public UserData()
        {
            ToolData = new List<ToolData>();
        }

        public ICollection<ToolData> ToolData { get; set; }
    }

    internal class ToolData
    {
        public string ToolKey { get; set; } = "";
        public ICollection<ToolVersionData>? ToolVersionSource { get; set; }
        public string CurrentInstalledVersion { get; set; } = "";
        public string InstallPath { get; set; } = "";
        public string ExecutePath { get; set; } = "";
        public ToolStatus ToolStatus { get; set; }
    }

    internal class ToolVersionData
    {
        public string Version { get; set; } = "";
        public string DownloadFilePath { get; set; } = "";
        public string ExecutePath { get; set; } = "";
        public ToolVersionStatus VersionStatus { get; set; } = ToolVersionStatus.None;
    }

    internal enum ToolStatus
    {
        Downloaded = 0,
        Installed = 1,
        InstallFailed = 3,
        Removed = 2
    }

    internal enum ToolVersionStatus
    {
        None = 0,
        VersionDownloadedButWithoutInstalled = 1,
        VersionInstalled = 2,
        VersionInstalledFail = 3,
    }
}
