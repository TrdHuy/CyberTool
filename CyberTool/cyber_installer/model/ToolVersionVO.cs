using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.model
{
    internal class ToolVersionVO
    {
        public int VersionId { get; set; }
        public string Version { get; set; } = "";
        public string Description { get; set; } = "";
        public int ToolId { get; set; }
        public System.DateTime DatePublished { get; set; }
        public string FolderPath { get; set; } = "";
        public string FileName { get; set; } = "";
        public string ExecutePath { get; set; } = "";
    }
}
