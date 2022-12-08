using log_guard.views.others.log_watcher._item;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace log_guard.views.others.log_watcher._tree_viewer
{
    public class HanzaTreeViewer : TreeView
    {
        public HanzaTreeViewer()
        {
            this.DefaultStyleKey = typeof(TreeView);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var hzTItem = new HanzaTreeViewItem();
            return hzTItem;
        }
    }

}
