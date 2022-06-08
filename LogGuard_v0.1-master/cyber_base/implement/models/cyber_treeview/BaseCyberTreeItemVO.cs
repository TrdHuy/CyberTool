using cyber_base.implement.command;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.implement.models.cyber_treeview
{
    public class BaseCyberTreeItemVO
    {
        public BaseCommandImpl? AddCmd;
        public BaseCommandImpl? RmCmd;
        public bool IsFirst = false;
        public bool IsLast = false;
        public bool IsSelected = false;
        public object? Parent;
        public ICyberTreeViewItem? Last;
        public ICyberTreeViewItem? First;
        public string Title = "";

        public BaseCyberTreeItemVO(string title)
        {
            Title = title;
        }

    }
}
