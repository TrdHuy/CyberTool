using cyber_base.implement.utils;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace cyber_base.implement.views.cyber_treeview
{
    public class CyberTreeViewer : TreeView, ICyberTreeViewElement
    {
        public static readonly DependencyProperty SelectedCyberItemProperty =
            DependencyProperty.Register(
                "SelectedCyberItem",
                typeof(object),
                typeof(CyberTreeViewer),
                new PropertyMetadata(default(object), OnSelectedCyberItemChangedCallback));

        private static void OnSelectedCyberItemChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public object SelectedCyberItem
        {
            get { return GetValue(SelectedCyberItemProperty); }
            set { SetValue(SelectedCyberItemProperty, value); }
        }

        public CyberTreeViewer()
        {
            this.DefaultStyleKey = typeof(TreeView);
            Items.SortDescriptions.Clear();
            Items.SortDescriptions.Add(new SortDescription("IsFolder", ListSortDirection.Descending));
            Items.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue != null
               && !(newValue is ICyberTreeViewObservableCollection<ICyberTreeViewItem>))
            {
                throw new InvalidOperationException("ItemsSource must be inherited from ICyberTreeViewObservableCollection");
            }
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var hzTItem = new CyberTreeViewItem(this);
            return hzTItem;
        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);
            SelectedCyberItem = e.NewValue;
        }

        public void OnChildNotify()
        {
            Items.SortDescriptions.Clear();
            Items.SortDescriptions.Add(new SortDescription("IsFolder", ListSortDirection.Descending));
            Items.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
        }
    }
}
