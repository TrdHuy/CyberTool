using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace cyber_base.implement.views.cyber_anim
{
    public class LoadingAnimation : Control
    {
        private StackPanel? _mainPanel;
        private Storyboard? _animationCache;

        protected const string MainPanelName = "PART_MainPanel";

        #region IsBusy
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy",
                typeof(bool),
                typeof(LoadingAnimation),
                new FrameworkPropertyMetadata(false,
                                        FrameworkPropertyMetadataOptions.AffectsMeasure |
                                        FrameworkPropertyMetadataOptions.AffectsRender,
                                        new PropertyChangedCallback(IsBusyChangedCallback)));

        private static void IsBusyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = d as LoadingAnimation;
            s?.OnBusyChanged((bool)e.NewValue);
        }


        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
        #endregion

        #region ElipseFill
        public static readonly DependencyProperty ElipseFillProperty =
            DependencyProperty.Register("ElipseFill",
                typeof(Brush),
                typeof(LoadingAnimation),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Aqua),
                                        FrameworkPropertyMetadataOptions.AffectsMeasure |
                                        FrameworkPropertyMetadataOptions.AffectsRender), null);

        public Brush ElipseFill
        {
            get { return (Brush)GetValue(ElipseFillProperty); }
            set { SetValue(ElipseFillProperty, value); }
        }
        #endregion

        #region ElipNumber
        public static readonly DependencyProperty ElipNumberProperty =
            DependencyProperty.Register("ElipNumber",
                typeof(int),
                typeof(LoadingAnimation),
                new FrameworkPropertyMetadata(3,
                                        FrameworkPropertyMetadataOptions.AffectsMeasure |
                                        FrameworkPropertyMetadataOptions.AffectsRender), null);

        public int ElipNumber
        {
            get { return (int)GetValue(ElipNumberProperty); }
            set { SetValue(ElipNumberProperty, value); }
        }
        #endregion

        #region AnimationTime
        public static readonly DependencyProperty AnimationTimeProperty =
            DependencyProperty.Register("AnimationTime",
                typeof(double),
                typeof(LoadingAnimation),
                new FrameworkPropertyMetadata(2.0,
                                        FrameworkPropertyMetadataOptions.AffectsMeasure |
                                        FrameworkPropertyMetadataOptions.AffectsRender), null);

        public double AnimationTime
        {
            get { return (double)GetValue(AnimationTimeProperty); }
            set { SetValue(AnimationTimeProperty, value); }
        }
        #endregion

        #region ElipseMarginGap
        public static readonly DependencyProperty ElipseMarginGapProperty =
            DependencyProperty.Register("ElipseMarginGap",
                typeof(Thickness),
                typeof(LoadingAnimation),
                new FrameworkPropertyMetadata(new Thickness(5, 0, 5, 0),
                                        FrameworkPropertyMetadataOptions.AffectsMeasure |
                                        FrameworkPropertyMetadataOptions.AffectsRender), null);

        public Thickness ElipseMarginGap
        {
            get { return (Thickness)GetValue(ElipseMarginGapProperty); }
            set { SetValue(ElipseMarginGapProperty, value); }
        }

        #endregion

        public override void OnApplyTemplate()
        {
            _mainPanel = GetTemplateChild(MainPanelName) as StackPanel;
            if (_mainPanel != null)
            {
                InitLoadingAnimation();
            }
            OnBusyChanged(IsBusy);

        }

        private void OnBusyChanged(bool busy)
        {
            if (!busy)
            {
                _animationCache?.Stop(this);
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                _animationCache?.Begin(this, true);
                this.Visibility = Visibility.Visible;

            }

        }

        private void InitLoadingAnimation()
        {
            if (_mainPanel == null)
            {
                return;
            }
            var defaultElipseName = "ElipseVO_";
            var defaultTransformElipseName = "TransElipseVO_";
            _mainPanel.Children.Clear();
            var fromScale = 0.5;
            var toScale = 1.0;
            var animTimePerElip = AnimationTime / (ElipNumber + 1);
            var currentTime = 0.0d;

            NameScope.SetNameScope(this, new NameScope());

            _animationCache = new Storyboard();
            _animationCache.RepeatBehavior = RepeatBehavior.Forever;

            for (int i = 0; i < ElipNumber; i++)
            {
                Ellipse e = new Ellipse();
                e.Width = 30;
                e.Height = 30;
                e.RenderTransformOrigin = new Point(0.5, 0.5);
                e.Name = defaultElipseName + i;
                this.RegisterName(e.Name, e);

                Binding fillBinding = new Binding();
                fillBinding.Source = this;
                fillBinding.Path = new PropertyPath("ElipseFill");
                fillBinding.Mode = BindingMode.TwoWay;
                fillBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                e.SetBinding(Ellipse.FillProperty, fillBinding);

                Binding marginBinding = new Binding();
                marginBinding.Source = this;
                marginBinding.Path = new PropertyPath("ElipseMarginGap");
                marginBinding.Mode = BindingMode.TwoWay;
                marginBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                e.SetBinding(Ellipse.MarginProperty, marginBinding);

                var scaleTransform = new ScaleTransform();
                scaleTransform.ScaleX = 0.5;
                scaleTransform.ScaleY = 0.5;
                this.RegisterName(defaultTransformElipseName + i, scaleTransform);
                e.RenderTransform = scaleTransform;

                //===============================
                DoubleAnimation scaleXAnimation = new DoubleAnimation()
                {
                    From = fromScale,
                    To = toScale,
                    AutoReverse = false,
                    BeginTime = TimeSpan.FromSeconds(currentTime)
                };
                scaleXAnimation.Duration = new Duration(TimeSpan.FromSeconds(animTimePerElip));
                Storyboard.SetTargetName(scaleXAnimation, (defaultTransformElipseName + i));
                Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

                DoubleAnimation scaleYAnimation = new DoubleAnimation()
                {
                    From = fromScale,
                    To = toScale,
                    AutoReverse = false,
                    BeginTime = TimeSpan.FromSeconds(currentTime)
                };
                scaleYAnimation.Duration = new Duration(TimeSpan.FromSeconds(animTimePerElip));
                Storyboard.SetTargetName(scaleYAnimation, (defaultTransformElipseName + i));
                Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

                //===============================
                // Increase current time
                currentTime += animTimePerElip;

                //===============================
                DoubleAnimation scaleXAnimation2 = new DoubleAnimation()
                {
                    From = toScale,
                    To = fromScale,
                    AutoReverse = false,
                    BeginTime = TimeSpan.FromSeconds(currentTime)
                };
                scaleXAnimation2.Duration = new Duration(TimeSpan.FromSeconds(animTimePerElip));
                Storyboard.SetTargetName(scaleXAnimation2, (defaultTransformElipseName + i));
                Storyboard.SetTargetProperty(scaleXAnimation2, new PropertyPath(ScaleTransform.ScaleXProperty));

                DoubleAnimation scaleYAnimation2 = new DoubleAnimation()
                {
                    From = toScale,
                    To = fromScale,
                    AutoReverse = false,
                    BeginTime = TimeSpan.FromSeconds(currentTime)
                };
                scaleYAnimation2.Duration = new Duration(TimeSpan.FromSeconds(animTimePerElip));
                Storyboard.SetTargetName(scaleYAnimation2, (defaultTransformElipseName + i));
                Storyboard.SetTargetProperty(scaleYAnimation2, new PropertyPath(ScaleTransform.ScaleYProperty));

                _animationCache.Children.Add(scaleXAnimation);
                _animationCache.Children.Add(scaleYAnimation);
                _animationCache.Children.Add(scaleXAnimation2);
                _animationCache.Children.Add(scaleYAnimation2);

                _mainPanel.Children.Add(e);
            }
        }
    }
}
