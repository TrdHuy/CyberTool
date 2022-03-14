using LogGuard_v0._1.LogGuard.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LogGuard_v0._1.LogGuard.Control
{
    public class LogMappingSizeManager
    {
        public float MapHeight { get; internal set; }
        public float MapWidth { get; internal set; }
        public float PointHeight { get; internal set; }

        public int CurrentStartIndex { get; set; } = -1;
        public int CurrentEndIndex { get; set; } = -1;
    }

    internal class LogWatcherSourceManager
    {
        public int DrawingItemNumber { get; set; } = (int)LogWatcher.DrawingItemsNumberProperty.DefaultMetadata.DefaultValue;
        public int CurrentSourceCount { get; set; }
        public int VisibleItemsStartIndex { get; internal set; }
        public int VisibleItemsCount { get; internal set; }
        private IEnumerable<ILogWatcherElements> CacheElements { get; set; }

        public LogWatcherSourceManager()
        {
        }

        public void SetCacheElements(IEnumerable newVal)
        {
            CacheElements = newVal.OfType<ILogWatcherElements>();
            CurrentSourceCount = CacheElements.Count();
        }

        public void HandleLogWatcherItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                CurrentSourceCount += e.NewItems.Count;
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                CurrentSourceCount = 0;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                CurrentSourceCount -= e.OldItems.Count;
            }
        }

        public object[] GetCopiedItemsCollection(ItemCollection items, int index, int total)
        {
            var allowItems = total <= DrawingItemNumber ? total : DrawingItemNumber;
            var newArr = new object[allowItems];

            IList cast = (IList)items;
            int j = 0;
            for (int i = index; i < total + index; i++)
            {
                newArr[j] = cast[i];
                j++;
            }
            return newArr;
        }

        public object[] GetCopiedItemsCollection2(ItemCollection items, int index, int total)
        {
            var allowItems = total <= DrawingItemNumber ? total : DrawingItemNumber;
            var newArr = new object[allowItems];
            try
            {
                items.CopyTo(newArr, index);
            }
            catch
            {

            }
            finally
            {
            }
            return newArr;
        }

        public int GetStartIndexToCopyItemCollection(int itemCount, int allowedItemNumber)
        {
            var middleDisplayIndex = VisibleItemsStartIndex + VisibleItemsCount / 2;

            var halfRight = allowedItemNumber / 2;
            var halfLeft = allowedItemNumber / 2 + allowedItemNumber % 2;

            if (middleDisplayIndex + halfRight > itemCount - 1)
            {
                var newHalfRight = itemCount - 1 - middleDisplayIndex;
                var newHalfLeft = halfRight - newHalfRight + halfLeft;
                var startIndex = middleDisplayIndex - newHalfLeft + 1;
                return startIndex < 0 ? 0 : startIndex;
            }
            else if (middleDisplayIndex - halfLeft < 0)
            {
                return 0;
            }

            return middleDisplayIndex - halfLeft;
        }

        public int GetCurrentDrawingItemsCount(int itemCount)
        {
            return itemCount <= DrawingItemNumber ? itemCount : DrawingItemNumber;
        }
    }

    [TemplatePart(Name = LogWatcher.RootBorderName, Type = typeof(Border))]
    [TemplatePart(Name = LogWatcher.BookmarkMappingBorderName, Type = typeof(LogMappingBorder))]
    [TemplatePart(Name = LogWatcher.LevelMappingBorderName, Type = typeof(LogMappingBorder))]
    [TemplatePart(Name = LogWatcher.ErrorMappingBorderName, Type = typeof(LogMappingBorder))]
    [TemplatePart(Name = LogWatcher.MainScrollViewerName, Type = typeof(ScrollViewWatcher))]
    public class LogWatcher : ListView
    {
        private const string RootBorderName = "Border";
        private const string MainScrollViewerName = "PART_MainScrollViewer";
        private const string BookmarkMappingBorderName = "PART_BookmarkMappingBorder";
        private const string LevelMappingBorderName = "PART_LevelMappingBorder";
        private const string ErrorMappingBorderName = "PART_ErrorMappingBorder";

        #region ExtrusionZoneBackgroundStyle
        public static readonly DependencyProperty ExtrusionZoneBackgroundStyleProperty =
                DependencyProperty.Register(
                        "ExtrusionZoneBackgroundStyle",
                        typeof(Style),
                        typeof(LogWatcher),
                        new PropertyMetadata(
                                default(Style),
                                new PropertyChangedCallback(ExtrusionZoneBackgroundStylePropertyCallback)));

        private static void ExtrusionZoneBackgroundStylePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public Style ExtrusionZoneBackgroundStyle
        {
            get { return (Style)GetValue(ExtrusionZoneBackgroundStyleProperty); }
            set { SetValue(ExtrusionZoneBackgroundStyleProperty, value); }
        }
        #endregion

        /// <summary>
        /// Do not bind directly to ItemsSource, bind the source to this property instead.
        /// In order to clear the event handler for Items collection of ListView
        /// </summary>
        #region LogWatcherItemsSource
        public static readonly DependencyProperty LogWatcherItemsSourceProperty
            = DependencyProperty.Register("LogWatcherItemsSource", typeof(IEnumerable), typeof(LogWatcher),
                                          new PropertyMetadata(default(IEnumerable), new PropertyChangedCallback(LogWatcherItemsSourceChangedCallback))
                                              , new ValidateValueCallback(LogWatcherItemsSourceValidateValueCallBack));

        private static bool LogWatcherItemsSourceValidateValueCallBack(object value)
        {
            if (value == null)
            {
                return true;
            }
            IEnumerable newValue = (IEnumerable)value;
            var asEnumerable = newValue as IEnumerable<ILogWatcherElements>;
            return asEnumerable != null;
        }

        private static void LogWatcherItemsSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LogWatcher ic = (LogWatcher)d;
            IEnumerable newValue = (IEnumerable)e.NewValue;
            ic.PreHandleLogWatcherItemsSourceChange(newValue);
        }


        public IEnumerable LogWatcherItemsSource
        {
            get { return (IEnumerable)GetValue(LogWatcherItemsSourceProperty); }
            set
            {
                if (value == null)
                {
                    ClearValue(LogWatcherItemsSourceProperty);
                }
                else
                {
                    SetValue(LogWatcherItemsSourceProperty, value);
                }
            }
        }
        #endregion

        #region UseAutoScroll
        public static readonly DependencyProperty UseAutoScrollProperty =
                DependencyProperty.Register(
                        "UseAutoScroll",
                        typeof(bool),
                        typeof(LogWatcher),
                        new PropertyMetadata(
                                false,
                                new PropertyChangedCallback(UseAutoScrollChangedCallback)),
                        null);

        private static void UseAutoScrollChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lw = d as LogWatcher;
            if (lw == null)
                return;
            lw.UseAutoScrollCache = (bool)e.NewValue;
        }

        public bool UseAutoScroll
        {
            get { return (bool)GetValue(UseAutoScrollProperty); }
            set { SetValue(UseAutoScrollProperty, value); }
        }
        #endregion

        /// <summary>
        /// Number of items will be drawn on the mapper
        /// </summary>
        #region DrawingItemsNumber
        public static readonly DependencyProperty DrawingItemsNumberProperty =
                DependencyProperty.Register(
                        "DrawingItemsNumber",
                        typeof(int),
                        typeof(LogWatcher),
                        new PropertyMetadata(
                                200, new PropertyChangedCallback(DrawingItemsNumberPropertyChangedCallback)),
                        new ValidateValueCallback(ValidateDrawingItemsNumberPropertyCallback));

        private static bool ValidateDrawingItemsNumberPropertyCallback(object value)
        {
            var con = Convert.ToInt32(value);
            return con > 0;
        }

        private static void DrawingItemsNumberPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lw = d as LogWatcher;
            lw.SourceManager.DrawingItemNumber = (int)e.NewValue;
        }

        public int DrawingItemsNumber
        {
            get { return (int)GetValue(DrawingItemsNumberProperty); }
            set { SetValue(DrawingItemsNumberProperty, value); }
        }
        #endregion

        /// <summary>
        /// Do not use this ItemsSource property
        /// bind the source to LogWatcherItemsSource property instead.
        /// </summary>
        public new IEnumerable ItemsSource { get; private set; }

        public event LogMapperDrawingHandler LevelMapperDrawing;
        public event LogMapperDrawingHandler ErrorMapperDrawing;
        public event LogMapperDrawingHandler BookmarkMapperDrawing;

        public LogWatcher()
        {
            DefaultStyleKey = typeof(LogWatcher);
            DrawingMapSemaphore = new SemaphoreSlim(1);
            MappingManager = new LogMappingSizeManager();
            SourceManager = new LogWatcherSourceManager();
        }

        private bool UseAutoScrollCache { get; set; }
        private SemaphoreSlim DrawingMapSemaphore { get; set; }
        private LogMappingSizeManager MappingManager { get; set; }
        private LogWatcherSourceManager SourceManager { get; set; }
        private Dictionary<string, Color> DefaultMapperColors { get; set; }

        private ScrollViewWatcher MainScrollViewer;
        private LogMappingBorder BookmarkMappingBorder;
        private LogMappingBorder LevelMappingBorder;
        private LogMappingBorder ErrorMappingBorder;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitColorMapper();

            MainScrollViewer = GetTemplateChild(MainScrollViewerName) as ScrollViewWatcher;
            BookmarkMappingBorder = GetTemplateChild(BookmarkMappingBorderName) as LogMappingBorder;
            LevelMappingBorder = GetTemplateChild(LevelMappingBorderName) as LogMappingBorder;
            ErrorMappingBorder = GetTemplateChild(ErrorMappingBorderName) as LogMappingBorder;

            if (BookmarkMappingBorder != null)
            {
                var magRadius = (double)LogMappingBorder.GetMagnifierRadius(this);
                LogMappingBorder.SetMagnifierRadius(BookmarkMappingBorder, magRadius);
                LogMappingBorder.SetMagnifierRadius(LevelMappingBorder, magRadius);
                LogMappingBorder.SetMagnifierRadius(ErrorMappingBorder, magRadius);

                BookmarkMappingBorder.SetLogMappingManager(MappingManager);
                LevelMappingBorder.SetLogMappingManager(MappingManager);
                ErrorMappingBorder.SetLogMappingManager(MappingManager);

                BookmarkMappingBorder.SizeChanged += HandleLogWatcherMappingBorderSizeChanged;

                LevelMappingBorder.MouseDown += LevelMappingBorder_MouseDown;
                ErrorMappingBorder.MouseDown += ErrorMappingBorder_MouseDown;
            }

            Loaded += LogWatcher_Loaded;
        }

        private void InitColorMapper()
        {
            DefaultMapperColors = new Dictionary<string, Color>();

            var D = Style.Resources["DebugLevelForeground"] as System.Windows.Media.SolidColorBrush;
            var F = Style.Resources["FatalLevelForeground"] as System.Windows.Media.SolidColorBrush;
            var V = Style.Resources["VerboseLevelForeground"] as System.Windows.Media.SolidColorBrush;
            var I = Style.Resources["InfoLevelForeground"] as System.Windows.Media.SolidColorBrush;
            var W = Style.Resources["WarningLevelForeground"] as System.Windows.Media.SolidColorBrush;
            var E = Style.Resources["ErrorLevelForeground"] as System.Windows.Media.SolidColorBrush;

            DefaultMapperColors.Add("F", F == null ? Color.FromArgb(252, 133, 255) :
                Color.FromArgb(F.Color.A,
                F.Color.R,
                F.Color.G,
                F.Color.B));

            DefaultMapperColors.Add("V", V == null ? Color.FromArgb(109, 255, 249) :
                Color.FromArgb(V.Color.A,
                V.Color.R,
                V.Color.G,
                V.Color.B));

            DefaultMapperColors.Add("I", I == null ? Color.FromArgb(255, 255, 255) :
                Color.FromArgb(I.Color.A,
                I.Color.R,
                I.Color.G,
                I.Color.B));

            DefaultMapperColors.Add("E", E == null ? Color.FromArgb(252, 98, 85) :
                Color.FromArgb(E.Color.A,
                E.Color.R,
                E.Color.G,
                E.Color.B));

            DefaultMapperColors.Add("W", W == null ? Color.FromArgb(255, 184, 85) :
                Color.FromArgb(W.Color.A,
                W.Color.R,
                W.Color.G,
                W.Color.B));

            DefaultMapperColors.Add("D", D == null ? Color.FromArgb(173, 255, 85) :
                Color.FromArgb(D.Color.A,
                D.Color.R,
                D.Color.G,
                D.Color.B));
        }

        private void LogWatcher_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainScrollViewer != null)
            {
                ScrollBar scrollBar = MainScrollViewer.Template.FindName("PART_VerticalScrollBar", MainScrollViewer) as ScrollBar;
                if (scrollBar != null)
                {
                    scrollBar.ValueChanged += delegate
                    {
                        SourceManager.VisibleItemsStartIndex = (int)MainScrollViewer.VerticalOffset;
                        SourceManager.VisibleItemsCount = (int)MainScrollViewer.ViewportHeight;
                        HandleDrawLogWatcherMap();
                    };
                }
            }
        }

        internal void HanldleLogWatcherMagnifierRadiusChanged()
        {
            if (IsLoaded)
            {
                var magRadius = (double)LogMappingBorder.GetMagnifierRadius(this);
                LogMappingBorder.SetMagnifierRadius(BookmarkMappingBorder, magRadius);
                LogMappingBorder.SetMagnifierRadius(LevelMappingBorder, magRadius);
                LogMappingBorder.SetMagnifierRadius(ErrorMappingBorder, magRadius);
            }
        }

        private void ErrorMappingBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ErrorMappingBorder.SelectedIndex != -1)
            {
                SelectedIndex = ErrorMappingBorder.SelectedIndex;
                if (SelectedIndex >= 0)
                {
                    ScrollIntoView(Items[SelectedIndex]);
                }
            }
        }

        private void LevelMappingBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LevelMappingBorder.SelectedIndex != -1)
            {
                SelectedIndex = LevelMappingBorder.SelectedIndex;
                if (SelectedIndex >= 0)
                {
                    ScrollIntoView(Items[SelectedIndex]);
                }
            }
        }

        private void HandleLogWatcherMappingBorderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            MappingManager.MapHeight = (float)(e.NewSize.Height - BookmarkMappingBorder.BorderThickness.Top - BookmarkMappingBorder.BorderThickness.Bottom);
            MappingManager.MapWidth = (float)(e.NewSize.Width - BookmarkMappingBorder.BorderThickness.Left - BookmarkMappingBorder.BorderThickness.Right);
            MappingManager.PointHeight = MappingManager.MapHeight / Items.Count;
            HandleDrawLogWatcherMap(forceDraw: true);
        }

        protected virtual void PreHandleLogWatcherItemsSourceChange(IEnumerable newValue)
        {
            SourceManager.SetCacheElements(newValue);

            var oldVal = Items as INotifyCollectionChanged;
            if (oldVal != null)
            {
                oldVal.CollectionChanged -= HandleLogWatcherItemsCollectionChanged;
            }

            base.ItemsSource = newValue;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            var oldISVal = oldValue as INotifyCollectionChanged;
            if (oldISVal != null)
            {
                oldISVal.CollectionChanged -= SourceManager.HandleLogWatcherItemsSourceCollectionChanged;
            }

            var newISVal = newValue as INotifyCollectionChanged;
            if (newISVal != null)
            {
                newISVal.CollectionChanged += SourceManager.HandleLogWatcherItemsSourceCollectionChanged;
            }

            var newVal = Items as INotifyCollectionChanged;
            if (newVal != null)
            {
                newVal.CollectionChanged += HandleLogWatcherItemsCollectionChanged;
            }


            HandleDrawLogWatcherMap();
        }

        private void HandleLogWatcherItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HandleAutoScrollDown();
        }

        private void HandleDrawLogWatcherMap(bool forceDraw = false)
        {

            if (DrawingMapSemaphore.CurrentCount == 0
                || MappingManager.PointHeight == 0)
            {
                return;
            }

            try
            {
                var itemCount = Items.Count;
                int allowedDrawingItemsCount = SourceManager.GetCurrentDrawingItemsCount(itemCount);
                int startCopiedIndex = SourceManager.GetStartIndexToCopyItemCollection(itemCount, allowedDrawingItemsCount);

                if (MappingManager.CurrentStartIndex == startCopiedIndex
                    && !forceDraw)
                {
                    return;
                }

                MappingManager.CurrentStartIndex = startCopiedIndex;
                MappingManager.CurrentEndIndex = startCopiedIndex + allowedDrawingItemsCount - 1;

                MappingManager.PointHeight = MappingManager.MapHeight / allowedDrawingItemsCount;
                var copiedItems = SourceManager.GetCopiedItemsCollection(Items, startCopiedIndex, allowedDrawingItemsCount);

                Task.Run(() =>
                {
                    DrawingMapSemaphore.Wait();

                    try
                    {
                        Stopwatch st = Stopwatch.StartNew();
                        var mapHeight =
                            Convert.ToInt32(MappingManager.MapHeight);
                        var mapWidth =
                            Convert.ToInt32(MappingManager.MapWidth);
                        System.Drawing.Bitmap levelMapBmp = new System.Drawing.Bitmap(mapWidth, mapHeight);
                        System.Drawing.Graphics levelMapGraphics = System.Drawing.Graphics.FromImage(levelMapBmp);

                        System.Drawing.Bitmap errMapBmp = new System.Drawing.Bitmap(mapWidth, mapHeight);
                        System.Drawing.Graphics errMapGraphics = System.Drawing.Graphics.FromImage(errMapBmp);

                        System.Drawing.Bitmap bookMarkMapBmp = new System.Drawing.Bitmap(mapWidth, mapHeight);
                        System.Drawing.Graphics bookMarkMapGraphics = System.Drawing.Graphics.FromImage(bookMarkMapBmp);

                        var lvDrawArg = new LogMapperDrawingEventArgs(levelMapBmp
                           , levelMapGraphics
                           , copiedItems
                           , MappingManager);
                        LevelMapperDrawing?.Invoke(this, lvDrawArg);

                        var errDrawArg = new LogMapperDrawingEventArgs(errMapBmp
                            , errMapGraphics
                            , copiedItems
                            , MappingManager);
                        ErrorMapperDrawing?.Invoke(this, errDrawArg);

                        BookmarkMapperDrawing?.Invoke(this,
                           new LogMapperDrawingEventArgs(bookMarkMapBmp
                           , bookMarkMapGraphics
                           , copiedItems
                           , MappingManager));

                        if (!lvDrawArg.Handled)
                            DefautMapperDraw(allowedDrawingItemsCount, copiedItems, levelMapGraphics);

                        if (!errDrawArg.Handled)
                            DefautMapperDraw(allowedDrawingItemsCount, copiedItems, errMapGraphics, true);


                        var lvBmpI = levelMapBmp.ToBitmapImage();
                        var errBmpI = errMapBmp.ToBitmapImage();


                        App.Current.Dispatcher.Invoke(() =>
                        {
                            LevelMappingBorder.Source = lvBmpI;
                            ErrorMappingBorder.Source = errBmpI;
                        });

                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        DrawingMapSemaphore.Release();
                    }



                });
            }
            catch (Exception e)
            {

            }

        }

        private void DefautMapperDraw(int count, object[] copiedItems, Graphics graphic, bool isErrorMap = false)
        {
            float yOffset = 0f;
            for (int i = 0; i < count; i++)
            {
                ILogWatcherElements lwe = null;
                lwe = copiedItems[i] as ILogWatcherElements;

                Brush br = new SolidBrush(DefaultMapperColors["I"]);

                if (lwe != null)
                {
                    if (!string.IsNullOrEmpty(lwe.Level))
                    {
                        if (isErrorMap)
                        {
                            br = new SolidBrush(DefaultMapperColors[lwe.Level == "E" ? "E" : "I"]);
                        }
                        else
                        {
                            br = new SolidBrush(DefaultMapperColors[lwe.Level]);
                        }
                    }
                }

                graphic.FillRectangle(br, 0f, yOffset, MappingManager.MapWidth, MappingManager.PointHeight);
                yOffset += MappingManager.PointHeight;
            }
        }


        private Random ColorRandom = new Random();

        private void HandleAutoScrollDown()
        {
            if (UseAutoScrollCache)
            {
                try
                {
                    /// Khi số lượng log thực tế lớn hơn rất nhiều so với số lượng log
                    /// được add vào để hiển thị lên, ta cần phải tạm dừng scroll bottom 
                    /// để cho lượng log chưa được hiển thị có thời gian add vào trong bộ
                    /// collection Items
                    if (SourceManager.CurrentSourceCount > Items.Count)
                    {
                        if (Items.Count % 20000 < 10)
                        {
                            ScrollToBottom();
                        }
                    }
                    else
                    {
                        ScrollToBottom();
                    }
                }
                catch
                {

                }


            }
        }

        private void ScrollToBottom()
        {
            SelectedIndex = Items.Count - 1;
            if (SelectedIndex >= 0)
            {
                ScrollIntoView(Items[SelectedIndex]);
            }
        }
    }

    public delegate void LogMapperDrawingHandler(object sender, LogMapperDrawingEventArgs args);
    public class LogMapperDrawingEventArgs
    {
        public System.Drawing.Bitmap Buffer { get; private set; }
        public System.Drawing.Graphics Graphic { get; private set; }
        public IEnumerable<object> CopiedItems { get; set; }
        public LogMappingSizeManager MappingSizeManager { get; private set; }

        public bool Handled { get; set; }

        public LogMapperDrawingEventArgs(System.Drawing.Bitmap buffer
            , System.Drawing.Graphics graphic
            , IEnumerable<object> copiedItems
            , LogMappingSizeManager sizeManager)
        {
            Handled = false;
            Buffer = buffer;
            Graphic = graphic;
            CopiedItems = copiedItems;
            MappingSizeManager = sizeManager;
        }
    }
}
