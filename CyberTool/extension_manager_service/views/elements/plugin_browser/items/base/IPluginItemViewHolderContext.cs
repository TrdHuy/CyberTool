using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.views.elements.plugin_browser.items.@base
{
    internal interface IPluginItemViewHolderContext
    {
        public string PluginName { get; set; }
        public string PluginAuthor { get; set; }
        public string PluginDescription { get; set; }
        public bool IsAuthenticated { get; set; }
        public Uri IconSource { get; set; }
        public bool IsInstalled { get; set; }
        public int Downloads { get; set; }
        public double Rates { get; set; }
        public PluginItemViewMode ViewMode { get; set; }
    }
}
