using HPSolutionCCDevPackage.netFramework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace LogGuard_v0._1.AppResources.Controls.LogGCalendar
{
    internal class SekerCalendar : Calendar
    {
        internal event SekerCalendarOnApplyTemplateHandler AppliedTemplate;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AppliedTemplate.Invoke();
        }
    }

    internal delegate void SekerCalendarOnApplyTemplateHandler();

    /// <summary>
    /// Represents a control that displays a Chart.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = DateTimeSeker.TabSelectorGridName, Type = typeof(Grid))]
    [TemplatePart(Name = DateTimeSeker.CalendarName, Type = typeof(Calendar))]
    [TemplatePart(Name = DateTimeSeker.CalendarItemName, Type = typeof(CalendarItem))]
    [TemplatePart(Name = DateTimeSeker.RootBorderName, Type = typeof(Border))]
    [TemplatePart(Name = DateTimeSeker.SekerCalendarRootName, Type = typeof(Grid))]
    [TemplatePart(Name = DateTimeSeker.SelectorDateToogleName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = DateTimeSeker.SelectorTimeToogleName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = DateTimeSeker.TodayButtonName, Type = typeof(Button))]
    [TemplatePart(Name = DateTimeSeker.ContinueButtonName, Type = typeof(Button))]
    [TemplatePart(Name = DateTimeSeker.NowButtonName, Type = typeof(Button))]
    [TemplatePart(Name = DateTimeSeker.DoneButtonName, Type = typeof(Button))]
    public class DateTimeSeker : Control
    {
        /// <summary>
        /// Specifies the name of the TabSelectorGrid.
        /// </summary>
        protected const string TabSelectorGridName = "PART_TabSelectorGrid";

        /// <summary>
        /// Specifies the name of the Calendar.
        /// </summary>
        protected const string CalendarName = "PART_Calendar";

        /// <summary>
        /// Specifies the name of the CalendarItem.
        /// </summary>
        protected const string CalendarItemName = "PART_CalendarItem";

        /// <summary>
        /// Specifies the name of the RootBorder.
        /// </summary>
        protected const string RootBorderName = "RootBorder";

        /// <summary>
        /// Specifies the name of the SekerCalendarRootGrid.
        /// </summary>
        protected const string SekerCalendarRootName = "PART_Root";

        /// <summary>
        /// Specifies the name of the DteTgBt.
        /// </summary>
        protected const string SelectorDateToogleName = "SELECTOR_DteTgBt";

        /// <summary>
        /// Specifies the name of the TmeTgBt.
        /// </summary>
        protected const string SelectorTimeToogleName = "SELECTOR_TmeTgBt";

        /// <summary>
        /// Specifies the name of the TodayButton.
        /// </summary>
        protected const string TodayButtonName = "SEKER_TDaBt";

        /// <summary>
        /// Specifies the name of the ContinueButton.
        /// </summary>
        protected const string ContinueButtonName = "SEKER_ConBt";

        /// <summary>
        /// Specifies the name of the ContinueButton.
        /// </summary>
        protected const string NowButtonName = "SEKER_NwBt";

        /// <summary>
        /// Specifies the name of the ContinueButton.
        /// </summary>
        protected const string DoneButtonName = "SEKER_DnBt";

        #region ConerRadius
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius",
                typeof(CornerRadius),
                typeof(DateTimeSeker),
                new FrameworkPropertyMetadata(default(CornerRadius),
                                        FrameworkPropertyMetadataOptions.AffectsMeasure |
                                        FrameworkPropertyMetadataOptions.AffectsRender), null);

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        #endregion

        #region CurrentTab
        public static readonly DependencyProperty CurrentTabProperty =
            DependencyProperty.Register("CurrentTab",
                typeof(SekerTab),
                typeof(DateTimeSeker),
                new PropertyMetadata(default(SekerTab)));

        public SekerTab CurrentTab
        {
            get
            {
                return (SekerTab)GetValue(CurrentTabProperty);
            }
            internal set
            {
                SetValue(CurrentTabProperty, value);
            }
        }
        #endregion

        #region CurrentTimeSession
        public static readonly DependencyProperty CurrentTimeSessionProperty =
            DependencyProperty.Register("CurrentTimeSession",
                typeof(SekerTimeSession),
                typeof(DateTimeSeker),
                new PropertyMetadata(default(SekerTimeSession),
                    OnCurrentSelectedTimeChanged));

        public SekerTimeSession CurrentTimeSession
        {
            get
            {
                return (SekerTimeSession)GetValue(CurrentTimeSessionProperty);
            }
            internal set
            {
                SetValue(CurrentTimeSessionProperty, value);
            }
        }
        #endregion

        #region CurrentHour
        public static readonly DependencyProperty CurrentHourProperty =
            DependencyProperty.Register("CurrentHour",
                typeof(int),
                typeof(DateTimeSeker),
                new PropertyMetadata(default(int),
                    OnCurrentSelectedTimeChanged));

        public int CurrentHour
        {
            get
            {
                return (int)GetValue(CurrentHourProperty);
            }
            internal set
            {
                SetValue(CurrentHourProperty, value);
            }
        }
        #endregion

        #region CurrentMinute
        public static readonly DependencyProperty CurrentMinuteProperty =
            DependencyProperty.Register("CurrentMinute",
                typeof(int),
                typeof(DateTimeSeker),
                new PropertyMetadata(default(int),
                    OnCurrentSelectedTimeChanged));

        public int CurrentMinute
        {
            get
            {
                return (int)GetValue(CurrentMinuteProperty);
            }
            internal set
            {
                SetValue(CurrentMinuteProperty, value);
            }
        }
        #endregion

        #region CurrentSecond
        public static readonly DependencyProperty CurrentSecondProperty =
            DependencyProperty.Register("CurrentSecond",
                typeof(int),
                typeof(DateTimeSeker),
                new PropertyMetadata(default(int),
                    OnCurrentSelectedTimeChanged));

        public int CurrentSecond
        {
            get
            {
                return (int)GetValue(CurrentSecondProperty);
            }
            internal set
            {
                SetValue(CurrentSecondProperty, value);
            }
        }
        #endregion

        #region CurrentMilliSecond
        public static readonly DependencyProperty CurrentMilliSecondProperty =
            DependencyProperty.Register("CurrentMilliSecond",
                typeof(int),
                typeof(DateTimeSeker),
                new PropertyMetadata(default(int),
                    OnCurrentSelectedTimeChanged));

        public int CurrentMilliSecond
        {
            get
            {
                return (int)GetValue(CurrentMilliSecondProperty);
            }
            internal set
            {
                SetValue(CurrentMilliSecondProperty, value);
            }
        }
        #endregion

        #region SelectedDateTime
        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime",
                typeof(DateTime),
                typeof(DateTimeSeker),
                new PropertyMetadata(default(DateTime)));

        public DateTime SelectedDateTime
        {
            get
            {
                return (DateTime)GetValue(SelectedDateTimeProperty);
            }
            internal set
            {
                SetValue(SelectedDateTimeProperty, value);
            }
        }
        #endregion

        #region DoneButtonCommand
        public static readonly DependencyProperty DoneButtonCommandProperty =
            DependencyProperty.Register("DoneButtonCommand",
                typeof(ICommand),
                typeof(DateTimeSeker),
                new PropertyMetadata(default(ICommand), new PropertyChangedCallback(DoneButtonCommandPropertyChangedCallback)));

        private static void DoneButtonCommandPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public ICommand DoneButtonCommand
        {
            get
            {
                return (ICommand)GetValue(DoneButtonCommandProperty);
            }
            set
            {
                SetValue(DoneButtonCommandProperty, value);
            }
        }
        #endregion
        private static void OnCurrentSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as DateTimeSeker;
            ctrl.UpdateSelectedDateTime();
        }

        private Border RootBorder { get; set; }
        private Grid TabSelectorGrid { get; set; }
        private SekerCalendar MainCalendar { get; set; }
        private CalendarItem SekerCalendarItem { get; set; }
        private Grid SekerCalendarRootGrid { get; set; }
        private ToggleButton SelectorDateToggleButton { get; set; }
        private ToggleButton SelectorTimeToggleButton { get; set; }
        private Button TodayButton { get; set; }
        private Button ContinueButton { get; set; }
        private Button NowButton { get; set; }
        private Button DoneButton { get; set; }

        public DateTimeSeker()
        {
            DefaultStyleKey = typeof(DateTimeSeker);
            this.IsTabStop = true;
        }

        public override void BeginInit()
        {
            base.BeginInit();
            CurrentTab = SekerTab.DateTab;
            CurrentTimeSession = SekerTimeSession.AM;
            SelectedDateTime = DateTime.Now;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RootBorder = GetTemplateChild(RootBorderName) as Border;
            MainCalendar = GetTemplateChild(CalendarName) as SekerCalendar;
            TabSelectorGrid = GetTemplateChild(TabSelectorGridName) as Grid;
            SelectorDateToggleButton = GetTemplateChild(SelectorDateToogleName) as ToggleButton;
            SelectorTimeToggleButton = GetTemplateChild(SelectorTimeToogleName) as ToggleButton;
            TodayButton = GetTemplateChild(TodayButtonName) as Button;
            ContinueButton = GetTemplateChild(ContinueButtonName) as Button;
            NowButton = GetTemplateChild(NowButtonName) as Button;
            DoneButton = GetTemplateChild(DoneButtonName) as Button;

            MainCalendar.AppliedTemplate += MainCalendar_AppliedTemplate;
            TodayButton.Click += TodayButton_Click;
            ContinueButton.Click += ContinueButton_Click;
            NowButton.Click += NowButton_Click;
            DoneButton.Click += DoneButton_Click;
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if(DoneButtonCommand != null)
            {
                if (DoneButtonCommand.CanExecute(this))
                {
                    DoneButtonCommand?.Execute(this);
                }
            }
        }

        private void NowButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentHour = DateTime.Now.Hour % 12;
            CurrentMinute = DateTime.Now.Minute;
            CurrentSecond = DateTime.Now.Second;
            CurrentMilliSecond = DateTime.Now.Millisecond;

            if (DateTime.Now.Hour >= 12)
            {
                CurrentTimeSession = SekerTimeSession.PM;
            }
            else
            {
                CurrentTimeSession = SekerTimeSession.AM;
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentTab = SekerTab.TimeTab;
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedDateTime = DateTime.Now.Date;
            MainCalendar.DisplayDate = SelectedDateTime;
        }

        private void MainCalendar_AppliedTemplate()
        {
            SekerCalendarItem = MainCalendar?.FindChild<CalendarItem>(CalendarItemName);
            SekerCalendarRootGrid = MainCalendar?.FindChild<Grid>(SekerCalendarRootName);

            SekerCalendarRootGrid.SizeChanged -= SekerCalendarRootGrid_SizeChanged;
            SekerCalendarRootGrid.SizeChanged += SekerCalendarRootGrid_SizeChanged;
        }

        private void SekerCalendarRootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RootBorder.Width = SekerCalendarRootGrid.ActualWidth + RootBorder.BorderThickness.Left + RootBorder.BorderThickness.Right;
        }

        private void UpdateSelectedDateTime()
        {
            var tempTime = new TimeSpan(CurrentTimeSession == SekerTimeSession.PM ? CurrentHour + 12 : CurrentHour
                , CurrentMinute
                , CurrentSecond);
            var milliSec = TimeSpan.FromMilliseconds(CurrentMilliSecond);
            SelectedDateTime = SelectedDateTime.Date + tempTime + milliSec;
        }

    }

    public enum SekerTab
    {
        DateTab = 0,
        TimeTab = 1
    }

    public enum SekerTimeSession
    {
        PM = 0,
        AM = 1
    }


    internal class SekerTabIsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.Equals(value.ToString()))
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.Equals(SekerTab.DateTab.ToString()))
            {
                return SekerTab.DateTab;
            }
            else if (parameter.Equals(SekerTab.TimeTab.ToString()))
            {
                return SekerTab.TimeTab;
            }
            else if (parameter.Equals(SekerTimeSession.AM.ToString()))
            {
                return SekerTimeSession.AM;
            }
            else if (parameter.Equals(SekerTimeSession.PM.ToString()))
            {
                return SekerTimeSession.PM;
            }
            return null;
        }
    }
}
