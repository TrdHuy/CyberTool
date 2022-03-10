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
    [TemplatePart(Name = HanzaTreeViewItem.RootBorderName, Type = typeof(Border))]
    public class HanzaTreeViewItem : ListViewItem
    {
        private class HanzaBtnCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;
            public Action<object> action;

            public HanzaBtnCommand(Action<object> act)
            {
                action = act;
            }
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                action?.Invoke(parameter);
            }
        }

        private const string RootBorderName = "PART_MainBorder";
        private const string AddBtnName = "PART_addBtn";
        private const string ContenLstName = "PART_ContentLst";

        public HanzaTreeViewItem()
        {
            this.DefaultStyleKey = typeof(HanzaTreeViewItem);
        }

        #region IsOpen
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                "IsOpen",
                typeof(bool),
                typeof(HanzaTreeViewItem),
                new PropertyMetadata(false, new PropertyChangedCallback(IsOpenPropertyChangedCallback)));

        private static void IsOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HanzaTreeViewItem hz = d as HanzaTreeViewItem;
            hz.OpenHanzaAnimation();
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        #endregion

        #region Source
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                "Source",
                typeof(IEnumerable),
                typeof(HanzaTreeViewItem),
                new PropertyMetadata(default(IEnumerable), new PropertyChangedCallback(SourcePropertyChangedCallback)));

        private static void SourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hz = d as HanzaTreeViewItem;
            hz.OnSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private void OnSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            var oldNoti = oldValue as INotifyCollectionChanged;
            if (oldNoti != null)
            {
                oldNoti.CollectionChanged -= HandleHanzaViewItemCollectionChanged;
            }

            var newNoti = newValue as INotifyCollectionChanged;
            if (newNoti != null)
            {
                newNoti.CollectionChanged += HandleHanzaViewItemCollectionChanged;
            }
            Type sourceElmType = newValue.GetType().GetGenericArguments()[0];

            // Dynamic create a generic source cache
            MethodInfo method = typeof(Enumerable).GetMethod("OfType");
            MethodInfo generic = method.MakeGenericMethod(new Type[] { sourceElmType });
            SourceCache = (IEnumerable<object>)generic.Invoke
                  (null, new object[] { newValue });

            UpdateVisibilityOfExpandBtn();
            UpdateVisibilityOfAdd_SubBtn();
        }

        private void HandleHanzaViewItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateVisibilityOfExpandBtn();
            UpdateVisibilityOfAdd_SubBtn();
        }

        public IEnumerable Source
        {
            get { return (IEnumerable)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        #endregion

        #region ContentMaxHeight
        public static readonly DependencyProperty ContentMaxHeightProperty =
            DependencyProperty.Register(
                "ContentMaxHeight",
                typeof(double),
                typeof(HanzaTreeViewItem),
                new PropertyMetadata(300d, new PropertyChangedCallback(ContentMaxHeightPropertyChangedCallback)));

        private static void ContentMaxHeightPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rPB = d as HanzaTreeViewItem;
        }

        public double ContentMaxHeight
        {
            get { return (double)GetValue(ContentMaxHeightProperty); }
            set { SetValue(ContentMaxHeightProperty, value); }
        }
        #endregion

        private bool IsAppliedTemplate
        {
            get
            {
                if (MainContent == null || ExpandButton == null)
                    return false;
                //return isAppliedTemplate
                //    && MainContent.IsAppliedTemplate;
                return isAppliedTemplate;
            }
        }
        private bool isAppliedTemplate = false;
        private Border MainBorder;

        private Button AddBtn;
        private Button SubBtn;
        private CCControl MainContent;
        private Button ExpandButton;
        private HanzaTreeViewer ContentLstView;
        private IEnumerable<object> SourceCache;
        private HanzaTreeViewItem Parent;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            MainContent = GetTemplateChild("PART_MainContent") as CCControl;
            ExpandButton = GetTemplateChild("PART_expandBtn") as Button;

            MainBorder = GetTemplateChild(RootBorderName) as Border;

            AddBtn = GetTemplateChild(AddBtnName) as Button;
            SubBtn = GetTemplateChild("PART_subBtn") as Button;
            ContentLstView = GetTemplateChild(ContenLstName) as HanzaTreeViewer;


            if (MainBorder == null
                || AddBtn == null
                || ContentLstView == null
                || MainContent == null)
            {
                throw new InvalidOperationException();
            }
            MouseBinding OpenCmdMouseBinding = new MouseBinding();
            OpenCmdMouseBinding.MouseAction = MouseAction.LeftClick;
            OpenCmdMouseBinding.Command = new HanzaBtnCommand((param) =>
            {
                if (DataContext is IHanzaTreeViewItem)
                    IsOpen = !IsOpen;
            });

            MainContent.ApplyTemplate += (sender) =>
            {
            };

            ExpandButton.Click += (sender, arg) =>
            {
                if (DataContext is IHanzaTreeViewItem)
                    IsOpen = !IsOpen;
            };

            isAppliedTemplate = true;

            UpdateContLstVisibility();

            MainContent.InputBindings.Add(OpenCmdMouseBinding);

            ContentLstView.Parent = this;
            ContentLstView.ItemSourceCauseSizeChanged += ContentLstView_ItemSourceCauseSizeChanged;
            UpdateVisibilityOfAdd_SubBtn(true);
            UpdateVisibilityOfExpandBtn(true);

        }

        public void SetHanzaItemParent(HanzaTreeViewItem parent)
        {
            Parent = parent;
        }

        private void ContentLstView_ItemSourceCauseSizeChanged(object sender, SizeChangedEventArgs arg)
        {
            if (IsOpen)
            {
                AddHanzaAnimation(arg.PreviousSize.Height, arg.NewSize.Height);
            }
        }

        private void UpdateVisibilityOfExpandBtn(bool force = false)
        {
            if (!IsAppliedTemplate && !force) return;

            if (SourceCache == null || SourceCache.Count() == 0)
            {
                ExpandButton.Visibility = Visibility.Hidden;
            }
            else
            {
                ExpandButton.Visibility = Visibility.Visible;
            }
        }

        private void UpdateVisibilityOfAdd_SubBtn(bool force = false)
        {
            if (!IsAppliedTemplate && !force) return;

            if (!(DataContext is IHanzaTreeViewItem))
            {
                AddBtn.Visibility = Visibility.Collapsed;
                SubBtn.Visibility = Visibility.Collapsed;
                return;
            }

            if (SourceCache == null || SourceCache.Count() == 0)
            {
                AddBtn.Visibility = Visibility.Visible;
                SubBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                AddBtn.Visibility = Visibility.Visible;
                SubBtn.Visibility = Visibility.Visible;
            }
        }

        private void OpenHanzaAnimation()
        {
            if (!IsAppliedTemplate) return;

            var newHeight = ContentLstView.GetItemPresenterHeight();

            if (IsOpen)
            {
                ContentLstView.Visibility = Visibility.Visible;
            }

            if (Parent != null)
            {

                Parent.OpenSubHanzaAnimation(
                    Parent.ContentLstView.GetItemPresenterHeight(),
                    IsOpen ? Parent.ContentLstView.GetItemPresenterHeight() + newHeight
                    : Parent.ContentLstView.GetItemPresenterHeight() - newHeight);
            }

            Storyboard feedbackSB_Btn = new Storyboard();
            Storyboard feedbackSB_HanzaList = new Storyboard();

            long anmDuration = 100;

            DoubleAnimation rotateAnimation = new DoubleAnimation();

            rotateAnimation.From = IsOpen ? 0 : 90;
            rotateAnimation.To = IsOpen ? 90 : 0;
            rotateAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(anmDuration));
            Storyboard.SetTargetName(rotateAnimation, "PART_expandBtnRotateTransform");
            Storyboard.SetTargetProperty(rotateAnimation,
                new PropertyPath(RotateTransform.AngleProperty));
            feedbackSB_Btn.Children.Add(rotateAnimation);

            DoubleAnimation heightAnimation = new DoubleAnimation();

            //heightAnimation.From = IsOpen ? 0 : (newHeight > ContentMaxHeight ? ContentMaxHeight : newHeight);
            //heightAnimation.To = IsOpen ? (newHeight > ContentMaxHeight ? ContentMaxHeight : newHeight) : 0;
            heightAnimation.From = IsOpen ? 0 : newHeight;
            heightAnimation.To = IsOpen ? newHeight : 0;

            heightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(anmDuration));
            Storyboard.SetTargetName(heightAnimation, ContentLstView.Name);
            Storyboard.SetTargetProperty(heightAnimation,
                new PropertyPath(ListView.HeightProperty));
            feedbackSB_HanzaList.Children.Add(heightAnimation);

            feedbackSB_HanzaList.Completed += (sender, e) =>
            {
                UpdateContLstVisibility();
            };

            feedbackSB_HanzaList.Begin(MainBorder);
            feedbackSB_Btn.Begin(ExpandButton);


        }

        private void OpenSubHanzaAnimation(double from, double to)
        {
            if (!IsOpen || !IsAppliedTemplate) return;

            Storyboard feedbackSB = new Storyboard();
            long anmDuration = 100;

            DoubleAnimation heightAnimation = new DoubleAnimation();

            heightAnimation.From = from;
            heightAnimation.To = to;

            heightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(anmDuration));
            Storyboard.SetTargetName(heightAnimation, ContentLstView.Name);
            Storyboard.SetTargetProperty(heightAnimation,
                new PropertyPath(ListView.HeightProperty));
            feedbackSB.Children.Add(heightAnimation);

            feedbackSB.Begin(MainBorder);
        }

        private void AddHanzaAnimation(double from, double to)
        {
            if (!IsOpen || !IsAppliedTemplate) return;

            Storyboard feedbackSB = new Storyboard();
            long anmDuration = 100;

            DoubleAnimation heightAnimation = new DoubleAnimation();

            heightAnimation.From = from;
            heightAnimation.To = to;

            heightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(anmDuration));
            Storyboard.SetTargetName(heightAnimation, ContentLstView.Name);
            Storyboard.SetTargetProperty(heightAnimation,
                new PropertyPath(ListView.HeightProperty));
            feedbackSB.Children.Add(heightAnimation);

            feedbackSB.Begin(MainBorder);
        }

        private void UpdateContLstVisibility()
        {
            ContentLstView.Visibility = IsOpen ? Visibility.Visible : Visibility.Collapsed;
        }

    }


}
