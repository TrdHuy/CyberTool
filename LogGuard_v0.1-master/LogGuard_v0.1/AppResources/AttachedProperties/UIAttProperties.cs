using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogGuard_v0._1.AppResources.AttachedProperties
{
    public class UIAttProperties : UIElement
    {
        #region StringData
        public static readonly DependencyProperty StringDataProperty =
            DependencyProperty.RegisterAttached(
            "StringData",
            typeof(string),
            typeof(UIAttProperties),
            new FrameworkPropertyMetadata(defaultValue: "",
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static string GetStringData(UIElement target) =>
            (string)target.GetValue(StringDataProperty);
        public static void SetStringData(UIElement target, string value) =>
            target.SetValue(StringDataProperty, value);
        #endregion
    }
}
