using honeyboard_release_service.views.elements.calendar_notebook.@base;
using honeyboard_release_service.views.elements.calendar_notebook.data_structure;
using honeyboard_release_service.views.elements.calendar_notebook.definitions;
using honeyboard_release_service.views.elements.calendar_notebook.extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace honeyboard_release_service.views.elements.calendar_notebook
{
    /// <summary>
    /// Interaction logic for CalendarNotebook.xaml
    /// </summary>
    public partial class CalendarNotebook : UserControl
    {
        /// <summary>
        /// Quản lý dữ liệu binding trên các listview trong 1 item project
        /// Bao gồm các tập collection các commit
        /// Mảng chứa tổng số commit theo từng ngày
        /// Số lượng lớn commit lớn nhất trong 1 project theo ngày
        /// </summary>
        private class SourceCacheManager : INotifyPropertyChanged
        {
            private int _listViewSize;
            public event PropertyChangedEventHandler? PropertyChanged;
            public double[] ProjectDataForChart { get; private set; }
            public double ChartViewMaxPointScore { get; private set; }
            public int ChartViewUpdatingNotificationCounter { get; private set; }
            public NotebookListObservableCollection<ObservableCollection<ICalendarNotebookCommitItemContext>> ListViewSource { get; set; }

            public SourceCacheManager(int listViewSize = 9)
            {
                _listViewSize = listViewSize;
                ProjectDataForChart = new double[7];
                ChartViewMaxPointScore = double.MinValue;
                ListViewSource = new NotebookListObservableCollection<ObservableCollection<ICalendarNotebookCommitItemContext>>(listViewSize);
            }

            public void ExtendListViewSize(int sizeChanged)
            {
                _listViewSize += sizeChanged;
                if (sizeChanged > 0)
                {
                    ListViewSource.IncreaseSize(amount: sizeChanged
                        , isNotify: true
                        , isLast: true);
                }
                else if (sizeChanged < 0)
                {
                    ListViewSource.DecreaseSize(amount: -sizeChanged
                       , isNotify: true
                       , isLast: true);
                }
            }

            public void UpdateListViewSize(int newSize)
            {
                if (_listViewSize != newSize)
                {
                    int sizeChanged = newSize - _listViewSize;
                    if (sizeChanged > 0)
                    {
                        ListViewSource.IncreaseSize(amount: sizeChanged
                            , isNotify: true
                            , isLast: true);
                    }
                    else if (sizeChanged < 0)
                    {
                        ListViewSource.DecreaseSize(amount: -sizeChanged
                           , isNotify: true
                           , isLast: true);
                    }
                    _listViewSize = newSize;
                }
            }

            public void ResetChartMaxPointScore()
            {
                ChartViewMaxPointScore = double.MinValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProjectDataForChart"));
            }

            public void SetItemContext(int index
                , ObservableCollection<ICalendarNotebookCommitItemContext> context
                , bool isNotifyChanged = true)
            {
                if (index >= _listViewSize)
                {
                    throw new ArgumentException("index must be < than " + _listViewSize);
                }

                ListViewSource.Replace(index, context);

                if (isNotifyChanged)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceCache"));
            }

            public void SynthesisDataForChart(int columnNumber
                , bool isNotifyChanged = true)
            {
                ProjectDataForChart = new double[columnNumber];
                var isMaxPointScoreChanged = false;
                ChartViewUpdatingNotificationCounter++;

                if (ListViewSource != null)
                {
                    for (int i = 0; i < columnNumber; i++)
                    {
                        ProjectDataForChart[i] = ListViewSource[i]?.Count ?? 0;
                        if (ChartViewMaxPointScore < ProjectDataForChart[i])
                        {
                            ChartViewMaxPointScore = ProjectDataForChart[i];
                            isMaxPointScoreChanged = true;
                        }
                    }
                }
                if (isNotifyChanged && isMaxPointScoreChanged)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChartViewMaxPointScore"));
                }

                if (isNotifyChanged)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProjectDataForChart"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChartViewUpdatingNotificationCounter"));
                }
            }

            public void NotifyCacheChanged()
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceCache"));
            }

            public void NotifyChartDataChanged()
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProjectDataForChart"));
            }
        }

        private const CalendarNotebookDateMode RUNE_START_DATE_MODE = CalendarNotebookDateMode.Month;
        private const int RUNE_MINIMUM_DISPLAYED_COLUMN_NUMBER = 7;
        private const int RUNE_MAXIMUM_DISPLAYED_COLUMN_NUMBER = 31;

        private bool _isNeedUpdateWhenDisplayDateTimePropertyChanged = true;
        private bool _isChangeDateAnimationFinish = true;
        private CalendarNotebookViewMode _viewMode = CalendarNotebookViewMode.Chart;
        private CalendarNotebookDateMode _dateMode = RUNE_START_DATE_MODE;
        private int _movingColumnNumberIndex = 0;
        private int[] _movingColumnNumberArray = new int[] { 1, 3, 7 };
        private int _currentDisplayedColumnNumber = 7;
        private DateTime _startTimeCache = RUNE_START_DATE_MODE == CalendarNotebookDateMode.Day ?
                    DateTime.Now.StartOfWeek(DayOfWeek.Monday) :
                    DateTime.Now.StartOfYear();
        private DateTime _endTimeCache = RUNE_START_DATE_MODE == CalendarNotebookDateMode.Day ?
                    DateTime.Now.EndOfWeek(DayOfWeek.Sunday) :
                    DateTime.Now.EndOfYear();

        private Dictionary<ICalendarNotebookProjectItemContext, SourceCacheManager> _sourceCacheManagerMap
            = new Dictionary<ICalendarNotebookProjectItemContext, SourceCacheManager>();

        #region HeaderHeight
        public static readonly DependencyProperty HeaderHeightProperty =
            DependencyProperty.Register(
                "HeaderHeight",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(45d));

        public double HeaderHeight
        {
            get { return (double)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }
        #endregion

        #region ProjectColumnWidth
        public static readonly DependencyProperty ProjectColumnWidthProperty =
            DependencyProperty.Register(
                "ProjectColumnWidth",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(60d));

        public double ProjectColumnWidth
        {
            get { return (double)GetValue(ProjectColumnWidthProperty); }
            set { SetValue(ProjectColumnWidthProperty, value); }
        }
        #endregion

        #region RightGapColumnWidth
        public static readonly DependencyProperty RightGapColumnWidthProperty =
            DependencyProperty.Register(
                "RightGapColumnWidth",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(30d));

        public double RightGapColumnWidth
        {
            get { return (double)GetValue(RightGapColumnWidthProperty); }
            set { SetValue(RightGapColumnWidthProperty, value); }
        }
        #endregion

        #region HeaderCornerRadius
        public static readonly DependencyProperty HeaderCornerRadiusProperty =
            DependencyProperty.Register(
                "HeaderCornerRadius",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(5d));

        public double HeaderCornerRadius
        {
            get { return (double)GetValue(HeaderCornerRadiusProperty); }
            set { SetValue(HeaderCornerRadiusProperty, value); }
        }
        #endregion

        #region HeaderMarginTop
        public static readonly DependencyProperty HeaderMarginTopProperty =
            DependencyProperty.Register(
                "HeaderMarginTop",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(10d));

        public double HeaderMarginTop
        {
            get { return (double)GetValue(HeaderMarginTopProperty); }
            set { SetValue(HeaderMarginTopProperty, value); }
        }
        #endregion

        #region HeaderMarginLeft
        public static readonly DependencyProperty HeaderMarginLeftProperty =
            DependencyProperty.Register(
                "HeaderMarginLeft",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(5d));

        public double HeaderMarginLeft
        {
            get { return (double)GetValue(HeaderMarginLeftProperty); }
            set { SetValue(HeaderMarginLeftProperty, value); }
        }
        #endregion

        #region HeaderMarginRight
        public static readonly DependencyProperty HeaderMarginRightProperty =
            DependencyProperty.Register(
                "HeaderMarginRight",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(5d));

        public double HeaderMarginRight
        {
            get { return (double)GetValue(HeaderMarginRightProperty); }
            set { SetValue(HeaderMarginRightProperty, value); }
        }
        #endregion

        #region HeaderMarginBottom
        public static readonly DependencyProperty HeaderMarginBottomProperty =
            DependencyProperty.Register(
                "HeaderMarginBottom",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(5d));

        public double HeaderMarginBottom
        {
            get { return (double)GetValue(HeaderMarginBottomProperty); }
            set { SetValue(HeaderMarginBottomProperty, value); }
        }
        #endregion

        #region HeaderContentMarginLeft
        public static readonly DependencyProperty HeaderContentMarginLeftProperty =
            DependencyProperty.Register(
                "HeaderContentMarginLeft",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(8d));

        public double HeaderContentMarginLeft
        {
            get { return (double)GetValue(HeaderContentMarginLeftProperty); }
            set { SetValue(HeaderContentMarginLeftProperty, value); }
        }
        #endregion

        #region HeaderContentFontSize
        public static readonly DependencyProperty HeaderContentFontSizeProperty =
            DependencyProperty.Register(
                "HeaderContentFontSize",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(10d));

        public double HeaderContentFontSize
        {
            get { return (double)GetValue(HeaderContentFontSizeProperty); }
            set { SetValue(HeaderContentFontSizeProperty, value); }
        }
        #endregion

        #region ProjectItemHeight
        public static readonly DependencyProperty ProjectItemHeightProperty =
            DependencyProperty.Register(
                "ProjectItemHeight",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(100d));

        public double ProjectItemHeight
        {
            get { return (double)GetValue(ProjectItemHeightProperty); }
            set { SetValue(ProjectItemHeightProperty, value); }
        }
        #endregion

        #region HeaderItemHeight
        public static readonly DependencyProperty HeaderItemHeightProperty =
            DependencyProperty.Register(
                "HeaderItemHeight",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(90d));

        public double HeaderItemHeight
        {
            get { return (double)GetValue(HeaderItemHeightProperty); }
            set { SetValue(HeaderItemHeightProperty, value); }
        }
        #endregion

        #region HeaderItemWidth
        public static readonly DependencyProperty HeaderItemWidthProperty =
            DependencyProperty.Register(
                "HeaderItemWidth",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(30d));

        public double HeaderItemWidth
        {
            get { return (double)GetValue(HeaderItemWidthProperty); }
            set { SetValue(HeaderItemWidthProperty, value); }
        }
        #endregion

        #region HeaderItemCornerRad
        public static readonly DependencyProperty HeaderItemCornerRadProperty =
            DependencyProperty.Register(
                "HeaderItemCornerRad",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(5d));

        public double HeaderItemCornerRad
        {
            get { return (double)GetValue(HeaderItemCornerRadProperty); }
            set { SetValue(HeaderItemCornerRadProperty, value); }
        }
        #endregion

        #region HeaderItemFontSize
        public static readonly DependencyProperty HeaderItemFontSizeProperty =
            DependencyProperty.Register(
                "HeaderItemFontSize",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(10d));

        public double HeaderItemFontSize
        {
            get { return (double)GetValue(HeaderItemFontSizeProperty); }
            set { SetValue(HeaderItemFontSizeProperty, value); }
        }
        #endregion

        #region ItemCornerRad
        public static readonly DependencyProperty ItemCornerRadProperty =
            DependencyProperty.Register(
                "ItemCornerRad",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(5d));

        public double ItemCornerRad
        {
            get { return (double)GetValue(ItemCornerRadProperty); }
            set { SetValue(ItemCornerRadProperty, value); }
        }
        #endregion

        #region ItemMargin
        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.Register(
                "ItemMargin",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(5d));

        public double ItemMargin
        {
            get { return (double)GetValue(ItemMarginProperty); }
            set { SetValue(ItemMarginProperty, value); }
        }
        #endregion

        #region ItemFontSize
        public static readonly DependencyProperty ItemFontSizeProperty =
            DependencyProperty.Register(
                "ItemFontSize",
                typeof(double),
                typeof(CalendarNotebook),
                new PropertyMetadata(10d));

        public double ItemFontSize
        {
            get { return (double)GetValue(ItemFontSizeProperty); }
            set { SetValue(ItemFontSizeProperty, value); }
        }
        #endregion

        #region StartTime
        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register(
                "StartTime",
                typeof(DateTime),
                typeof(CalendarNotebook),
                new PropertyMetadata(RUNE_START_DATE_MODE == CalendarNotebookDateMode.Day ?
                    DateTime.Now.StartOfWeek(DayOfWeek.Monday) :
                    DateTime.Now.StartOfYear()
                    , new PropertyChangedCallback(OnDisplayTimeChangedCallback)));

        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }
        #endregion

        #region EndTime
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register(
                "EndTime",
                typeof(DateTime),
                typeof(CalendarNotebook),
                new PropertyMetadata(RUNE_START_DATE_MODE == CalendarNotebookDateMode.Day ?
                    DateTime.Now.EndOfWeek(DayOfWeek.Sunday) :
                    DateTime.Now.EndOfYear()
                    , new PropertyChangedCallback(OnDisplayTimeChangedCallback)));

        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }
        #endregion

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(ObservableCollection<ICalendarNotebookProjectItemContext>),
                typeof(CalendarNotebook),
                new PropertyMetadata(default(ObservableCollection<ICalendarNotebookProjectItemContext>)
                    , new PropertyChangedCallback(OnItemsSourceChangedCallback)));

        private static void OnItemsSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as CalendarNotebook;
            ctrl?._sourceCacheManagerMap.Clear();
            ctrl?.UpdateProjectContentPanelWhenSourceChanged(
                e.OldValue as ObservableCollection<ICalendarNotebookProjectItemContext>
                , e.NewValue as ObservableCollection<ICalendarNotebookProjectItemContext>);
        }

        public ObservableCollection<ICalendarNotebookProjectItemContext>? ItemsSource
        {
            get { return (ObservableCollection<ICalendarNotebookProjectItemContext>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        #endregion

        private static void OnDisplayTimeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as CalendarNotebook;
            ctrl?.UpdateCurrentDisplayedColumnNumber();
            ctrl?.UpdateDisplayTimeOfHeaderPanel(ctrl._isNeedUpdateWhenDisplayDateTimePropertyChanged);

            ctrl?.UpdateAllProjectDataContent(ctrl._isNeedUpdateWhenDisplayDateTimePropertyChanged);
        }

        public CalendarNotebook()
        {
            InitializeComponent();
            UpdateCurrentDisplayedColumnNumber();
            UpdateDisplayTimeOfHeaderPanel();
            if (ItemsSource != null)
            {
                UpdateAllProjectDataContent();
            }
        }

        private void UpdateCurrentDisplayedColumnNumber()
        {
            _startTimeCache = StartTime;
            _endTimeCache = EndTime;

            if (_dateMode == CalendarNotebookDateMode.Day)
            {
                _currentDisplayedColumnNumber = Convert.ToInt32((EndTime - StartTime).TotalDays) + 1;

                if (_currentDisplayedColumnNumber < RUNE_MINIMUM_DISPLAYED_COLUMN_NUMBER)
                {
                    _currentDisplayedColumnNumber = RUNE_MINIMUM_DISPLAYED_COLUMN_NUMBER;
                }
                else if (_currentDisplayedColumnNumber > RUNE_MAXIMUM_DISPLAYED_COLUMN_NUMBER)
                {
                    _currentDisplayedColumnNumber = RUNE_MAXIMUM_DISPLAYED_COLUMN_NUMBER;
                }
            }
            else if (_dateMode == CalendarNotebookDateMode.Month)
            {
                _currentDisplayedColumnNumber = Convert.ToInt32(StartTime.TotalMonths(EndTime)) + 1;

                if (_currentDisplayedColumnNumber < RUNE_MINIMUM_DISPLAYED_COLUMN_NUMBER)
                {
                    _currentDisplayedColumnNumber = RUNE_MINIMUM_DISPLAYED_COLUMN_NUMBER;
                }
                else if (_currentDisplayedColumnNumber > RUNE_MAXIMUM_DISPLAYED_COLUMN_NUMBER)
                {
                    _currentDisplayedColumnNumber = RUNE_MAXIMUM_DISPLAYED_COLUMN_NUMBER;
                }
            }

            PART_ColumnNumberMenuItem.Header = _currentDisplayedColumnNumber + " columns";
        }

        private void HandleViewLoadedEvent(object sender, RoutedEventArgs e)
        {
            var fE = sender as FrameworkElement;
            if (fE != null)
            {
                switch (fE.Name)
                {
                    case "PART_CalendarNotebook":
                        // Cập nhật màu hiển thị cho item view mode hiện tại
                        {
                            switch (_viewMode)
                            {
                                case CalendarNotebookViewMode.List:
                                    (PART_ListViewTypeMenuItem.Icon as Path)?
                                        .SetResourceReference(Path.FillProperty, "ButtonBackground_Level1");
                                    break;
                                case CalendarNotebookViewMode.Chart:
                                    (PART_ChartViewTypeMenuItem.Icon as Path)?
                                        .SetResourceReference(Path.FillProperty, "ButtonBackground_Level1");
                                    break;
                            }
                        }
                        // Cập nhật màu hiển thị cho item date mode hiện tại
                        {
                            switch (_dateMode)
                            {
                                case CalendarNotebookDateMode.Day:
                                    (PART_DayModeMenuItem.Icon as Path)?
                                        .SetResourceReference(Path.FillProperty, "ButtonBackground_Level1");
                                    break;
                                case CalendarNotebookDateMode.Month:
                                    (PART_MonthModeMenuItem.Icon as Path)?
                                        .SetResourceReference(Path.FillProperty, "ButtonBackground_Level1");
                                    break;
                            }
                        }
                        break;
                    case "ProjectItemContainBorder":
                        var context = fE.DataContext as ICalendarNotebookProjectItemContext;
                        if (context != null)
                        {
                            context.OnBegin();
                        }
                        break;
                }
            }
        }

        #region HeaderPanel
        private void UpdateDisplayTimeOfHeaderPanel(bool force = true)
        {
            if (!force)
            {
                return;
            }

            var gridContent = new Grid();

            FramingHeaderPanelGridViewColumn(gridContent
                , _currentDisplayedColumnNumber);
            GenerateColumnHeader(gridContent
                , _currentDisplayedColumnNumber
                , StartTime
                , HeaderCornerRadius
                , HeaderMarginLeft
                , HeaderMarginTop
                , HeaderMarginRight
                , HeaderMarginBottom
                , HeaderContentMarginLeft
                , HeaderContentFontSize
                , _dateMode);
            PART_HeaderPanel.Children.Clear();
            PART_HeaderPanel.Children.Add(gridContent);
        }

        private void ChangeDisplayDateTime(bool moveLeft)
        {
            if (!_isChangeDateAnimationFinish) return;

            var newGridContent = new Grid();
            FramingHeaderPanelGridViewColumn(newGridContent
                , _currentDisplayedColumnNumber);
            GenerateColumnHeader(newGridContent
                , _currentDisplayedColumnNumber
                , StartTime
                , HeaderCornerRadius
                , HeaderMarginLeft
                , HeaderMarginTop
                , HeaderMarginRight
                , HeaderMarginBottom
                , HeaderContentMarginLeft
                , HeaderContentFontSize
                , _dateMode);
            BeginChangeDisplayDateTimeAnimationForHeader(moveLeft
                , newGridContent
                , PART_HeaderPanel
                , _movingColumnNumberArray[_movingColumnNumberIndex]
                , _currentDisplayedColumnNumber);
        }

        private void BeginChangeDisplayDateTimeAnimationForHeader(bool moveLeft
            , Grid newContent
            , Grid containerPanel
            , int moveStep
            , int columnNumber
            , bool force = true)
        {
            if (!_isChangeDateAnimationFinish) return;

            string oldContentTransformName = "oldContentTransform";
            string newContentTransformName = "newContentTransform";
            var animTime = 300;
            var destination = moveLeft ? -PART_HeaderPanel.ActualWidth : PART_HeaderPanel.ActualWidth;
            destination = destination * moveStep / columnNumber;

            var oldContent = containerPanel.Children[0] as Grid;
            if (oldContent == null || newContent == null) return;

            _isChangeDateAnimationFinish = false;
            containerPanel.Children.Add(newContent);
            try
            {
                containerPanel.UnregisterName(oldContentTransformName);
                containerPanel.UnregisterName(newContentTransformName);
            }
            catch { }

            var oldTransform = new TranslateTransform();
            oldTransform.X = 0;
            containerPanel.RegisterName(oldContentTransformName, oldTransform);
            oldContent.RenderTransform = oldTransform;

            var newTransform = new TranslateTransform();
            newTransform.X = -destination;
            containerPanel.RegisterName(newContentTransformName, newTransform);
            newContent.RenderTransform = newTransform;

            DoubleAnimation leaveAnimation = new DoubleAnimation();
            leaveAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animTime));
            leaveAnimation.From = 0;
            leaveAnimation.To = destination;
            leaveAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
            Storyboard.SetTargetName(leaveAnimation, oldContentTransformName);
            Storyboard.SetTargetProperty(leaveAnimation,
               new PropertyPath(TranslateTransform.XProperty));

            DoubleAnimation enterAnimation = new DoubleAnimation();
            enterAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animTime));
            enterAnimation.From = -destination;
            enterAnimation.To = 0;
            enterAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
            Storyboard.SetTargetName(enterAnimation, newContentTransformName);
            Storyboard.SetTargetProperty(enterAnimation,
               new PropertyPath(TranslateTransform.XProperty));

            Storyboard sb = new Storyboard();
            sb.Children.Add(leaveAnimation);
            sb.Children.Add(enterAnimation);
            sb.Completed += (s, e) =>
            {
                containerPanel.Children.Remove(oldContent);
                _isChangeDateAnimationFinish = true;
            };
            sb.Begin(containerPanel);
        }

        private static void FramingHeaderPanelGridViewColumn(Grid g
            , int columnsNum)
        {
            // Header view generator
            {
                g.ColumnDefinitions.Clear();
                for (int i = 0; i < columnsNum; i++)
                {
                    g.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
                }
            }
        }

        private static void GenerateColumnHeader(Grid g
            , int columnsNum
            , DateTime startTime
            , double headerCornerRadius
            , double headerMarginLeft
            , double headerMarginTop
            , double headerMarginRight
            , double headerMarginBottom
            , double headerContentMarginLeft
            , double headerContentFontSize
            , CalendarNotebookDateMode dateMode)
        {
            string dateTimeFormat = "";
            if (dateMode == CalendarNotebookDateMode.Day)
            {
                dateTimeFormat = columnsNum <= 7 ? "dddd dd" : columnsNum >= 15 ? "dd-M" : "MMM dd";
            }
            else if (dateMode == CalendarNotebookDateMode.Month)
            {
                dateTimeFormat = columnsNum <= 15 ? "MMM yy" : "MM-yy";
            }

            g.Children.Clear();
            for (int i = 1; i <= columnsNum; i++)
            {
                string title = "";

                if (dateMode == CalendarNotebookDateMode.Day)
                {
                    title = startTime.AddDays(i - 1).ToString(dateTimeFormat);
                }
                else if (dateMode == CalendarNotebookDateMode.Month)
                {
                    title = startTime.AddMonths(i - 1).ToString(dateTimeFormat);
                }

                Border headerBorder = new Border()
                {
                    CornerRadius = new CornerRadius(headerCornerRadius),
                    Margin = new Thickness(headerMarginLeft
                        , headerMarginTop
                        , headerMarginRight
                        , headerMarginBottom),
                };
                headerBorder.SetResourceReference(Border.BackgroundProperty, "Background_Level3");
                headerBorder.SetValue(Grid.RowProperty, 0);
                headerBorder.SetValue(Grid.ColumnProperty, i - 1);

                TextBlock headerContent = new TextBlock()
                {
                    Text = title,
                    Margin = new Thickness(columnsNum <= 7 ? headerContentMarginLeft : 0, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = columnsNum <= 7 ? HorizontalAlignment.Stretch : HorizontalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    FontSize = headerContentFontSize,
                };

                headerContent.SetResourceReference(TextBlock.ForegroundProperty, "Foreground_Level3");
                headerBorder.Child = headerContent;
                g.Children.Add(headerBorder);
            }
        }

        #endregion

        #region ContentPanel

        /// <summary>
        /// Cập nhật bề mặt hiển thị của phần nội dung project 
        /// </summary>
        private void UpdateProjectContentPanel(bool isNeedUpdatedDataContent = true)
        {
            if (ItemsSource != null)
            {
                foreach (var context in ItemsSource)
                {
                    var contentGrid = GenerateContextItemView(_sourceCacheManagerMap[context]);
                    context.ItemView = contentGrid;

                    if (isNeedUpdatedDataContent)
                        UpdateSpecificProjectDataContent(_sourceCacheManagerMap[context]
                                , context
                                , StartTime
                                , _currentDisplayedColumnNumber
                                , _viewMode
                                , _dateMode);
                }
            }
        }

        /// <summary>
        /// Cập nhật bề mặt hiển thị của phần nội dung project 
        /// khi tập dữ liệu nguồn thay đổi
        /// </summary>
        /// <param name="oldSource"></param>
        /// <param name="newSource"></param>
        private void UpdateProjectContentPanelWhenSourceChanged(
            ObservableCollection<ICalendarNotebookProjectItemContext>? oldSource
            , ObservableCollection<ICalendarNotebookProjectItemContext>? newSource)
        {
            if (oldSource != null)
            {
                oldSource.CollectionChanged -= HandleProjectSourceCollectionChanged;
                foreach (var context in oldSource)
                {
                    context.CommitSource.KeyCollectionChanged -= HandleKeyCommitCollectionChanged;
                    context.CommitSource.CollectionChanged -= HandleCommitCollectionChanged;
                }
            }

            if (newSource != null)
            {
                foreach (var context in newSource)
                {
                    var cacheManager = new SourceCacheManager(_currentDisplayedColumnNumber + CalendarNotebookListItem.RUNE_STRETCH_COLUMNS);
                    _sourceCacheManagerMap.Add(context, cacheManager);
                    var contentGrid = GenerateContextItemView(_sourceCacheManagerMap[context]);
                    context.ItemView = contentGrid;
                    UpdateSpecificProjectDataContent(_sourceCacheManagerMap[context]
                            , context
                            , StartTime
                            , _currentDisplayedColumnNumber
                            , _viewMode
                            , _dateMode);
                    context.CommitSource.KeyCollectionChanged -= HandleKeyCommitCollectionChanged;
                    context.CommitSource.KeyCollectionChanged += HandleKeyCommitCollectionChanged;

                    context.CommitSource.CollectionChanged -= HandleCommitCollectionChanged;
                    context.CommitSource.CollectionChanged += HandleCommitCollectionChanged;
                }
                newSource.CollectionChanged -= HandleProjectSourceCollectionChanged;
                newSource.CollectionChanged += HandleProjectSourceCollectionChanged;
            }

            PART_ContentListView.ItemsSource = newSource;
        }

        /// <summary>
        /// Xử lý sự kiện khi thêm phần tử mới vào tập collection 
        /// commit của 1 project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleCommitCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                var project = (e.NewItems[0] as ICalendarNotebookCommitItemContext)?.Project;
                if (_viewMode == CalendarNotebookViewMode.Chart
                    && project != null)
                {
                    var sourceCacheManager = _sourceCacheManagerMap[project];
                    sourceCacheManager.SynthesisDataForChart(_currentDisplayedColumnNumber + CalendarNotebookListItem.RUNE_STRETCH_COLUMNS);
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi tập collection commit của 1 project
        /// tạo thêm 1 mục key mới, tức là lúc này dữ liệu phân theo ngày
        /// có thêm 1 ngày mới
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyCommitCollectionChanged(object sender, KeyCollectionChangedEventArgs<ICalendarNotebookCommitItemContext> e)
        {
            var collection = sender as CalendarNotebookItemCollection<ICalendarNotebookCommitItemContext>;

            if (e.ChangedAction == KeyCollectionChangedEventArgs<ICalendarNotebookCommitItemContext>.Action.Add
                && e.NewKey != null
                && e.ChangedItem != null
                && collection != null)
            {
                var project = e.ChangedItem.Project;
                if (_sourceCacheManagerMap.ContainsKey(project))
                {
                    var sourceCacheManager = _sourceCacheManagerMap[project];

                    var index = _dateMode == CalendarNotebookDateMode.Day ?
                        Convert.ToInt32(((e.NewKey ?? _startTimeCache) - _startTimeCache).TotalDays) + 1
                        : Convert.ToInt32(_startTimeCache.TotalMonths(e.NewKey ?? _startTimeCache)) + 1;

                    if (_viewMode == CalendarNotebookViewMode.List)
                    {
                        sourceCacheManager.SetItemContext(index
                            , collection[e.NewKey ?? _startTimeCache, _dateMode]
                                ?? new ObservableCollection<ICalendarNotebookCommitItemContext>());
                    }
                    else if (_viewMode == CalendarNotebookViewMode.Chart)
                    {
                        sourceCacheManager.SetItemContext(index
                            , collection[e.NewKey ?? _startTimeCache, _dateMode] ?? new ObservableCollection<ICalendarNotebookCommitItemContext>()
                            , isNotifyChanged: false);
                    }
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện thêm, sửa, làm mới tập collection project 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleProjectSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (var context in e.NewItems)
                {
                    var cast = context as ICalendarNotebookProjectItemContext;
                    if (cast != null)
                    {
                        var cacheManager = new SourceCacheManager(_currentDisplayedColumnNumber + CalendarNotebookListItem.RUNE_STRETCH_COLUMNS);
                        _sourceCacheManagerMap.Add(cast, cacheManager);
                        var contentGrid = GenerateContextItemView(_sourceCacheManagerMap[cast]);
                        cast.ItemView = contentGrid;
                        UpdateSpecificProjectDataContent(_sourceCacheManagerMap[cast]
                            , cast
                            , StartTime
                            , _currentDisplayedColumnNumber
                            , _viewMode
                            , _dateMode);
                        cast.CommitSource.KeyCollectionChanged -= HandleKeyCommitCollectionChanged;
                        cast.CommitSource.KeyCollectionChanged += HandleKeyCommitCollectionChanged;

                        cast.CommitSource.CollectionChanged -= HandleCommitCollectionChanged;
                        cast.CommitSource.CollectionChanged += HandleCommitCollectionChanged;
                    }
                }
            }
            else if ((e.Action == NotifyCollectionChangedAction.Remove
                || e.Action == NotifyCollectionChangedAction.Reset)
                && e.OldItems != null)
            {
                foreach (var context in e.OldItems)
                {
                    var cast = context as ICalendarNotebookProjectItemContext;
                    if (cast != null)
                    {
                        cast.CommitSource.KeyCollectionChanged -= HandleKeyCommitCollectionChanged;
                        cast.CommitSource.CollectionChanged -= HandleCommitCollectionChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Tạo view hiển thị cho context
        /// </summary>
        /// <param name="sourceCacheManager"></param>
        /// <returns></returns>
        private Grid GenerateContextItemView(SourceCacheManager sourceCacheManager)
        {
            var contentGrid = new Grid();
            FramingGridViewColumn(contentGrid
                        , columnsNum: 1
                        , ProjectColumnWidth
                        , RightGapColumnWidth);
            GenerateProjectItemHeaderAndHorLine(contentGrid
                , columnNumber: 1
                , HeaderItemWidth
                , HeaderItemHeight
                , HeaderItemCornerRad
                , HeaderItemFontSize);

            if (_viewMode == CalendarNotebookViewMode.List)
            {
                GenerateNotebookItemContentForListViewType(contentGrid
                    , sourceCacheManager);
            }
            else if (_viewMode == CalendarNotebookViewMode.Chart)
            {
                GenerateNotebookItemContentForChartViewType(contentGrid
                    , sourceCacheManager);
            }

            return contentGrid;
        }

        /// <summary>
        /// Tạo khung (các column) cho grid 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="columnsNum"></param>
        /// <param name="projectColumnWidth"></param>
        /// <param name="rightGapColumnWidth"></param>
        private static void FramingGridViewColumn(Grid g
            , int columnsNum
            , double projectColumnWidth
            , double rightGapColumnWidth)
        {
            g.ColumnDefinitions.Clear();
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(projectColumnWidth, GridUnitType.Pixel)
            });
            for (int i = 0; i < columnsNum; i++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(rightGapColumnWidth, GridUnitType.Pixel)
            });
        }

        /// <summary>
        /// Tạo thanh ngang ngăn giữa các item
        /// và tiêu đề cho mỗi item trong 1 grid cho trước
        /// </summary>
        /// <param name="g"></param>
        /// <param name="columnNumber"></param>
        /// <param name="headerItemWidth"></param>
        /// <param name="headerItemHeight"></param>
        /// <param name="headerItemCornerRad"></param>
        /// <param name="headerItemFontSize"></param>
        private static void GenerateProjectItemHeaderAndHorLine(Grid g
            , int columnNumber
            , double headerItemWidth
            , double headerItemHeight
            , double headerItemCornerRad
            , double headerItemFontSize)
        {
            g.RowDefinitions.Clear();
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Pixel) });
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            // Hor line
            {
                Rectangle horLine = new Rectangle()
                {
                    Height = 1,
                };
                horLine.SetResourceReference(Rectangle.FillProperty, "Foreground_Level2");
                horLine.SetValue(Grid.RowProperty, 0);
                horLine.SetValue(Grid.ColumnSpanProperty, columnNumber + 2);

                g.Children.Add(horLine);
            }

            // Header content
            {
                Border headerBorder = new Border()
                {
                    Width = headerItemWidth,
                    Height = headerItemHeight,
                    CornerRadius = new CornerRadius(headerItemCornerRad)
                };
                headerBorder.SetResourceReference(Border.BackgroundProperty, "ButtonBackground_Level1");
                headerBorder.SetValue(Grid.RowProperty, 1);
                TextBlock headerContent = new TextBlock()
                {
                    FontSize = headerItemFontSize,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    LayoutTransform = new RotateTransform(270),
                };
                headerContent.SetResourceReference(TextBlock.ForegroundProperty, "Foreground_Level3");
                Binding textBinding = new Binding("ProjectName") { FallbackValue = "HoneyBoard" };
                headerContent.SetBinding(TextBlock.TextProperty, textBinding);
                headerBorder.Child = headerContent;

                g.Children.Add(headerBorder);
            }

        }

        /// <summary>
        /// Tạo nội dung cho item với kiểu hiển thị là danh sách (listview)
        /// </summary>
        /// <param name="contentPanel"></param>
        /// <param name="columnNumber"></param>
        /// <param name="cacheManager"></param>
        /// <param name="itemCornerRad"></param>
        /// <param name="itemMargin"></param>
        /// <param name="itemFontSize"></param>
        private static void GenerateNotebookItemContentForListViewType(Grid contentPanel
            , SourceCacheManager cacheManager)
        {
            if (contentPanel != null)
            {
                Binding itemSourceBinding = new Binding();
                itemSourceBinding.Source = cacheManager;
                itemSourceBinding.Path = new PropertyPath("ListViewSource");
                itemSourceBinding.Mode = BindingMode.TwoWay;
                itemSourceBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                var listViewItem = new CalendarNotebookListItem();
                listViewItem.SetValue(Grid.RowProperty, 1);
                listViewItem.SetValue(Grid.ColumnProperty, 1);
                listViewItem.SetBinding(CalendarNotebookListItem.ItemsSourceProperty, itemSourceBinding);

                contentPanel.Children.Add(listViewItem);
            }
        }

        /// <summary>
        /// Tạo nội dung cho item với kiểu hiển thị là biểu đồ (chartview)
        /// </summary>
        /// <param name="contentPanel"></param>
        /// <param name="cacheManager"></param>
        private static void GenerateNotebookItemContentForChartViewType(Grid contentPanel
          , SourceCacheManager cacheManager)
        {
            if (contentPanel != null)
            {
                Binding itemSourceBinding = new Binding();
                itemSourceBinding.Source = cacheManager;
                itemSourceBinding.Path = new PropertyPath("ProjectDataForChart");
                itemSourceBinding.Mode = BindingMode.OneWay;
                itemSourceBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                Binding maxPointScoreBinding = new Binding();
                maxPointScoreBinding.Source = cacheManager;
                maxPointScoreBinding.Path = new PropertyPath("ChartViewMaxPointScore");
                maxPointScoreBinding.Mode = BindingMode.OneWay;
                maxPointScoreBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                Binding updatingNotificationBinding = new Binding();
                updatingNotificationBinding.Source = cacheManager;
                updatingNotificationBinding.Path = new PropertyPath("ChartViewUpdatingNotificationCounter");
                updatingNotificationBinding.Mode = BindingMode.OneWay;
                updatingNotificationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                var chartItem = new CalendarNotebookChartItem()
                {
                };

                chartItem.SetBinding(CalendarNotebookChartItem.DataProperty, itemSourceBinding);
                chartItem.SetBinding(CalendarNotebookChartItem.MaxPointScoreProperty, maxPointScoreBinding);
                chartItem.SetBinding(CalendarNotebookChartItem.UpdatingNotificationProperty, updatingNotificationBinding);

                var grid = new Grid()
                {
                    Margin = new Thickness(0, 5, 0, 5),
                };
                grid.SetValue(Grid.RowProperty, 1);
                grid.SetValue(Grid.ColumnProperty, 1);
                grid.Children.Add(chartItem);

                contentPanel.Children.Add(grid);
            }
        }
        #endregion

        #region Data manager

        /// <summary>
        /// Cập nhật kích thước tệp dữ liệu hiển thị cho tất cả project
        /// </summary>
        /// <param name="sizeChanged"></param>
        private void ExtendSizeForAllProject(int sizeChanged)
        {
            if (ItemsSource != null)
            {
                foreach (var projectCtx in ItemsSource)
                {
                    var sourceCacheManager = _sourceCacheManagerMap[projectCtx];
                    if (sourceCacheManager != null)
                    {
                        sourceCacheManager.ExtendListViewSize(sizeChanged);
                    }
                }
            }
        }

        /// <summary>
        /// Cập nhật dữ liệu trong source cache manager cho toàn bộ project
        /// trong tập collection project
        /// </summary>
        /// <param name="start"></param>
        /// <param name="force"></param>
        private void UpdateAllProjectDataContent(bool force = true
            , bool isRefreshChartMaxPointScore = false)
        {
            if (!force)
            {
                return;
            }

            if (ItemsSource != null)
            {
                foreach (var projectCtx in ItemsSource)
                {
                    var sourceCacheManager = _sourceCacheManagerMap[projectCtx];
                    if (sourceCacheManager != null)
                    {
                        UpdateSpecificProjectDataContent(sourceCacheManager
                            , projectCtx
                            , StartTime
                            , _currentDisplayedColumnNumber
                            , _viewMode
                            , _dateMode
                            , isRefreshChartMaxPointScore);
                    }
                }
            }
        }

        /// <summary>
        /// Cập nhật dữ liệu trong source cache manager cho 1 project 
        /// cụ thể có trong collection project
        /// </summary>
        /// <param name="sourceCacheManager"></param>
        /// <param name="projectContext"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="viewMode"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void UpdateSpecificProjectDataContent(SourceCacheManager sourceCacheManager
            , ICalendarNotebookProjectItemContext projectContext
            , DateTime start
            , int columnNumber
            , CalendarNotebookViewMode viewMode
            , CalendarNotebookDateMode dateMode
            , bool isRefreshChartMaxPointScore = false)
        {
            if (isRefreshChartMaxPointScore)
            {
                sourceCacheManager.ResetChartMaxPointScore();
            }
            sourceCacheManager.UpdateListViewSize(columnNumber + CalendarNotebookListItem.RUNE_STRETCH_COLUMNS);

            for (int i = 0; i < columnNumber + CalendarNotebookListItem.RUNE_STRETCH_COLUMNS; i++)
            {
                var key = dateMode == CalendarNotebookDateMode.Day ?
                    start.AddDays(i - 1) : start.AddMonths(i - 1);
                var source = projectContext.CommitSource[key, dateMode]
                    ?? new ObservableCollection<ICalendarNotebookCommitItemContext>();
                sourceCacheManager.SetItemContext(i, source, false);
            }

            if (viewMode == CalendarNotebookViewMode.List)
            {
                sourceCacheManager.NotifyCacheChanged();
            }
            else if (viewMode == CalendarNotebookViewMode.Chart)
            {
                sourceCacheManager.SynthesisDataForChart(columnNumber + CalendarNotebookListItem.RUNE_STRETCH_COLUMNS);
            }
        }
        #endregion

        /// <summary>
        /// Xử lý sự kiện nhấn nút trên calendar notebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleButtonAndMenuItemClick(object sender, RoutedEventArgs e)
        {
            var ctrl = sender as Button;
            if (ctrl != null)
            {
                Debug.WriteLine(ctrl.Name);
                switch (ctrl.Name)
                {
                    case "PART_DecreaseTimeButton":
                        {
                            if (!_isChangeDateAnimationFinish) return;
                            _isNeedUpdateWhenDisplayDateTimePropertyChanged = false;
                            if (_dateMode == CalendarNotebookDateMode.Day)
                            {
                                StartTime = StartTime.AddDays(-_movingColumnNumberArray[_movingColumnNumberIndex]);
                                EndTime = EndTime.AddDays(-_movingColumnNumberArray[_movingColumnNumberIndex]);
                            }
                            else if (_dateMode == CalendarNotebookDateMode.Month)
                            {
                                StartTime = StartTime.AddMonths(-_movingColumnNumberArray[_movingColumnNumberIndex]);
                                EndTime = EndTime.AddMonths(-_movingColumnNumberArray[_movingColumnNumberIndex]);
                            }
                            _isNeedUpdateWhenDisplayDateTimePropertyChanged = true;
                            ChangeDisplayDateTime(moveLeft: false);
                            UpdateAllProjectDataContent();
                            break;
                        }
                    case "PART_IncreaseTimeButton":
                        {
                            if (!_isChangeDateAnimationFinish) return;
                            _isNeedUpdateWhenDisplayDateTimePropertyChanged = false;
                            if (_dateMode == CalendarNotebookDateMode.Day)
                            {
                                StartTime = StartTime.AddDays(_movingColumnNumberArray[_movingColumnNumberIndex]);
                                EndTime = EndTime.AddDays(_movingColumnNumberArray[_movingColumnNumberIndex]);
                            }
                            else if (_dateMode == CalendarNotebookDateMode.Month)
                            {
                                StartTime = StartTime.AddMonths(_movingColumnNumberArray[_movingColumnNumberIndex]);
                                EndTime = EndTime.AddMonths(_movingColumnNumberArray[_movingColumnNumberIndex]);
                            }
                            _isNeedUpdateWhenDisplayDateTimePropertyChanged = true;
                            ChangeDisplayDateTime(moveLeft: true);
                            UpdateAllProjectDataContent();
                            break;
                        }
                    case "PART_MenuButton":
                        {
                            PART_ControlPanelContextMenu.IsOpen = true;
                            break;
                        }
                    case "PART_IncreaseMovingColumnNumberButton":
                        {
                            _movingColumnNumberIndex++;
                            if (_movingColumnNumberIndex >= _movingColumnNumberArray.Length)
                            {
                                _movingColumnNumberIndex = 0;
                            }
                            PART_MovingColumnNumberMenuItem.Header = _movingColumnNumberArray[_movingColumnNumberIndex] + " columns";
                            break;
                        }
                    case "PART_DecreaseMovingColumnNumberButton":
                        {
                            _movingColumnNumberIndex--;
                            if (_movingColumnNumberIndex < 0)
                            {
                                _movingColumnNumberIndex = _movingColumnNumberArray.Length - 1;
                            }
                            PART_MovingColumnNumberMenuItem.Header = _movingColumnNumberArray[_movingColumnNumberIndex] + " columns";
                            break;
                        }
                    case "PART_IncreaseColumnNumberButton":
                        {
                            if (_currentDisplayedColumnNumber < RUNE_MAXIMUM_DISPLAYED_COLUMN_NUMBER)
                            {
                                ExtendSizeForAllProject(1);
                                if (_dateMode == CalendarNotebookDateMode.Day)
                                {
                                    EndTime = EndTime.AddDays(1);
                                }
                                else if (_dateMode == CalendarNotebookDateMode.Month)
                                {
                                    EndTime = EndTime.AddMonths(1);
                                }
                            }
                            break;
                        }
                    case "PART_DecreaseColumnNumberButton":
                        {
                            if (_currentDisplayedColumnNumber > RUNE_MINIMUM_DISPLAYED_COLUMN_NUMBER)
                            {
                                ExtendSizeForAllProject(-1);
                                if (_dateMode == CalendarNotebookDateMode.Day)
                                {
                                    EndTime = EndTime.AddDays(-1);
                                }
                                else if (_dateMode == CalendarNotebookDateMode.Month)
                                {
                                    EndTime = EndTime.AddMonths(-1);
                                }
                            }
                            break;
                        }
                }
                return;
            }

            var menuItem = sender as System.Windows.Controls.MenuItem;

            if (menuItem != null)
            {
                switch (menuItem.Name)
                {
                    case "PART_ListViewTypeMenuItem":
                    case "PART_ChartViewTypeMenuItem":
                        HandleViewTypeMenuItemClick(menuItem);
                        break;
                    case "PART_DayModeMenuItem":
                    case "PART_MonthModeMenuItem":
                        HandleDateModeMenuItemClick(menuItem);
                        break;
                }

            }
        }

        /// <summary>
        /// Xử lý sự kiện chọn kiểu hiển thị datemode
        /// </summary>
        /// <param name="selectedViewTypeItem"></param>
        private void HandleDateModeMenuItemClick(System.Windows.Controls.MenuItem selectedViewTypeItem)
        {
            var oldMode = _dateMode;
            switch (selectedViewTypeItem.Name)
            {
                case "PART_DayModeMenuItem":
                    _dateMode = CalendarNotebookDateMode.Day;
                    break;
                case "PART_MonthModeMenuItem":
                    _dateMode = CalendarNotebookDateMode.Month;
                    break;
            }

            if (oldMode == _dateMode)
            {
                return;
            }

            _isNeedUpdateWhenDisplayDateTimePropertyChanged = false;
            if (_dateMode == CalendarNotebookDateMode.Day)
            {
                StartTime = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                EndTime = DateTime.Now.EndOfWeek(DayOfWeek.Sunday);
            }
            else if (_dateMode == CalendarNotebookDateMode.Month)
            {
                StartTime = DateTime.Now.StartOfYear();
                EndTime = DateTime.Now.EndOfYear();
            }
            _isNeedUpdateWhenDisplayDateTimePropertyChanged = true;
            UpdateDisplayTimeOfHeaderPanel();
            UpdateAllProjectDataContent(force: true, isRefreshChartMaxPointScore: true);
            UpdateProjectContentPanel(isNeedUpdatedDataContent: false);

            switch (oldMode)
            {
                case CalendarNotebookDateMode.Day:
                    (PART_DayModeMenuItem.Icon as Path)?
                        .SetResourceReference(Path.FillProperty, "Background_Level2");
                    break;
                case CalendarNotebookDateMode.Month:
                    (PART_MonthModeMenuItem.Icon as Path)?
                        .SetResourceReference(Path.FillProperty, "Background_Level2");
                    break;
            }
            var pathIcon = selectedViewTypeItem.Icon as Path;
            pathIcon?.SetResourceReference(Path.FillProperty, "ButtonBackground_Level1");
        }

        /// <summary>
        /// Xử lý sự kiện chọn kiểu hiển thị notebook
        /// </summary>
        /// <param name="selectedViewTypeItem"></param>
        private void HandleViewTypeMenuItemClick(System.Windows.Controls.MenuItem selectedViewTypeItem)
        {
            var oldMode = _viewMode;

            switch (selectedViewTypeItem.Name)
            {
                case "PART_ListViewTypeMenuItem":
                    _viewMode = CalendarNotebookViewMode.List;
                    break;
                case "PART_ChartViewTypeMenuItem":
                    _viewMode = CalendarNotebookViewMode.Chart;
                    break;
            }

            if (oldMode == _viewMode)
            {
                return;
            }

            UpdateProjectContentPanel();

            switch (oldMode)
            {
                case CalendarNotebookViewMode.List:
                    (PART_ListViewTypeMenuItem.Icon as Path)?
                        .SetResourceReference(Path.FillProperty, "Background_Level2");
                    break;
                case CalendarNotebookViewMode.Chart:
                    (PART_ChartViewTypeMenuItem.Icon as Path)?
                        .SetResourceReference(Path.FillProperty, "Background_Level2");
                    break;
            }
            var pathIcon = selectedViewTypeItem.Icon as Path;
            pathIcon?.SetResourceReference(Path.FillProperty, "ButtonBackground_Level1");
        }

        /// <summary>
        /// Xử lý sự kiện khi context menu đã được mở hiển thị lên
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleControlPanelContextMenuOpened(object sender, RoutedEventArgs e)
        {
            PART_MovingColumnNumberMenuItem.Header = "Moving column";
        }

    }
}
