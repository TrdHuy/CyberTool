using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.models
{
    internal class PluginVersionVO
    {
        public int VersionId { get; set; }
        public string Version { get; set; } = "";
        public string Description { get; set; } = "";
        public int PluginId { get; set; }
        public System.DateTime DatePublished { get; set; }
        public string FilePath { get; set; } = "";

    }
}
