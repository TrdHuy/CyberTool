using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace progtroll.views.controls.version_history_list
{
    internal class VersionHistoryListView : ListView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new VersionHistoryListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            //Debug.WriteLine("ClearContainerForItemOverride");
            //Debug.WriteLine(item.ToString());
            //Debug.WriteLine((element as VersionHistoryListViewItem)?.DataContext.ToString());
            //Debug.WriteLine("=============================");
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            //Debug.WriteLine("PrepareContainerForItemOverride");
            //Debug.WriteLine(item.ToString());
            //Debug.WriteLine((element as VersionHistoryListViewItem)?.DataContext.ToString());
            //Debug.WriteLine("=============================");
        }
    }
}
