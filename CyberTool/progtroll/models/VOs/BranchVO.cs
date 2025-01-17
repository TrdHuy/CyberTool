﻿using cyber_base.implement.models.cyber_treeview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.models.VOs
{
    internal class BranchVO : BaseCyberTreeItemVO
    {
        public string BranchPath { get; set; }
        public bool IsRemote { get; set; } = false;
        public bool IsNode { get; set; } = false;

        public Dictionary<string, VersionUpCommitVO>? CommitMap { get; set; }

        public BranchVO(string path, string title, bool isNode = false, bool isRemote = false)
            : base(title)
        {
            BranchPath = path;
            IsRemote = isRemote;
            IsNode = isNode;

            if (IsNode)
            {
                CommitMap = new Dictionary<string, VersionUpCommitVO>();
            }
        }
    }
}
