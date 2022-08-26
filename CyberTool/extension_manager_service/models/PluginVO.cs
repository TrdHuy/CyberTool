using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.models
{
    internal class PluginVO
    {
        public PluginVO()
        {
            this.PluginVersions = new HashSet<PluginVersionVO>();
            this.Tags = new HashSet<TagVO>();
            this.Votes = new HashSet<VoteVO>();
        }

        public int PluginId { get; set; }
        public string StringId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Author { get; set; } = "";
        public string Description { get; set; } = "";
        public string ProjectURL { get; set; } = "";
        public string IconSource { get; set; } = "";
        public bool IsAuthenticated { get; set; }
        public int Downloads { get; set; }

        public virtual ICollection<PluginVersionVO> PluginVersions { get; set; }
        public virtual ICollection<VoteVO> Votes { get; set; }
        public virtual ICollection<TagVO> Tags { get; set; }
    }
}
