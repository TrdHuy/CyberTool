using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LogGuard_v0._1.LogGuard.Control
{
    public class LogMappingBorder : System.Windows.Controls.Control
    {
        #region ImageSource
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                    "Source",
                    typeof(ImageSource),
                    typeof(LogMappingBorder),
                    new FrameworkPropertyMetadata(
                            null,
                            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                            new PropertyChangedCallback(OnSourceChanged),
                            null),
                    null);

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }


        public ImageSource Source
        {
            get
            {
                return (ImageSource)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }
        #endregion

        public static readonly DependencyProperty MagnifierRadiusProperty =
            DependencyProperty.RegisterAttached(
                    "MagnifierRadius",
                    typeof(double), typeof(LogMappingBorder),
                    new PropertyMetadata(100d, new PropertyChangedCallback(OnMagnifierRadiusPropertyChangedCallback), null));

        private static void OnMagnifierRadiusPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LogWatcher)
            {
                (d as LogWatcher)?.HanldleLogWatcherMagnifierRadiusChanged();
            }
        }

        public static double GetMagnifierRadius(DependencyObject d)
        {
            return (double)d.GetValue(MagnifierRadiusProperty);
        }

        public static void SetMagnifierRadius(DependencyObject d, double value)
        {
            d.SetValue(MagnifierRadiusProperty, value);
        }

        public int SelectedIndex { get; set; } = -1;

        public LogMappingBorder()
        {
            this.DefaultStyleKey = typeof(LogMappingBorder);
        }

        private Ellipse MagnifierCircle { get; set; }
        private VisualBrush MagnifierBrush { get; set; }
        private LogMappingSizeManager LogMappingManager { get; set; }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            MagnifierCircle = GetTemplateChild("MagnifierCircle") as Ellipse;
            MagnifierBrush = MagnifierCircle.Fill as VisualBrush;
        }

        internal void SetLogMappingManager(LogMappingSizeManager lmm)
        {
            LogMappingManager = lmm;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Panel.SetZIndex(this, 1);
            MagnifierCircle.Visibility = Visibility.Visible;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Panel.SetZIndex(this, 0);
            MagnifierCircle.Visibility = Visibility.Hidden;
        }

        private double _factor = 0.15;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point center = e.GetPosition(this);
            double length = MagnifierCircle.ActualWidth * _factor;
            double radius = length / 2;
            Rect viewboxRect = new Rect(ActualWidth / 2 - radius - (BorderThickness.Left + BorderThickness.Right) / 2, center.Y - radius, length, length);
            MagnifierBrush.Viewbox = viewboxRect;

            MagnifierCircle.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Point center = e.GetPosition(this);
            SelectedIndex = (int)Math.Round(center.Y / LogMappingManager.PointHeight, MidpointRounding.AwayFromZero) - 1
                + LogMappingManager.CurrentStartIndex;

        }

    }
}
