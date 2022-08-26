using cyber_base.implement.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.views.elements.plugin_browser.items.@base
{
    internal interface IVersionHistoryItemViewHolderContext : IFirstLastElement
    {
        bool IsInstalled { get; set; }
        string Version { get; set; }
        string Description { get; set; }
    }
}
