using HPSolutionCCDevPackage.netFramework.Atrributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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


        public static readonly DependencyProperty LeftDoubleClickCommandProperty =
            DependencyProperty.Register("LeftDoubleClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetLeftDoubleClickCommand(UIElement target) =>
           (ICommand)target.GetValue(LeftDoubleClickCommandProperty);
        public static void SetLeftDoubleClickCommand(UIElement target, ICommand value) =>
            target.SetValue(LeftDoubleClickCommandProperty, value);


        public static readonly DependencyProperty LeftClickCommandProperty =
            DependencyProperty.Register("LeftClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetLeftClickCommand(UIElement target) =>
           (ICommand)target.GetValue(LeftClickCommandProperty);
        public static void SetLeftClickCommand(UIElement target, ICommand value) =>
            target.SetValue(LeftClickCommandProperty, value);


        public static readonly DependencyProperty RightClickCommandProperty =
            DependencyProperty.Register("RightClickCommand",
                typeof(ICommand),
                typeof(TextBoxAttProperties),
                new PropertyMetadata(default(ICommand)));
        public static ICommand GetRightClickCommand(UIElement target) =>
           (ICommand)target.GetValue(RightClickCommandProperty);
        public static void SetRightClickCommand(UIElement target, ICommand value) =>
            target.SetValue(RightClickCommandProperty, value);

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
    }
}
