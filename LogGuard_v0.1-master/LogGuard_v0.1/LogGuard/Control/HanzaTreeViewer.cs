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

namespace LogGuard_v0._1.LogGuard.Control
{
    public class HanzaTreeViewer : ListView
    {
        public event ItemSourceCauseSizeChangedHandler ItemSourceCauseSizeChanged;
        public HanzaTreeViewItem Parent { get; set; }

        public HanzaTreeViewer()
        {
            this.DefaultStyleKey = typeof(HanzaTreeViewer);
        }

        private ItemsPresenter _itemsPresenter;
        private bool _isItemSourceChangeCauseSizeChanged = false;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _itemsPresenter = GetTemplateChild("PART_ItemPresenter") as ItemsPresenter;

            if (_itemsPresenter == null)
            {
                throw new InvalidOperationException();
            }

            _itemsPresenter.SizeChanged += _itemsPresenter_SizeChanged;
            LostFocus += HanzaTreeViewer_LostFocus; ;
        }


        private void HanzaTreeViewer_LostFocus(object sender, RoutedEventArgs e)
        {
            UnselectAll();
        }

        private void _itemsPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isItemSourceChangeCauseSizeChanged)
            {
                ItemSourceCauseSizeChanged?.Invoke(this, e);
                _isItemSourceChangeCauseSizeChanged = false;
            }


        }
        
        public double GetItemPresenterHeight()
        {
            return _itemsPresenter == null ? 0 : _itemsPresenter.ActualHeight;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (item is IHanzaTreeViewItem)
            {
                var hzItem = element as HanzaTreeViewItem;
                var hzItemImpl = item as IHanzaTreeViewItem;

                if (hzItemImpl != null && hzItem != null)
                {
                    Binding b = new Binding();
                    b.Source = hzItemImpl;
                    b.Path = new PropertyPath("Childs");
                    b.Mode = BindingMode.TwoWay;
                    b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    hzItem.SetBinding(HanzaTreeViewItem.SourceProperty, b);

                    hzItem.DataContext = hzItemImpl;

                    if (Parent != null)
                    {
                        hzItem.SetHanzaItemParent(Parent);
                    }
                }
            }
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new HanzaTreeViewItem();
        }
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (IsLoaded)
            {
                _isItemSourceChangeCauseSizeChanged = true;
            }
        }
    }

    public delegate void ItemSourceCauseSizeChangedHandler(object sender, SizeChangedEventArgs arg);

    public interface IHanzaTreeViewItem : INotifyPropertyChanged
    {
        IEnumerable Childs { get; set; }

        String Title { get; set; }

    }
}
