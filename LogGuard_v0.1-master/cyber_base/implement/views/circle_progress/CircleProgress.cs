using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace cyber_base.implement.views.circle_progress
{
    public class CircleProgress : Control
    {
        private const string MainGridName = "MainGrid";
        private const string ValuePathName = "ValuePath";
        private const string BgPathName = "BgPath";
        private const string FramePathName = "FramePath";

        #region StrokeThickness
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                "StrokeThickness",
                typeof(double),
                typeof(CircleProgress),
                new PropertyMetadata(5d, new PropertyChangedCallback(StrokeThicknessPropertyChangedCallback)));

        private static void StrokeThicknessPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cP = d as CircleProgress;
            cP?.UpdateValuePathUI();
        }


        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        #endregion

        #region Value
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(CircleProgress),
                new PropertyMetadata(0d, new PropertyChangedCallback(ValuePropertyChangedCallback)));

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cP = d as CircleProgress;
            cP?.OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        #endregion

        #region Maximum
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(double),
                typeof(CircleProgress),
                new PropertyMetadata(100d, new PropertyChangedCallback(MaximumPropertyChangedCallback)));

        private static void MaximumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cP = d as CircleProgress;
            cP?.UpdateValuePathUI();
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        #endregion

        public event ValueChangedHandler? CurrentValueChanged;

        private Grid? PART_MainGrid;
        private Path? PART_FramePath;
        private Path? PART_BgPath;
        private Path? PART_ValuePath;

        public CircleProgress()
        {
            this.DefaultStyleKey = typeof(CircleProgress);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_MainGrid = GetTemplateChild(MainGridName) as Grid;
            PART_ValuePath = GetTemplateChild(ValuePathName) as Path;
            PART_FramePath = GetTemplateChild(FramePathName) as Path;
            PART_BgPath = GetTemplateChild(BgPathName) as Path;
            if (PART_MainGrid == null
                || PART_ValuePath == null
                || PART_BgPath == null
                || PART_FramePath == null)
            {
                throw new InvalidOperationException();
            }
            UpdateValuePathUI();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateValuePathUI();
        }

        private void OnValueChanged(double oldVal, double newVal)
        {
            UpdateValuePathUI();
            CurrentValueChanged?.Invoke(this, new ValueChangedEventArgs(oldVal, newVal));
        }

        private void UpdateValuePathUI()
        {
            if (PART_ValuePath != null && PART_BgPath != null && PART_FramePath != null)
            {
                double T = StrokeThickness;
                double R = (ActualHeight >= ActualWidth ? ActualWidth : ActualHeight) / 2;

                if (R == 0 || T == 0)
                {
                    R = 30;
                    T = 5;
                }

                SetCurrentProgressPathData(PART_BgPath
                    , PART_FramePath
                    , PART_ValuePath
                    , Value
                    , Maximum
                    , R, T);
            }
        }


        private void SetCurrentProgressPathData(Path bgPath
           , Path framePath
          , Path mainPath
           , double value
           , double max
           , double R = 30
           , double T = 5)
        {
            double dX_O = R * (1 - Math.Cos(value / max * 2 * Math.PI));
            double dY_O = R * (1 - Math.Sin(value / max * 2 * Math.PI));
            double dX_I = (R - T) * (1 - Math.Cos(value / max * 2 * Math.PI)) + T;
            double dY_I = (R - T) * (1 - Math.Sin(value / max * 2 * Math.PI)) + T;

            string tmp = "M 0,{0} " +
                    "A 1 1 0 0 1 {1},{0} " +
                    "A 1 1 0 0 1 0,{0} " +
                    "L {2} {0} " +
                    "A 1 1 0 0 1 {3},{0} " +
                    "A 1 1 0 0 1 {2},{0} Z";

            var dataBG = string.Format(tmp
           , R
           , 2 * R
           , T
           , 2 * R - T);
            bgPath.Data = Geometry.Parse(dataBG);
            framePath.Data = Geometry.Parse(dataBG);

            if (value <= max / 2)
            {
                tmp = "M 0,{0} " +
                    "A {0} {0} 0 0 1 {1},{2} " +
                    "L {3} {4} " +
                    "A {5} {5} 0 0 0 {6},{0} Z";
                var dataVal = string.Format(tmp
                , R
                , dX_O
                , dY_O
                , dX_I
                , dY_I
                , R - T
                , T);
                mainPath.Data = Geometry.Parse(dataVal);
            }
            else if (value > max / 2)
            {
                tmp = "M 0,{0} " +
                    "A 1 1 0 0 1 {1},{0} " +
                    "A {0} {0} 0 0 1 {2},{3} " +
                    "L {4}, {5} " +
                    "A {6} {6} 0 0 0 {7},{0} " +
                    "A {6} {6} 0 0 0 {8},{0} Z";
                var dataVal = string.Format(tmp
                , R
                , 2 * R
                , dX_O
                , dY_O
                , dX_I
                , dY_I
                , R - T
                , 2 * R - T
                , T);
                mainPath.Data = Geometry.Parse(dataVal);
            }
        }
    }

    public delegate void ValueChangedHandler(object sender, ValueChangedEventArgs args);

    public class ValueChangedEventArgs
    {
        public double OldValue { get; private set; }
        public double NewValue { get; private set; }

        public bool Handled { get; set; }

        public ValueChangedEventArgs(double oldVal, double newVal)
        {
            Handled = false;
            OldValue = oldVal;
            NewValue = newVal;
        }
    }
}
