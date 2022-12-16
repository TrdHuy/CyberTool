using cyber_installer.@base.model;
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
            CertificateData = new CertificateData();
        }

        public ICollection<ToolData> ToolData { get; set; }
        public CertificateData CertificateData { get; set; }
    }

    internal class CertificateData
    {
        public string TRCA_Thumbprint { get; set; } = "";
        public string TRCA_CNName { get; set; } = "";
        public string TRCA_Expriation { get; set; } = "";

        public bool IsEmpty()
        {
            return (string.IsNullOrEmpty(TRCA_CNName)
                && string.IsNullOrEmpty(TRCA_Expriation))
                || string.IsNullOrEmpty(TRCA_Thumbprint);
        }
    }

    internal class ToolData : IToolInfo
    {
        public string StringId { get; set; } = "";
        public string Name { get; set; } = "";
        public List<ToolVersionData> ToolVersionSource { get; set; }
        public string CurrentInstalledVersion { get; set; } = "";
        public string InstallPath { get; set; } = "";
        public string ExecutePath { get; set; } = "";
        public ToolStatus ToolStatus { get; set; }

        public string LatestVersion => ToolVersionSource?.Last().Version ?? "";

        public string IconSource { get; set; } = "";

        public ToolData()
        {
            ToolVersionSource = new List<ToolVersionData>();
        }
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
        Removed = 2,
        Updateable = 4,
    }

    internal enum ToolVersionStatus
    {
        None = 0,
        VersionDownloadedButWithoutInstalled = 1,
        VersionInstalled = 2,
        VersionInstalledFail = 3,
    }
}
