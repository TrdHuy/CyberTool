using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LogGuard_v0._1.LogGuard.Control
{
    [TemplatePart(Name = RadialProgressBar.RootBorderName, Type = typeof(Border))]
    [TemplatePart(Name = RadialProgressBar.MainGridName, Type = typeof(Grid))]
    [TemplatePart(Name = RadialProgressBar.BackgroundPathName, Type = typeof(Path))]
    [TemplatePart(Name = RadialProgressBar.ValuePathName, Type = typeof(Path))]
    [TemplatePart(Name = RadialProgressBar.MaxLabelName, Type = typeof(Label))]
    [TemplatePart(Name = RadialProgressBar.MinLabelPathName, Type = typeof(Label))]
    [TemplatePart(Name = RadialProgressBar.DetailContPathName, Type = typeof(Label))]
    [TemplatePart(Name = RadialProgressBar.PercentContPathName, Type = typeof(Label))]
    public class RadialProgressBar : System.Windows.Controls.Control
    {
        private const string RootBorderName = "PART_MainBorder";
        private const string MainGridName = "PART_MainGrid";
        private const string BackgroundPathName = "PART_BackgroundPath";
        private const string ValuePathName = "PART_CurValPath";
        private const string MaxLabelName = "PART_MaximumLabel";
        private const string MinLabelPathName = "PART_MiniumLabel";
        private const string DetailContPathName = "PART_DetailContLabel";
        private const string PercentContPathName = "PART_PercentContLabel";

        private const double _BaseRadius = 150d;
        private const double _BaseStrokeThickness = 17d;

        public RadialProgressBar()
        {
            this.DefaultStyleKey = typeof(RadialProgressBar);
        }

        #region Radius
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(
                "Radius",
                typeof(double),
                typeof(RadialProgressBar),
                new PropertyMetadata(150d, new PropertyChangedCallback(RadiusPropertyChangedCallback)),
                        new ValidateValueCallback(ValidateRadiusPropertyCallback));

        private static void RadiusPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rPB = d as RadialProgressBar;
            rPB?.OnRefreshUI();

        }

        private static bool ValidateRadiusPropertyCallback(object value)
        {
            double? val = Convert.ToDouble(value);
            return !(val == null || val < 10);
        }

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        #endregion

        #region StrokeThickness
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                "StrokeThickness",
                typeof(double),
                typeof(RadialProgressBar),
                new PropertyMetadata(10d, new PropertyChangedCallback(StrokeThicknessPropertyChangedCallback)));

        private static void StrokeThicknessPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rPB = d as RadialProgressBar;
            rPB?.OnRefreshUI();
        }


        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        #endregion

        #region Offset
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register(
                "Offset",
                typeof(double),
                typeof(RadialProgressBar),
                new PropertyMetadata(10d, new PropertyChangedCallback(OffsetPropertyChangedCallback)));

        private static void OffsetPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rPB = d as RadialProgressBar;
            rPB?.OnRefreshUI();
        }


        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }
        #endregion

        #region Value
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(RadialProgressBar),
                new PropertyMetadata(0d, new PropertyChangedCallback(ValuePropertyChangedCallback)));

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rPB = d as RadialProgressBar;
            rPB?.OnValueChanged((double)e.OldValue, (double)e.NewValue);
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
                typeof(RadialProgressBar),
                new PropertyMetadata(100d, new PropertyChangedCallback(MaximumPropertyChangedCallback)));

        private static void MaximumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rPB = d as RadialProgressBar;
            rPB?.UpdatePercentCont();
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        #endregion

        #region DetailContent
        public static readonly DependencyProperty DetailContentProperty =
            DependencyProperty.Register(
                "DetailContent",
                typeof(string),
                typeof(RadialProgressBar),
                new PropertyMetadata(""));

        public string DetailContent
        {
            get { return (string)GetValue(DetailContentProperty); }
            set { SetValue(DetailContentProperty, value); }
        }
        #endregion

        #region ExtraContent
        public static readonly DependencyProperty ExtraContentProperty =
            DependencyProperty.Register(
                "ExtraContent",
                typeof(string),
                typeof(RadialProgressBar),
                new PropertyMetadata(""));

        public string ExtraContent
        {
            get { return (string)GetValue(ExtraContentProperty); }
            set { SetValue(ExtraContentProperty, value); }
        }
        #endregion


        #region bgPathBrush
        public static readonly DependencyProperty BgPathBackgroundProperty =
            DependencyProperty.Register(
                "BgPathBackground",
                typeof(Brush),
                typeof(RadialProgressBar),
                new UIPropertyMetadata(new SolidColorBrush(Colors.Green)));

        public Brush BgPathBackground
        {
            get { return (Brush)GetValue(BgPathBackgroundProperty); }
            set { SetValue(BgPathBackgroundProperty, value); }
        }
        #endregion

        #region valPathBrush
        public static readonly DependencyProperty ValPathBackgroundProperty =
            DependencyProperty.Register(
                "ValPathBackground",
                typeof(Brush),
                typeof(RadialProgressBar),
                new UIPropertyMetadata(new SolidColorBrush(Colors.Aqua)));

        public Brush ValPathBackground
        {
            get { return (Brush)GetValue(ValPathBackgroundProperty); }
            set { SetValue(ValPathBackgroundProperty, value); }
        }
        #endregion

        #region PercentContForeground
        public static readonly DependencyProperty PercentContForegroundProperty =
            DependencyProperty.Register(
                "PercentContForeground",
                typeof(Brush),
                typeof(RadialProgressBar),
                new UIPropertyMetadata(new SolidColorBrush(Colors.Green)));

        public Brush PercentContForeground
        {
            get { return (Brush)GetValue(PercentContForegroundProperty); }
            set { SetValue(PercentContForegroundProperty, value); }
        }
        #endregion



        public event ValueChangedHandler ValueChanged;

        private Border RootBorder;
        private Grid MainGrid;
        private Path BgPath;
        private Path CurValPath;
        private Label MaxLabel;
        private Label MinLabel;
        private Label DetailContLabel;
        private Label PercentContLabel;

        private double CalculatedRadForPath = 0d;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RootBorder = GetTemplateChild(RootBorderName) as Border;
            MainGrid = GetTemplateChild(MainGridName) as Grid;
            BgPath = GetTemplateChild(BackgroundPathName) as Path;
            CurValPath = GetTemplateChild(ValuePathName) as Path;
            MaxLabel = GetTemplateChild(MaxLabelName) as Label;
            MinLabel = GetTemplateChild(MinLabelPathName) as Label;
            DetailContLabel = GetTemplateChild(DetailContPathName) as Label;
            PercentContLabel = GetTemplateChild(PercentContPathName) as Label;

            if (BgPath == null || CurValPath == null || MainGrid == null || RootBorder == null
                || MaxLabel == null || MinLabel == null || DetailContLabel == null || PercentContLabel == null)
            {
                throw new InvalidOperationException("Not found some UI elements!");
            }


            MainGrid.SizeChanged -= OnMainGridSizeChangedHandler;
            MainGrid.SizeChanged += OnMainGridSizeChangedHandler;
            UpdatePercentCont();
        }



        private void OnMainGridSizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            OnRefreshUI(true);
        }

        protected void OnRefreshUI(bool force = false)
        {
            if (!IsInitialized && !force) return;
            var radius = MainGrid.ActualWidth / 2;
            CaculateRadForPath(radius, StrokeThickness);
            CreateBackgroundPathData(BgPath, radius, StrokeThickness);
            CreateValuePathData(CurValPath, radius, StrokeThickness, Value, Maximum);
        }



        private void UpdatePercentCont()
        {
            if (!IsInitialized) return;
            if (Maximum > 0)
            {
                PercentContLabel.Content = Math.Round(100 * Value / Maximum, 2) + "%";
            }
            else
            {
                PercentContLabel.Content = 0 + "%";
            }
        }
        protected void OnValueChanged(double oldVal, double newVal)
        {
            if (!IsInitialized) return;

            var arg = new ValueChangedEventArgs(oldVal, newVal);
            ValueChanged?.Invoke(this, arg);

            if (arg.Handled == true) return;
            var radius = MainGrid.ActualWidth / 2;
            CreateValuePathData(CurValPath, radius, StrokeThickness, Value, Maximum);
            UpdatePercentCont();
        }



        private Geometry GetMainOpacityMask(double radius, double strokeThickness)
        {
            double cornerRadius = 1;
            var offsetThickness = 1;

            Point startClipPoint = new Point(0 + offsetThickness, (int)(radius + (strokeThickness / 2) - offsetThickness));
            Point arcSeg1DesPoint = new Point(strokeThickness + offsetThickness, (int)(radius + (strokeThickness / 2) - offsetThickness));
            Point arcSeg2DesPoint = new Point((int)(2 * radius - strokeThickness) - offsetThickness, (int)(radius + (strokeThickness / 2) - offsetThickness));
            Point arcSeg3DesPoint = new Point((int)(2 * radius) - offsetThickness, (int)(radius + (strokeThickness / 2) - offsetThickness));
            Point arcSeg4DesPoint = new Point(0 + offsetThickness, (int)(radius + (strokeThickness / 2) - offsetThickness));

            string clipData = "m {2},{3} A {0},{1} 0 0 0 {4},{5} A {0},{1} 0 0 1 {6},{7} A {0},{1} 0 0 0 {8},{9} A {0},{1} 0 0 0 {10},{11} Z";

            return Geometry.Parse(string.Format(clipData
                , cornerRadius
                , cornerRadius
                , startClipPoint.X
                , startClipPoint.Y
                , arcSeg1DesPoint.X
                , arcSeg1DesPoint.Y
                , arcSeg2DesPoint.X
                , arcSeg2DesPoint.Y
                , arcSeg3DesPoint.X
                , arcSeg3DesPoint.Y
                , arcSeg4DesPoint.X
                , arcSeg4DesPoint.Y));
        }

        private void CaculateRadForPath(double radius, double strokeThickness)
        {
            var offsetThickness = 1;

            Point startMainDataPoint = new Point(strokeThickness / 2 + offsetThickness, 0);
            Point arcSeg1MainDataDesPoint = new Point((int)(2 * radius - strokeThickness / 2 - offsetThickness), 0);

            CalculatedRadForPath = ((double)arcSeg1MainDataDesPoint.X - (double)startMainDataPoint.X) / 2;
        }
        private void CreateBackgroundPathData(Path path, double radius, double strokeThickness)
        {
            if (!IsInitialized) return;
            var offsetThicknessForX = 1;
            var offsetThicknessForY = radius - CalculatedRadForPath;
            path.Clip = GetMainOpacityMask(radius, strokeThickness);

            Point startMainDataPoint = new Point(strokeThickness / 2 + offsetThicknessForX, (int)(CalculatedRadForPath + strokeThickness + offsetThicknessForY));

            Point arcSeg1MainDataDesPoint = new Point((int)(2 * CalculatedRadForPath + strokeThickness / 2 + offsetThicknessForX), (int)(CalculatedRadForPath + strokeThickness + offsetThicknessForY));

            string mainData = "m {0},{1} A {2},{2} 0 0 1 {3},{4}";

            path.Data = Geometry.Parse(string.Format(mainData
                , startMainDataPoint.X
                , startMainDataPoint.Y
                , CalculatedRadForPath
                , arcSeg1MainDataDesPoint.X
                , arcSeg1MainDataDesPoint.Y));
            path.StrokeThickness = strokeThickness * 3;
        }

        private void CreateValuePathData(Path path, double radius, double strokeThickness, double currentValue, double maxValue)
        {
            if (!IsInitialized) return;
            var offsetThicknessForX = 1;
            var offsetThicknessForY = radius - CalculatedRadForPath;

            var percent = currentValue / maxValue;
            if (percent > 1) percent = 1;

            path.Clip = GetMainOpacityMask(radius, strokeThickness);

            Point startMainDataPoint = new Point(strokeThickness / 2 + offsetThicknessForX, (int)(CalculatedRadForPath + strokeThickness + offsetThicknessForY));

            Point arcSeg1MainDataDesPoint =
                new Point(
                    CalculatedRadForPath * (1 - Math.Cos(Math.PI * percent)) + strokeThickness / 2 + offsetThicknessForX
                , CalculatedRadForPath * (1 - Math.Sin(Math.PI * percent)) + strokeThickness + offsetThicknessForY);

            string mainData = "m {0},{1} A {2},{2} 0 0 1 {3},{4}";

            path.Data = Geometry.Parse(string.Format(mainData
                , startMainDataPoint.X
                , startMainDataPoint.Y
                , CalculatedRadForPath
                , arcSeg1MainDataDesPoint.X
                , arcSeg1MainDataDesPoint.Y));

            path.StrokeThickness = strokeThickness * 3;
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
