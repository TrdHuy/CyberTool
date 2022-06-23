using cyber_base.implement.models.cyber_treeview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.models.VOs
{
    internal class BranchVO : BaseCyberTreeItemVO
    {
        public string BranchPath { get; }
        public bool IsRemote { get; } = false;
        public bool IsNode { get; } = false;

        public BranchVO(string path, string title, bool isNode = false, bool isRemote = false) : base(title)
        {
            BranchPath = path;
            IsRemote = isRemote;
            IsNode = isNode;
        }
    }
}
