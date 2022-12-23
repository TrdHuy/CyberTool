using cyber_installer.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.@base.modules
{
    internal interface ISwInstallingManager : ICyberInstallerModule
    {
        public string GetInstallationPath();
        public Task<ToolData?> StartDownloadingLatestVersionToolTask(ToolVO toolVO, Action<object, double> downloadProgressChangedCallback);
        public Task<ToolData?> StartToolInstallingTask(ToolData toolData, string installPath, Action<double>? installProgressChangedCallback = null);
    }
}
