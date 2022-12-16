using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.@base.model
{
    internal interface IToolInfo
    {
        public string Name { get; set; }
        public string LatestVersion { get; }
        public string IconSource { get; set; }
        public string StringId { get; set; }
    }
}
