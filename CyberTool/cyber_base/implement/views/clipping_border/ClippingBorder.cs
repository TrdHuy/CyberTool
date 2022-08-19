using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace cyber_base.implement.views.clipping_border
{
    public class ClippingBorder : Border
    {
        #region IsAbsolute
        public static readonly DependencyProperty IsAbsoluteProperty =
            DependencyProperty.Register(
                "IsAbsolute",
                typeof(bool),
                typeof(ClippingBorder),
                new PropertyMetadata(true));

        public bool IsAbsolute
        {
            get { return (bool)GetValue(IsAbsoluteProperty); }
            set { SetValue(IsAbsoluteProperty, value); }
        }
        #endregion
        
        protected override void OnRender(DrawingContext dc)
        {
            OnApplyChildClip();
            base.OnRender(dc);
        }

        public override UIElement Child
        {
            get
            {
                return base.Child;
            }
            set
            {
                if (this.Child != value)
                {
                    if (this.Child != null)
                    {
                        // Restore original clipping
                        this.Child.SetValue(UIElement.ClipProperty, _oldClip);
                    }

                    if (value != null)
                    {
                        _oldClip = value.ReadLocalValue(UIElement.ClipProperty);
                    }
                    else
                    {
                        // If we dont set it to null we could leak a Geometry object
                        _oldClip = null;
                    }

                    base.Child = value;
                }
            }
        }

        protected virtual void OnApplyChildClip()
        {
            UIElement child = this.Child;
            if (child != null)
            {
                if (IsAbsolute)
                {
                    var geo = GenerateGeometry(
                    ActualHeight
                    , ActualWidth
                    , CornerRadius.TopLeft
                    , CornerRadius.TopRight
                    , CornerRadius.BottomRight
                    , CornerRadius.BottomLeft);
                    child.Clip = geo;
                }
                else
                {
                    RectangleGeometry clipRect = new RectangleGeometry();
                    clipRect.RadiusX = _clipRect.RadiusY = Math.Max(0.0, this.CornerRadius.TopLeft - (this.BorderThickness.Left));
                    clipRect.Rect = new Rect(Child.RenderSize);
                    child.Clip = clipRect;
                }
            }
        }

        private RectangleGeometry _clipRect = new RectangleGeometry();
        private object? _oldClip;

        public static Geometry GenerateGeometry(
           double height
           , double width
           , double topLeft
           , double topRight
           , double botRight
           , double botLeft)
        {
            topLeft = RecalCornerRad(height, width, topLeft);
            topRight = RecalCornerRad(height, width, topRight);
            botRight = RecalCornerRad(height, width, botRight);
            botLeft = RecalCornerRad(height, width, botLeft);

            var point_M = new Point(topLeft, 0);
            var point_L1 = new Point(width - topRight, 0);
            var point_A1 = new Point(width, topRight);
            var point_L2 = new Point(width, height - botRight);
            var point_A2 = new Point(width - botRight, height);
            var point_L3 = new Point(botLeft, height);
            var point_A3 = new Point(0, height - botLeft);
            var point_L4 = new Point(0, topLeft);

            var geoStr = "M {0} {1} " +
                   "L {2} {3} " +
                   "A {4} {4} 0 0 1 {5} {6} " +
                   "L {7} {8} " +
                   "A {9} {9} 0 0 1 {10} {11} " +
                   "L {12} {13} " +
                   "A {14} {14} 0 0 1 {15} {16} " +
                   "L {17} {18} " +
                   "A {19} {19} 0 0 1 {20} {21}";

            var dataVal = string.Format(geoStr
                , point_M.X, point_M.Y
                , point_L1.X, point_L1.Y
                , topLeft, point_A1.X, point_A1.Y
                , point_L2.X, point_L2.Y
                , botRight, point_A2.X, point_A2.Y
                , point_L3.X, point_L3.Y
                , botLeft, point_A3.X, point_A3.Y
                , point_L4.X, point_L4.Y
                , topLeft, point_M.X, point_M.Y);
            return Geometry.Parse(dataVal);
        }

        public static double RecalCornerRad(double height, double width, double rad)
        {
            var newRad = rad;
            if (newRad > width / 2)
            {
                newRad = width / 2;
            }
            if (newRad > height / 2)
            {
                newRad = height / 2;
            }
            return newRad;
        }
    }
}
