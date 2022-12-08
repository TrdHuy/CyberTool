using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace progtroll.views.elements.calendar_notebook
{
    /// <summary>
    /// Interaction logic for CalendarNotebookChartItem.xaml
    /// </summary>
    public partial class CalendarNotebookChartItem : UserControl
    {
        private static bool RUNE_IS_AUTO_SET_MAX_POINT_SCORE = false;
        private static bool RUNE_IS_USE_UPDATING_NOTIFICATION = true;
        private static bool RUNE_IS_UPDATING_CHART_ASYNC = true;
        private static string DEFINE_BASE_SEGMENT_NAME = "LineSegment_";
        private static string DEFINE_BASE_POINT_NAME = "EllipseGeo_";

        private class CalculatedData
        {
            public double base_X;
            public double base_Y;
            public int direction_X;
            public int direction_Y;
            public double Y_Ratio;
            public double lenghtPerLine;
        }

        private CalculatedData _currentCalculatedData = new CalculatedData();
        private double _maxPointScoreCache = 100;
        private SemaphoreSlim _semaphore;

        #region UpdatingNotification
        public static readonly DependencyProperty UpdatingNotificationProperty =
            DependencyProperty.Register(
                "UpdatingNotification",
                typeof(int),
                typeof(CalendarNotebookChartItem),
                new PropertyMetadata(0
                    , new PropertyChangedCallback(OnUpdatingNotificationCallback)));
        private static void OnUpdatingNotificationCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as CalendarNotebookChartItem;
            if (ctrl != null)
            {
                if (RUNE_IS_USE_UPDATING_NOTIFICATION)
                {
                    UpdateChartView(control: ctrl
                        , isUseAnimation: false
                        , isSetupPointToolTip: true);
                }
            }

        }
        public int UpdatingNotification
        {
            get { return (int)GetValue(UpdatingNotificationProperty); }
            set { SetValue(UpdatingNotificationProperty, value); }
        }
        #endregion

        #region Data
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                "Data",
                typeof(double[]),
                typeof(CalendarNotebookChartItem),
                new PropertyMetadata(default(double[])
                    , new PropertyChangedCallback(OnDataChangedCallback)));
        private static void OnDataChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as CalendarNotebookChartItem;
            if (ctrl != null)
            {
                if (RUNE_IS_AUTO_SET_MAX_POINT_SCORE)
                {
                    if (ctrl.Data != null)
                    {
                        ctrl._maxPointScoreCache = ctrl.Data.Max();
                    }
                }

                if (!RUNE_IS_USE_UPDATING_NOTIFICATION)
                {
                    UpdateChartView(control: ctrl
                        , isUseAnimation: false
                        , isSetupPointToolTip: true);
                }

            }

        }
        public double[] Data
        {
            get { return (double[])GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        #endregion

        #region MaxPointScore
        public static readonly DependencyProperty MaxPointScoreProperty =
            DependencyProperty.Register(
                "MaxPointScore",
                typeof(double),
                typeof(CalendarNotebookChartItem),
                new PropertyMetadata(100d, new PropertyChangedCallback(OnMaxPointScoreChangedCallback)));

        private static void OnMaxPointScoreChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as CalendarNotebookChartItem;
            if (ctrl != null)
            {
                if (!RUNE_IS_AUTO_SET_MAX_POINT_SCORE)
                {
                    ctrl._maxPointScoreCache = (double)e.NewValue;

                    if (!RUNE_IS_USE_UPDATING_NOTIFICATION)
                    {
                        UpdateChartView(control: ctrl
                            , isUseAnimation: false
                            , isSetupPointToolTip: true);
                    }
                }
            }
        }

        public double MaxPointScore
        {
            get { return (double)GetValue(MaxPointScoreProperty); }
            set { SetValue(MaxPointScoreProperty, value); }
        }
        #endregion

        #region PointRadius
        public static readonly DependencyProperty PointRadiusProperty =
            DependencyProperty.Register(
                "PointRadius",
                typeof(double),
                typeof(CalendarNotebookChartItem),
                new PropertyMetadata(5d, new PropertyChangedCallback(OnConfigChangedCallback)));

        public double PointRadius
        {
            get { return (double)GetValue(PointRadiusProperty); }
            set { SetValue(PointRadiusProperty, value); }
        }
        #endregion

        #region HeightRatio
        public static readonly DependencyProperty HeightRatioProperty =
            DependencyProperty.Register(
                "HeightRatio",
                typeof(double),
                typeof(CalendarNotebookChartItem),
                new PropertyMetadata(1.5d, new PropertyChangedCallback(OnConfigChangedCallback)));

        public double HeightRatio
        {
            get { return (double)GetValue(HeightRatioProperty); }
            set { SetValue(HeightRatioProperty, value); }
        }
        #endregion

        #region StretchPoint
        public static readonly DependencyProperty StretchPointProperty =
            DependencyProperty.Register(
                "StretchPoint",
                typeof(int),
                typeof(CalendarNotebookChartItem),
                new PropertyMetadata(2, new PropertyChangedCallback(OnConfigChangedCallback)));

        public int StretchPoint
        {
            get { return (int)GetValue(StretchPointProperty); }
            set { SetValue(StretchPointProperty, value); }
        }
        #endregion

        #region AnimationTime
        public static readonly DependencyProperty AnimationTimeProperty =
            DependencyProperty.Register(
                "AnimationTime",
                typeof(int),
                typeof(CalendarNotebookChartItem),
                new PropertyMetadata(1000));

        public int AnimationTime
        {
            get { return (int)GetValue(AnimationTimeProperty); }
            set { SetValue(AnimationTimeProperty, value); }
        }
        #endregion

        #region IsLoadingData
        public static readonly DependencyProperty IsLoadingDataProperty =
            DependencyProperty.Register(
                "IsLoadingData",
                typeof(bool),
                typeof(CalendarNotebookChartItem),
                new PropertyMetadata(false, OnIsLoadingDataChangedCallback));

        private static void OnIsLoadingDataChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as CalendarNotebookChartItem;
            ctrl?.UpdateLoadingState((bool)e.NewValue);
        }

        public bool IsLoadingData
        {
            get { return (bool)GetValue(IsLoadingDataProperty); }
            set { SetValue(IsLoadingDataProperty, value); }
        }
        #endregion

        public CalendarNotebookChartItem()
        {
            InitializeComponent();
            PART_MainCanvas.SizeChanged += PART_MainCanvas_SizeChanged;
            PART_MainCanvas.Loaded += PART_MainCanvas_Loaded;
            DataContextChanged -= HandleDataContextChanged;
            DataContextChanged += HandleDataContextChanged;
            _semaphore = new SemaphoreSlim(1, 1);
            UpdateLoadingState(IsLoadingData);
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(DataContext != null)
            {
                Binding isLoadingBinding = new Binding();
                isLoadingBinding.Source = DataContext;
                isLoadingBinding.Path = new PropertyPath("IsLoadingData");
                isLoadingBinding.Mode = BindingMode.TwoWay;
                isLoadingBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                SetBinding(CalendarNotebookChartItem.IsLoadingDataProperty, isLoadingBinding);

            }
        }

        private static void OnConfigChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as CalendarNotebookChartItem;
            if (ctrl != null)
            {
                UpdateChartView(control: ctrl
                  , isUseAnimation: ctrl.PART_MainCanvas.Visibility == Visibility.Visible
                  , isSetupPointToolTip: true);
            }
        }

        private void UpdateLoadingState(bool isLoading)
        {
            var oldContentVisiblity = PART_MainCanvas.Visibility;
            PART_LoadingAnimation.IsBusy = isLoading;
            PART_MainCanvas.Visibility = isLoading ? Visibility.Hidden : Visibility.Visible;

            if (oldContentVisiblity != PART_MainCanvas.Visibility && !isLoading)
            {
                GenerateAnimation(
                    pathFigureParents: PART_LinePath
                   , pointPath: PART_PointPath
                   , points: Data
                   , animationTime: AnimationTime
                   , baseSegmentName: DEFINE_BASE_SEGMENT_NAME
                   , basePointName: DEFINE_BASE_POINT_NAME
                   , base_X: _currentCalculatedData.base_X
                   , base_Y: _currentCalculatedData.base_Y
                   , direction_X: _currentCalculatedData.direction_X
                   , direction_Y: _currentCalculatedData.direction_Y
                   , Y_Ratio: _currentCalculatedData.Y_Ratio
                   , lenghtPerLine: _currentCalculatedData.lenghtPerLine);
            }
        }

        private void PART_MainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateChartView(control: this
                   , isUseAnimation: PART_MainCanvas.Visibility == Visibility.Visible
                   , isSetupPointToolTip: true);
        }

        private void PART_MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoaded)
                UpdateChartView(control: this
                    , isUseAnimation: false
                    , isSetupPointToolTip: true);
        }

        private static void UpdateChartView(CalendarNotebookChartItem control
            , bool isUseAnimation
            , bool isSetupPointToolTip)
        {
            if (!RUNE_IS_UPDATING_CHART_ASYNC)
            {
                GenerateLineSegmentAndPointGeometries(
                                     toolTipCanvas: control.PART_ToolTipCanvas
                                    , pathFigure: control.PART_LinePathFigure
                                    , pathFigureParents: control.PART_LinePath
                                    , pointPath: control.PART_PointPath
                                    , points: control.Data
                                    , maxPointScore: control._maxPointScoreCache
                                    , chartHeight: control.ActualHeight
                                    , chartWidth: control.ActualWidth
                                    , pointRadius: control.PointRadius
                                    , heightRatio: control.HeightRatio
                                    , stretchPoints: control.StretchPoint
                                    , calculatedDataCache: control._currentCalculatedData
                                    , animationTime: control.AnimationTime
                                    , isUseAnimation: isUseAnimation
                                    , isSetupPointToolTip: isSetupPointToolTip);
            }
            else
            {
                GenerateLineSegmentAndPointGeometriesAsync(
                                     control.PART_ToolTipCanvas
                                    , control.PART_LinePathFigure
                                    , control.PART_LinePath
                                    , control.PART_PointPath
                                    , points: control.Data
                                    , maxPointScore: control._maxPointScoreCache
                                    , chartHeight: control.ActualHeight
                                    , chartWidth: control.ActualWidth
                                    , pointRadius: control.PointRadius
                                    , heightRatio: control.HeightRatio
                                    , stretchPoints: control.StretchPoint
                                    , animationTime: control.AnimationTime
                                    , semaphore: control._semaphore
                                    , calculatedDataCache: control._currentCalculatedData
                                    , isUseAnimation: isUseAnimation
                                    , isSetupPointToolTip: isSetupPointToolTip);
            }

        }

        private static void GenerateLineSegmentAndPointGeometries(
             Canvas toolTipCanvas
            , PathFigure pathFigure
            , Path pathFigureParents
            , Path pointPath
            , double[]? points
            , double maxPointScore
            , double chartHeight
            , double chartWidth
            , double pointRadius
            , double heightRatio
            , int stretchPoints
            , CalculatedData calculatedDataCache
            , int animationTime = 1000
            , bool isUseAnimation = true
            , bool isSetupPointToolTip = false)
        {
            if (points == null
                || points.Length == 0
                || chartHeight == 0
                || chartWidth == 0
                || points.Length <= stretchPoints) return;

            if (maxPointScore == 0) maxPointScore = 100;

            var baseSegmentName = DEFINE_BASE_SEGMENT_NAME;
            var basePointName = DEFINE_BASE_POINT_NAME;

            double base_X = 0;
            double base_Y = chartHeight - pointRadius - 2;
            int direction_X = 1;
            int direction_Y = -1;
            double Y_Ratio = maxPointScore / base_Y * heightRatio;
            double lenghtPerLine = chartWidth / (points.Length + 1);
            if (stretchPoints > 0)
            {
                lenghtPerLine = chartWidth / (points.Length - stretchPoints);
                base_X = 0 - lenghtPerLine * ((stretchPoints + 1) / 2 + 0.5);
            }
            UpdateCalculatedData(calculatedDataCache, base_X, base_Y, direction_X, direction_Y, Y_Ratio, lenghtPerLine);
            pathFigure.StartPoint = new Point(base_X, base_Y);
            // Generate segment collection
            {
                var lineSegmentCollection = new PathSegmentCollection();
                var current_X = base_X;
                for (int i = 0; i < points.Length; i++)
                {
                    current_X += lenghtPerLine * direction_X;
                    var cor_Y = points[i] / Y_Ratio * direction_Y + base_Y;
                    var lineSegment = new LineSegment();
                    lineSegment.Point = new Point(current_X, cor_Y);
                    lineSegmentCollection.Add(lineSegment);
                    var lineSegmentName = baseSegmentName + i;

                    try
                    {
                        pathFigureParents.UnregisterName(lineSegmentName);
                    }
                    catch { }
                    finally
                    {
                        pathFigureParents.RegisterName(lineSegmentName, lineSegment);
                    }
                }
                current_X += lenghtPerLine * direction_X;
                var lastLineSegment = new LineSegment();
                lastLineSegment.Point = new Point(current_X, base_Y);
                lineSegmentCollection.Add(lastLineSegment);
                pathFigure.Segments = lineSegmentCollection;
            }

            // Generate geometry collection
            {
                var ellipseGeometryGroup = new GeometryGroup();
                var firstPoint = new EllipseGeometry()
                {
                    Center = new Point(base_X, base_Y),
                    RadiusX = pointRadius,
                    RadiusY = pointRadius
                };

                //Setup tooltip
                if (isSetupPointToolTip)
                {
                    toolTipCanvas.Children.Clear();
                    var elipseTip = new Ellipse()
                    {
                        Height = pointRadius * 2,
                        Width = pointRadius * 2,
                        Fill = new SolidColorBrush(Colors.Transparent),
                        ToolTip = "0",
                    };
                    Canvas.SetLeft(elipseTip, base_X - pointRadius);
                    Canvas.SetTop(elipseTip, base_Y - pointRadius);
                    toolTipCanvas.Children.Add(elipseTip);
                }
                ellipseGeometryGroup.Children.Add(firstPoint);
                var current_X = base_X;
                for (int i = 0; i < points.Length; i++)
                {
                    current_X += lenghtPerLine * direction_X;
                    var cor_Y = points[i] / Y_Ratio * direction_Y + base_Y;
                    var point = new EllipseGeometry()
                    {
                        Center = new Point(current_X, cor_Y),
                        RadiusX = pointRadius,
                        RadiusY = pointRadius
                    };
                    ellipseGeometryGroup.Children.Add(point);
                    var ellipseGeometryName = basePointName + i;

                    //Setup tooltip
                    if (isSetupPointToolTip)
                    {
                        var elipseTip = new Ellipse()
                        {
                            Height = pointRadius * 2,
                            Width = pointRadius * 2,
                            Fill = new SolidColorBrush(Colors.Transparent),
                            ToolTip = "" + points[i],
                        };
                        Canvas.SetLeft(elipseTip, current_X - pointRadius);
                        Canvas.SetTop(elipseTip, cor_Y - pointRadius);
                        toolTipCanvas.Children.Add(elipseTip);
                    }
                    try
                    {
                        pointPath.UnregisterName(ellipseGeometryName);
                    }
                    catch { }
                    finally
                    {
                        pointPath.RegisterName(ellipseGeometryName, point);
                    }
                }

                current_X += lenghtPerLine * direction_X;
                var lastPoint = new EllipseGeometry()
                {
                    Center = new Point(current_X, base_Y),
                    RadiusX = pointRadius,
                    RadiusY = pointRadius
                };

                //Setup tooltip
                if (isSetupPointToolTip)
                {
                    var elipseTip = new Ellipse()
                    {
                        Height = pointRadius * 2,
                        Width = pointRadius * 2,
                        Fill = new SolidColorBrush(Colors.Transparent),
                        ToolTip = "0",
                    };
                    Canvas.SetLeft(elipseTip, current_X - pointRadius);
                    Canvas.SetTop(elipseTip, base_Y - pointRadius);
                    toolTipCanvas.Children.Add(elipseTip);
                }
                ellipseGeometryGroup.Children.Add(lastPoint);
                pointPath.Data = ellipseGeometryGroup;
            }

            // Generate Animation
            if (isUseAnimation)
            {
                GenerateAnimation(pathFigureParents
                    , pointPath
                    , points
                    , animationTime
                    , baseSegmentName
                    , basePointName
                    , base_X
                    , base_Y
                    , direction_X
                    , direction_Y
                    , Y_Ratio
                    , lenghtPerLine);
            }

        }

        private static void UpdateCalculatedData(CalculatedData data
            , double base_X
            , double base_Y
            , int direction_X
            , int direction_Y
            , double Y_Ratio
            , double lenghtPerLine)
        {
            data.base_X = base_X;
            data.base_Y = base_Y;
            data.direction_X = direction_X;
            data.direction_Y = direction_Y;
            data.Y_Ratio = Y_Ratio;
            data.lenghtPerLine = lenghtPerLine;
        }

        private static void GenerateAnimation(Path pathFigureParents
            , Path pointPath
            , double[] points
            , int animationTime
            , string baseSegmentName
            , string basePointName
            , double base_X
            , double base_Y
            , int direction_X
            , int direction_Y
            , double Y_Ratio
            , double lenghtPerLine)
        {
            var animTime = animationTime;
            var current_X = base_X;
            Storyboard segmentSB = new Storyboard();
            Storyboard pointSB = new Storyboard();

            Random rnd = new Random();
            for (int i = 0; i < points.Length; i++)
            {
                current_X += lenghtPerLine * direction_X;
                var cor_Y = points[i] / Y_Ratio * direction_Y + base_Y;
                var animTime_1 = animTime / 2;
                var animTime_2 = animTime - animTime_1;
                var to_Y = rnd.Next(0, (int)cor_Y);

                PointAnimation pointAnimation_1 = new PointAnimation();
                Storyboard.SetTargetName(pointAnimation_1, baseSegmentName + i);
                Storyboard.SetTargetProperty(pointAnimation_1, new PropertyPath(LineSegment.PointProperty));
                pointAnimation_1.BeginTime = TimeSpan.FromMilliseconds(0);
                pointAnimation_1.Duration = new Duration(TimeSpan.FromMilliseconds(animTime_1));
                pointAnimation_1.From = new Point(current_X, base_Y);
                pointAnimation_1.To = new Point(current_X, to_Y);
                PointAnimation pointAnimation_2 = new PointAnimation();
                Storyboard.SetTargetName(pointAnimation_2, baseSegmentName + i);
                Storyboard.SetTargetProperty(pointAnimation_2, new PropertyPath(LineSegment.PointProperty));
                pointAnimation_2.BeginTime = TimeSpan.FromMilliseconds(animTime_1);
                pointAnimation_2.Duration = new Duration(TimeSpan.FromMilliseconds(animTime_2));
                pointAnimation_2.From = new Point(current_X, to_Y);
                pointAnimation_2.To = new Point(current_X, cor_Y);

                segmentSB.Children.Add(pointAnimation_1);
                segmentSB.Children.Add(pointAnimation_2);

                PointAnimation pointAnimation_3 = new PointAnimation();
                Storyboard.SetTargetName(pointAnimation_3, basePointName + i);
                Storyboard.SetTargetProperty(pointAnimation_3, new PropertyPath(EllipseGeometry.CenterProperty));
                pointAnimation_3.BeginTime = TimeSpan.FromMilliseconds(0);
                pointAnimation_3.Duration = new Duration(TimeSpan.FromMilliseconds(animTime_1));
                pointAnimation_3.From = new Point(current_X, base_Y);
                pointAnimation_3.To = new Point(current_X, to_Y);
                PointAnimation pointAnimation_4 = new PointAnimation();
                Storyboard.SetTargetName(pointAnimation_4, basePointName + i);
                Storyboard.SetTargetProperty(pointAnimation_4, new PropertyPath(EllipseGeometry.CenterProperty));
                pointAnimation_4.BeginTime = TimeSpan.FromMilliseconds(animTime_1);
                pointAnimation_4.Duration = new Duration(TimeSpan.FromMilliseconds(animTime_2));
                pointAnimation_4.From = new Point(current_X, to_Y);
                pointAnimation_4.To = new Point(current_X, cor_Y);

                pointSB.Children.Add(pointAnimation_3);
                pointSB.Children.Add(pointAnimation_4);
            }
            segmentSB.Begin(pathFigureParents);
            pointSB.Begin(pointPath);
        }

        private static async void GenerateLineSegmentAndPointGeometriesAsync(
            Canvas toolTipCanvas
           , PathFigure pathFigure
           , Path pathFigureParents
           , Path pointPath
           , double[]? points
           , double maxPointScore
           , double chartHeight
           , double chartWidth
           , double pointRadius
           , double heightRatio
           , int stretchPoints
           , SemaphoreSlim semaphore
           , CalculatedData calculatedDataCache
           , int animationTime = 1000
           , bool isUseAnimation = true
           , bool isSetupPointToolTip = false)
        {

            await semaphore.WaitAsync();
            try
            {
                GenerateLineSegmentAndPointGeometries(toolTipCanvas
                    , pathFigure
                    , pathFigureParents
                    , pointPath
                    , points
                    , maxPointScore
                    , chartHeight
                    , chartWidth
                    , pointRadius
                    , heightRatio
                    , stretchPoints
                    , calculatedDataCache
                    , animationTime
                    , isUseAnimation
                    , isSetupPointToolTip);
            }
            catch
            {

            }
            finally
            {
                semaphore.Release();
            }

        }
    }
}
