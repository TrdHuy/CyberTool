using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.models.user_data
{
    internal class PluginUD
    {
        public string PluginKey { get; set; } = "";
        public ICollection<PluginVersionUD>? PluginVersionSource { get; set; }
        public string CurrentInstalledVersion { get; set; } = "";
        public string CurrentInstalledVersionPath { get; set; } = "";
        public string CurrentInstalledVersionMainClassName { get; set; } = "";
        public PluginStatus PluginStatus { get; set; }
    }

    internal enum PluginStatus
    {
        Downloaded = 0,
        Installed = 1,
        NeedToRemove = 2
    }
}
