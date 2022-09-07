using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.models
{
    internal class TagVO
    {
        public string Content { get; set; } = "";
        public int TagId { get; set; }
        public int PluginId { get; set; }

    }
}
