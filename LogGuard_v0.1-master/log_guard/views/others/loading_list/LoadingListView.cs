using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace log_guard.views.others.loading_list
{
    public class LoadingListView : ListView
    {
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.RegisterAttached(
            "IsLoading",
            typeof(bool),
            typeof(LoadingListView),
            new FrameworkPropertyMetadata(defaultValue: false,
                flags: FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnIsLoadingChangedCallback)));

        private static void OnIsLoadingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lv = d as LoadingListView;
            lv.OnLoadingChanged();
        }

        public bool IsLoading
        {
            get
            {
                return (bool)GetValue(IsLoadingProperty);
            }
            set
            {
                SetValue(IsLoadingProperty, value);
            }
        }

        public LoadingListView()
        {
            this.DefaultStyleKey = typeof(LoadingListView);
        }

        private Path _watingIcon;
        private Grid _mainGrid;
        private Storyboard _waitingStoryBoard;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _mainGrid = GetTemplateChild("MainGrid") as Grid;
            _watingIcon = GetTemplateChild("WaitingIcon") as Path;
            OnLoadingChanged();
        }
        private void OnLoadingChanged()
        {
            if (!IsInitialized)
                return;
            if (IsLoading)
            {
                _watingIcon.Visibility = Visibility.Visible;
                var animTime = 2000;
                _waitingStoryBoard = new Storyboard();
                DoubleAnimation spinAnim = new DoubleAnimation();
                Storyboard.SetTargetName(spinAnim, "WaitingIconTransform");
                Storyboard.SetTargetProperty(spinAnim, new PropertyPath(RotateTransform.AngleProperty));
                spinAnim.From = 0.0d;
                spinAnim.To = 360.0d;
                spinAnim.Duration = TimeSpan.FromMilliseconds(animTime);
                _waitingStoryBoard.RepeatBehavior = RepeatBehavior.Forever;

                _waitingStoryBoard.Children.Add(spinAnim);
                _waitingStoryBoard.Begin(_mainGrid);
            }
            else
            {
                _watingIcon.Visibility = Visibility.Collapsed;
                if(_waitingStoryBoard != null)
                {
                    _waitingStoryBoard.Stop(_mainGrid);
                }
            }

        }
    }
}
