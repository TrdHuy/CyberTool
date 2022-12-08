using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.model
{
    public class ToolVO
    {
        public ToolVO()
        {
            this.ToolVersions = new List<ToolVersionVO>();
        }

        public int ToolId { get; set; }
        public string StringId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Author { get; set; } = "";
        public string Description { get; set; } = "";
        public string ProjectURL { get; set; } = "";
        public string IconSource { get; set; } = "";
        public bool IsAuthenticated { get; set; }
        public bool IsPreReleased { get; set; }
        public bool IsRequireLatestVersionToRun { get; set; }
        public int Downloads { get; set; }

        public List<ToolVersionVO> ToolVersions { get; set; }
    }
}
