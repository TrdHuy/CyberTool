using cyber_base.implement.command;
using cyber_base.implement.utils;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace cyber_base.implement.views.cyber_treeview
{
    public class CyberTreeViewItem : TreeViewItem, ICyberTreeViewElement
    {
        private const string ExpandBtnName = "Expander";
        private ICyberTreeViewItemContext? _castContext;
        private ICyberTreeViewElement _parents;

        public CyberTreeViewItem(ICyberTreeViewElement parents)
        {
            this.DefaultStyleKey = typeof(TreeViewItem);
            _parents = parents;
            Items.SortDescriptions.Clear();
            Items.SortDescriptions.Add(new SortDescription("IsFolder", ListSortDirection.Descending));
            Items.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));

            var noti = Items as INotifyCollectionChanged;
            if (noti != null)
            {
                noti.CollectionChanged -= HandleFirstLastElement;
                noti.CollectionChanged += HandleFirstLastElement;
            }

            DataContextChanged -= CyberTreeViewItem_DataContextChanged;
            DataContextChanged += CyberTreeViewItem_DataContextChanged;

        }

        private void CyberTreeViewItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _castContext = e.NewValue as ICyberTreeViewItemContext;
        }

        private void HandleFirstLastElement(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_castContext == null) return;

            if (Items.Count > 0)
            {
                var fi = Items.GetItemAt(0) as ICyberTreeViewItemContext;
                var li = Items.GetItemAt(Items.Count - 1) as ICyberTreeViewItemContext;

                if (fi != null)
                {
                    if (_castContext.First != null)
                    {
                        _castContext.First.IsFirst = false;
                    }
                    fi.IsFirst = true;
                    _castContext.First = fi;
                }
                if (li != null)
                {
                    if (_castContext.Last != null)
                    {
                        _castContext.Last.IsLast = false;
                    }
                    li.IsLast = true;
                    _castContext.Last = li;
                }
            }
        }

        private ToggleButton? _expandButton;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _expandButton = GetTemplateChild(ExpandBtnName) as ToggleButton;
            var animTime = 100;
            var mainGrid = GetTemplateChild("MainGrid") as Grid;
            if (_expandButton == null)
            {
                throw new InvalidOperationException("Some view elements not found!");
            }

            var source = DataContext as ICyberTreeViewItemContext;

            _expandButton.Checked += (s, e) =>
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

        public void OnChildNotify()
        {
            Items.SortDescriptions.Clear();
            Items.SortDescriptions.Add(new SortDescription("IsFolder", ListSortDirection.Descending));
            Items.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
            _parents.OnChildNotify();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var hzTItem = new CyberTreeViewItem(this);
            return hzTItem;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue != null
               && !(newValue is ICyberTreeViewObservableCollection<ICyberTreeViewItemContext>))
            {
                throw new InvalidOperationException("ItemsSource must be inherited from ICyberTreeViewObservableCollection");
            }

            base.OnItemsSourceChanged(oldValue, newValue);

            var noti = newValue as INotifyCollectionChanged;
            var oldNoti = oldValue as INotifyCollectionChanged;

            if (noti != null)
            {
                noti.CollectionChanged -= OnItemsSourceCollectionChanged;
                noti.CollectionChanged += OnItemsSourceCollectionChanged;
            }

            if (oldNoti != null)
            {
                oldNoti.CollectionChanged -= OnItemsSourceCollectionChanged;
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //Notify parents
            _parents?.OnChildNotify();
        }
    }

    public interface ICyberTreeViewElement
    {
        void OnChildNotify();
    }

    public interface IFirstLastElement
    {
        /// <summary>
        /// Kiểm tra phần tử hiện tại có là phần tử đầu tiên trong tập hợp hay không
        /// </summary>
        bool IsFirst { get; set; }

        /// <summary>
        /// Kiểm tra phần tử hiện tại có là phần tử cuối cùng trong tập hợp hay không
        /// </summary>
        bool IsLast { get; set; }
    }

    public interface ICyberTreeViewItemContext : IFirstLastElement
    {
        /// <summary>
        /// Tiêu đề hiển thị của item trên treeview
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Đường dẫn tuyệt đối đến item 
        /// </summary>
        string AbsoluteTitle { get; }

        /// <summary>
        /// Số lượng phần tử có trong 1 item
        /// </summary>
        int ItemsCount { get; }

        /// <summary>
        /// Kiểm tra item hiện tại có thuộc bất kỳ item nào khác không
        /// </summary>
        bool IsOrphaned { get; }

        /// <summary>
        /// Kiểm tra item hiện tại có đang được chọn hay không
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Kiểm tra item hiện tại có thể chọn được hay không
        /// </summary>
        bool IsSelectable { get; set; }

        /// <summary>
        /// Biến lưu đối tượng cha của item
        /// </summary>
        ICyberTreeViewItemContext? Parent { get; }

        /// <summary>
        /// Biến lưu đối tượng cuối trong tập phần tử con của item hiện tại
        /// </summary>
        ICyberTreeViewItemContext? Last { get; set; }

        /// <summary>
        /// Biến lưu đối tượng đầu trong tập phần tử con của item hiện tại
        /// </summary>
        ICyberTreeViewItemContext? First { get; set; }

        /// <summary>
        /// Kiểm tra item hiện tại có phải là 1 folder không
        /// </summary>
        bool IsFolder { get; }

        /// <summary>
        /// Tập hợp các phần tử con của item hiện tại
        /// </summary>
        CyberTreeViewObservableCollection<ICyberTreeViewItemContext> Items { get; set; }

    }

}
