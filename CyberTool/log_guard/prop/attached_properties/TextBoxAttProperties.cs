using cyber_base.implement.attributes;
using cyber_base.implement.command;
using cyber_base.implement.extension;
using log_guard.@base.flow.source_filterr;
using log_guard.views.others.calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace log_guard.prop.attached_properties
{
    
    /// <summary>
    /// Cách search mà Text box sẽ hoạt động
    /// None: TextBox sẽ không thực hiện bất kỳ chức năng nào khi người dùng thay đổi input
    /// NormalSearch: thực hiện search và filter khi người dùng nhấn Enter
    /// QuickSearch: thực hiện search và filter mỗi khi người dùng thay đổi giá trị input
    /// </summary>
    public enum SearchBehavior
    {
        None = -1,

        NormalSearch = 0,

        QuickSearch = 1
    }

    internal class TextBoxAttProperties : UIElement
    {
        #region Search
        public static readonly DependencyProperty SearchProperty =
            DependencyProperty.RegisterAttached(
            "Search",
            typeof(SearchBehavior),
            typeof(TextBoxAttProperties),
            new FrameworkPropertyMetadata(defaultValue: SearchBehavior.None,
                flags: FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnSearchTypeChangedCallback)));

        private static void OnSearchTypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            var type = (SearchBehavior)e.NewValue;

            if (type == SearchBehavior.NormalSearch && textBox != null)
            {
                textBox.KeyUp -= NormalSearchTypePreviewKeyDown;
                textBox.KeyUp += NormalSearchTypePreviewKeyDown;

            }
            else if (type == SearchBehavior.QuickSearch && textBox != null)
            {
                textBox.KeyUp -= NormalSearchTypePreviewKeyDown;
            }
        }

        private static void NormalSearchTypePreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if(e.Key == Key.Enter)
            {
                FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;
                TraversalRequest request = new TraversalRequest(focusDirection);
                textBox.MoveFocus(request);
            }
        }

        public static SearchBehavior GetSearch(UIElement target) =>
            (SearchBehavior)target.GetValue(SearchProperty);
        public static void SetSearch(UIElement target, SearchBehavior value) =>
            target.SetValue(SearchProperty, value);

        #endregion

        #region IsSupportMultiFilterEngine
        public static readonly DependencyProperty IsSupportMultiFilterEngineProperty =
            DependencyProperty.RegisterAttached(
            "IsSupportMultiFilterEngine",
            typeof(bool),
            typeof(TextBoxAttProperties),
            new FrameworkPropertyMetadata(defaultValue: true,
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static bool GetIsSupportMultiFilterEngine(UIElement target) =>
            (bool)target.GetValue(IsSupportMultiFilterEngineProperty);
        public static void SetIsSupportMultiFilterEngine(UIElement target, bool value) =>
            target.SetValue(IsSupportMultiFilterEngineProperty, value);

        #endregion

        #region IsBusy
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.RegisterAttached(
            "IsBusy",
            typeof(bool),
            typeof(TextBoxAttProperties),
            new FrameworkPropertyMetadata(defaultValue: true,
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static bool GetIsBusy(UIElement target) =>
            (bool)target.GetValue(IsBusyProperty);
        public static void SetIsBusy(UIElement target, bool value) =>
            target.SetValue(IsBusyProperty, value);

        #endregion

        #region IsFilterTextBox
        public static readonly DependencyProperty IsFilterTextBoxProperty =
            DependencyProperty.RegisterAttached(
            "IsFilterTextBox",
            typeof(bool),
            typeof(TextBoxAttProperties),
            new FrameworkPropertyMetadata(defaultValue: true,
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static bool GetIsFilterTextBox(UIElement target) =>
            (bool)target.GetValue(IsFilterTextBoxProperty);
        public static void SetIsFilterTextBox(UIElement target, bool value) =>
            target.SetValue(IsFilterTextBoxProperty, value);

        #endregion

        #region IsFilterEnable
        public static readonly DependencyProperty IsFilterEnableProperty =
            DependencyProperty.RegisterAttached(
            "IsFilterEnable",
            typeof(bool),
            typeof(TextBoxAttProperties),
            new FrameworkPropertyMetadata(defaultValue: false,
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static bool GetIsFilterEnable(UIElement target) =>
            (bool)target.GetValue(IsFilterEnableProperty);
        public static void SetIsFilterEnable(UIElement target, bool value) =>
            target.SetValue(IsFilterEnableProperty, value);

        #endregion

        #region Filter
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.RegisterAttached(
            "Filter",
            typeof(FilterType),
            typeof(TextBoxAttProperties),
            new FrameworkPropertyMetadata(defaultValue: FilterType.Simple,
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static FilterType GetFilter(UIElement target) =>
            (FilterType)target.GetValue(FilterProperty);
        public static void SetFilter(UIElement target, FilterType value) =>
            target.SetValue(FilterProperty, value);

        #endregion

        #region PathData
        public static readonly DependencyProperty PathDataProperty =
            DependencyProperty.RegisterAttached(
            "PathData",
            typeof(string),
            typeof(TextBoxAttProperties),
            new FrameworkPropertyMetadata(defaultValue: "",
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static string GetPathData(UIElement target) =>
            (string)target.GetValue(PathDataProperty);
        public static void SetPathData(UIElement target, string value) =>
            target.SetValue(PathDataProperty, value);
        #endregion

        #region EngineLeftDoubleClickCommand
        public static readonly DependencyProperty EngineLeftDoubleClickCommandProperty =
            DependencyProperty.Register("EngineLeftDoubleClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetEngineLeftDoubleClickCommand(UIElement target) =>
           (ICommand)target.GetValue(EngineLeftDoubleClickCommandProperty);
        public static void SetEngineLeftDoubleClickCommand(UIElement target, ICommand value) =>
            target.SetValue(EngineLeftDoubleClickCommandProperty, value);
        #endregion

        #region EngineLeftClickCmd
        public static readonly DependencyProperty EngineLeftClickCommandProperty =
            DependencyProperty.Register("EngineLeftClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetEngineLeftClickCommand(UIElement target) =>
           (ICommand)target.GetValue(EngineLeftClickCommandProperty);
        public static void SetEngineLeftClickCommand(UIElement target, ICommand value) =>
            target.SetValue(EngineLeftClickCommandProperty, value);
        #endregion

        #region EngineRightClickCmd
        public static readonly DependencyProperty EngineRightClickCommandProperty =
            DependencyProperty.Register("EngineRightClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetEngineRightClickCommand(UIElement target) =>
           (ICommand)target.GetValue(EngineRightClickCommandProperty);
        public static void SetEngineRightClickCommand(UIElement target, ICommand value) =>
            target.SetValue(EngineRightClickCommandProperty, value);
        #endregion

        #region FilterLeftDoubleClickCmd
        public static readonly DependencyProperty FilterLeftDoubleClickCommandProperty =
            DependencyProperty.Register("FilterLeftDoubleClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetFilterLeftDoubleClickCommand(UIElement target) =>
           (ICommand)target.GetValue(FilterLeftDoubleClickCommandProperty);
        public static void SetFilterLeftDoubleClickCommand(UIElement target, ICommand value) =>
            target.SetValue(FilterLeftDoubleClickCommandProperty, value);
        #endregion

        #region FilterLeftClickCmd
        public static readonly DependencyProperty FilterLeftClickCommandProperty =
            DependencyProperty.Register("FilterLeftClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetFilterLeftClickCommand(UIElement target) =>
           (ICommand)target.GetValue(FilterLeftClickCommandProperty);
        public static void SetFilterLeftClickCommand(UIElement target, ICommand value) =>
            target.SetValue(FilterLeftClickCommandProperty, value);
        #endregion

        #region FilterRightClickCmd
        public static readonly DependencyProperty FilterRightClickCommandProperty =
            DependencyProperty.Register("FilterRightClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetFilterRightClickCommand(UIElement target) =>
           (ICommand)target.GetValue(FilterRightClickCommandProperty);
        public static void SetFilterRightClickCommand(UIElement target, ICommand value) =>
            target.SetValue(FilterRightClickCommandProperty, value);
        #endregion

        #region FilterHelperContent
        public static readonly DependencyProperty FilterHelperContentProperty =
            DependencyProperty.RegisterAttached(
            "FilterHelperContent",
            typeof(string),
            typeof(TextBoxAttProperties),
            new FrameworkPropertyMetadata(defaultValue: null,
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static string GetFilterHelperContent(UIElement target) =>
            (string)target.GetValue(FilterHelperContentProperty);
        public static void SetFilterHelperContent(UIElement target, string value) =>
            target.SetValue(FilterHelperContentProperty, value);
        #endregion

        #region FilterConditionHelperContent
        public static readonly DependencyProperty FilterConditionHelperContentProperty =
            DependencyProperty.RegisterAttached(
            "FilterConditionHelperContent",
            typeof(string),
            typeof(TextBoxAttProperties),
            new FrameworkPropertyMetadata(defaultValue: null,
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static string GetFilterConditionHelperContent(UIElement target) =>
            (string)target.GetValue(FilterConditionHelperContentProperty);
        public static void SetFilterConditionHelperContent(UIElement target, string value) =>
            target.SetValue(FilterConditionHelperContentProperty, value);
        #endregion

        #region EngineHelperContent
        public static readonly DependencyProperty EngineHelperContentProperty =
            DependencyProperty.RegisterAttached(
            "EngineHelperContent",
            typeof(string),
            typeof(TextBoxAttProperties),
            new FrameworkPropertyMetadata(defaultValue: null,
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static string GetEngineHelperContent(UIElement target) =>
            (string)target.GetValue(EngineHelperContentProperty);
        public static void SetEngineHelperContent(UIElement target, string value) =>
            target.SetValue(EngineHelperContentProperty, value);
        #endregion

        #region IsOpenCalendarPopup
        public static readonly DependencyProperty IsOpenCalendarPopupProperty =
            DependencyProperty.RegisterAttached(
            "IsOpenCalendarPopup",
            typeof(bool),
            typeof(TextBoxAttProperties),
            new FrameworkPropertyMetadata(defaultValue: false,
                flags: FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnOpenCalendarPopupChangedCallback)));

        private static void OnOpenCalendarPopupChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            var popUp = textBox?.FindChild<Popup>("PART_Popup");
            if (popUp != null)
            {
                popUp.IsOpen = true;
                if (popUp.Child == null)
                {
                    popUp.Child = new DateTimeSeker()
                    {
                        DoneButtonCommand = new BaseDotNetCommandImpl((sender) =>
                        {
                            var seker = sender as DateTimeSeker;
                            if (seker != null)
                            {
                                popUp.IsOpen = false;
                                textBox.Text = seker?.SelectedDateTime.ToString("dd-MM-yyyy HH:mm:ss:ffffff");
                            }
                        })
                    };
                }
            }
        }

        public static bool GetIsOpenCalendarPopup(UIElement target) =>
            (bool)target.GetValue(IsOpenCalendarPopupProperty);
        public static void SetIsOpenCalendarPopup(UIElement target, bool value) =>
            target.SetValue(IsOpenCalendarPopupProperty, value);

        #endregion

        #region IsTimeTextBox
        public static readonly DependencyProperty IsTimeTextBoxProperty =
           DependencyProperty.RegisterAttached(
           "IsTimeTextBox",
           typeof(bool),
           typeof(TextBoxAttProperties),
           new FrameworkPropertyMetadata(defaultValue: false,
               flags: FrameworkPropertyMetadataOptions.AffectsRender,
               new PropertyChangedCallback(OnIsTimeTextBoxChangedCallback)));

        private static void OnIsTimeTextBoxChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            var isTimeTB = (bool)e.NewValue;

            if (isTimeTB && textBox != null)
            {
                textBox.PreviewKeyDown -= TimeTextBoxPreviewKeyDown;
                textBox.PreviewKeyDown += TimeTextBoxPreviewKeyDown;

            }
            else if (!isTimeTB && textBox != null)
            {
                textBox.PreviewKeyDown -= TimeTextBoxPreviewKeyDown;

            }
        }

        private static void TimeTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = true;

            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                textBox.Text = "";
            }
        }

        public static bool GetIsTimeTextBox(UIElement target) =>
            (bool)target.GetValue(IsOpenCalendarPopupProperty);
        public static void SetIsTimeTextBox(UIElement target, bool value) =>
            target.SetValue(IsOpenCalendarPopupProperty, value);
        #endregion
    }
}
