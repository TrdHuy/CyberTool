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
