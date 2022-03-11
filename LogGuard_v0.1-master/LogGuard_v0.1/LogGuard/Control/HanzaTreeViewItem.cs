using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LogGuard_v0._1.LogGuard.Control
{
    public class HanzaTreeViewItem : TreeViewItem
    {
        private const string AddBtnName = "PART_addBtn";
        private const string RemoveBtnName = "PART_removeBtn";
        public HanzaTreeViewItem()
        {
            this.DefaultStyleKey = typeof(TreeViewItem);
        }

        private Button _addButton;
        private Button _removeButton;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _addButton = GetTemplateChild(AddBtnName) as Button;
            _removeButton = GetTemplateChild(RemoveBtnName) as Button;

            if (_addButton == null || _removeButton == null)
            {
                throw new InvalidOperationException("Some view elements not found!");
            }

            //if (DataContext is IHanzaTreeViewItem)
            //{
            //    var source = DataContext as IHanzaTreeViewItem;
            //    Binding b = new Binding();
            //    b.Source = source;
            //    b.Path = new PropertyPath("AddBtnCommand");
            //    b.Mode = BindingMode.OneWay;
            //    b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //    _addButton.SetBinding(Button.CommandProperty, b);

            //    b = new Binding();
            //    b.Source = source;
            //    b.Path = new PropertyPath("RemoveBtnCommand");
            //    b.Mode = BindingMode.OneWay;
            //    b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //    _removeButton.SetBinding(Button.CommandProperty, b);
            //}

            var source = DataContext as IHanzaTreeViewItem;
            _addButton.Click += (s, e) =>
            {
                var parma = new object[2];
                parma[0] = this;
                parma[1] = e;
                source?.AddBtnCommand?.Execute(parma);
            };

            _removeButton.Click += (s, e) =>
            {
                var parma = new object[2];
                parma[0] = this;
                parma[1] = e;
                source?.RemoveBtnCommand?.Execute(parma);
            };
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var hzTItem = new HanzaTreeViewItem();
            return hzTItem;
        }
    }

    public interface IHanzaTreeViewItem
    {
        ICommand AddBtnCommand { get; }

        ICommand RemoveBtnCommand { get; }
    }

}
