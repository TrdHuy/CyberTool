using honeyboard_release_service.@base.view_model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace honeyboard_release_service.views.controls.version_history_list
{
    internal class VersionHistoryListViewItem : ListViewItem
    {
        public VersionHistoryListViewItem()
        {
            DataContextChanged -= OnDataContextChanged;
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Debug.WriteLine("OnDataContextChanged");
            //Debug.WriteLine((sender as VersionHistoryListViewItem)?.DataContext.ToString());
            //Debug.WriteLine(e.NewValue?.ToString() ?? "new value null");
            //Debug.WriteLine(e.OldValue?.ToString() ?? "old value null");
            //Debug.WriteLine("=============================");
            var virtualizingContext = e.NewValue as IVirtualizingViewModel;
            virtualizingContext?.OnVirtualizingViewModelLoaded();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
        }
    }
}
