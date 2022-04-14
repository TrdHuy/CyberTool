using HPSolutionCCDevPackage.netFramework.Atrributes;
using HPSolutionCCDevPackage.netFramework.Utils;
using LogGuard_v0._1.AppResources.Controls.LogGCalendar;
using LogGuard_v0._1.Utils;
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

namespace LogGuard_v0._1.AppResources.AttachedProperties
{
    public enum FilterType
    {
        [StringValue("Simple")]
        Simple = 0,

        [StringValue("Syntax")]
        Syntax = 1,

        [StringValue("Advance")]
        Advance = 2
    }
    public class TextBoxAttProperties : UIElement
    {
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

        #region LeftDoubleClickCmd
        public static readonly DependencyProperty LeftDoubleClickCommandProperty =
            DependencyProperty.Register("LeftDoubleClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetLeftDoubleClickCommand(UIElement target) =>
           (ICommand)target.GetValue(LeftDoubleClickCommandProperty);
        public static void SetLeftDoubleClickCommand(UIElement target, ICommand value) =>
            target.SetValue(LeftDoubleClickCommandProperty, value);
        #endregion

        #region LeftClickCmd
        public static readonly DependencyProperty LeftClickCommandProperty =
            DependencyProperty.Register("LeftClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetLeftClickCommand(UIElement target) =>
           (ICommand)target.GetValue(LeftClickCommandProperty);
        public static void SetLeftClickCommand(UIElement target, ICommand value) =>
            target.SetValue(LeftClickCommandProperty, value);
        #endregion

        #region RightClickCmd
        public static readonly DependencyProperty RightClickCommandProperty =
            DependencyProperty.Register("RightClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetRightClickCommand(UIElement target) =>
           (ICommand)target.GetValue(RightClickCommandProperty);
        public static void SetRightClickCommand(UIElement target, ICommand value) =>
            target.SetValue(RightClickCommandProperty, value);
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
                        DoneButtonCommand = new CommonCommand((sender) =>
                        {
                            if (sender != null)
                            {
                                var seker = sender as DateTimeSeker;
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
