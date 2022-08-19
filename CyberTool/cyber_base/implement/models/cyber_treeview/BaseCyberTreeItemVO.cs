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
        public string Title { get; set; } = "";

        public BaseCyberTreeItemVO(string title)
        {
            Title = title;
        }

    }
}
