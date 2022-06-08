using cyber_base.implement.models.cyber_treeview;
using cyber_base.implement.view_models.cyber_treeview;
using cyber_base.implement.views.cyber_treeview;
using honeyboard_release_service.models.VOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models.project_manager.items
{
    internal class BranchItemViewModel : BaseCyberTreeItemViewModel
    {
        private BranchVO _newVO;

        public bool IsRemoteBranch { get; set; }

        public BranchItemViewModel(string path, string title, bool isNode = false, bool isRemote = false)
            : base(new BranchVO(path, title, isNode, isRemote))
        {
            _newVO = (BranchVO)_vo;
            IsSelectable = isNode;
        }

        public override string ToString()
        {
            return _newVO.BranchPath;
        }
    }
}
