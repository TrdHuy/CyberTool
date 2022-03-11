using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private const string ExpandBtnName = "Expander";
        public HanzaTreeViewItem()
        {
            this.DefaultStyleKey = typeof(TreeViewItem);
        }

        private Button _addButton;
        private Button _removeButton;
        private ToggleButton _expandButton;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _addButton = GetTemplateChild(AddBtnName) as Button;
            _removeButton = GetTemplateChild(RemoveBtnName) as Button;
            _expandButton = GetTemplateChild(ExpandBtnName) as ToggleButton;
            var animTime = 100;
            var mainGrid = GetTemplateChild("MainGrid") as Grid;
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

            _expandButton.Checked += (s,e) =>
            {
                Storyboard expandSb = new Storyboard();

                DoubleAnimation rotateAnim = new DoubleAnimation();
                rotateAnim.From = 0.0;
                rotateAnim.To = 90.0;
                rotateAnim.Duration = new Duration(TimeSpan.FromMilliseconds(animTime));
                Storyboard.SetTargetName(rotateAnim, "PART_ToggleButtonRotateTransform");
                Storyboard.SetTargetProperty(rotateAnim, new PropertyPath(RotateTransform.AngleProperty));
                expandSb.Children.Add(rotateAnim);

                ObjectAnimationUsingKeyFrames oAUKF = new ObjectAnimationUsingKeyFrames();
                DiscreteObjectKeyFrame dOKF = new DiscreteObjectKeyFrame();
                dOKF.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));
                dOKF.Value = Visibility.Visible;
                oAUKF.KeyFrames.Add(dOKF);
                Storyboard.SetTargetName(oAUKF, "ItemsHost");
                Storyboard.SetTargetProperty(oAUKF, new PropertyPath(UIElement.VisibilityProperty));
                expandSb.Children.Add(oAUKF);

                DoubleAnimation openItemHostAnim = new DoubleAnimation();
                openItemHostAnim.From = 0.0;
                openItemHostAnim.To = 1.0;
                openItemHostAnim.Duration = new Duration(TimeSpan.FromMilliseconds(animTime));
                Storyboard.SetTargetName(openItemHostAnim, "PART_ItemHostScaleTransform");
                Storyboard.SetTargetProperty(openItemHostAnim, new PropertyPath(ScaleTransform.ScaleYProperty));
                expandSb.Children.Add(openItemHostAnim);

                expandSb.Begin(mainGrid);
            };
            _expandButton.Unchecked += (s, e) =>
            {
                Storyboard dispandSB = new Storyboard();

                DoubleAnimation rotateAnim = new DoubleAnimation();
                rotateAnim.From = 90.0;
                rotateAnim.To = 0.0;
                rotateAnim.Duration = new Duration(TimeSpan.FromMilliseconds(animTime));
                Storyboard.SetTargetName(rotateAnim, "PART_ToggleButtonRotateTransform");
                Storyboard.SetTargetProperty(rotateAnim, new PropertyPath(RotateTransform.AngleProperty));
                dispandSB.Children.Add(rotateAnim);

                ObjectAnimationUsingKeyFrames oAUKF = new ObjectAnimationUsingKeyFrames();
                DiscreteObjectKeyFrame dOKF = new DiscreteObjectKeyFrame();
                dOKF.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animTime));
                dOKF.Value = Visibility.Collapsed;
                oAUKF.KeyFrames.Add(dOKF);
                Storyboard.SetTargetName(oAUKF, "ItemsHost");
                Storyboard.SetTargetProperty(oAUKF, new PropertyPath(UIElement.VisibilityProperty));
                dispandSB.Children.Add(oAUKF);

                DoubleAnimation openItemHostAnim = new DoubleAnimation();
                openItemHostAnim.From = 1.0;
                openItemHostAnim.To = 0.0;
                openItemHostAnim.Duration = new Duration(TimeSpan.FromMilliseconds(animTime));
                Storyboard.SetTargetName(openItemHostAnim, "PART_ItemHostScaleTransform");
                Storyboard.SetTargetProperty(openItemHostAnim, new PropertyPath(ScaleTransform.ScaleYProperty));
                dispandSB.Children.Add(openItemHostAnim);

                dispandSB.Begin(mainGrid);
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
