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
        public Task<bool> StartToolDownloadingTask(ToolVO toolVO);
        public Task<bool> StartToolInstallingTask(ToolData toolData);
    }
}
