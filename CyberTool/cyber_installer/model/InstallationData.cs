using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.model
{
    internal class InstallationData
    {
        public string ToolKey { get; set; } = "";
        public string Guid { get; set; } = "";
        public string ToolName { get; set; } = "";
        public string CurrentInstalledVersion { get; set; } = "";
        public string InstallPath { get; set; } = "";
        public string ExecutePath { get; set; } = "";
    }
}
