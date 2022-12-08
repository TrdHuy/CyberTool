using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace log_guard.prop.attached_properties
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

        #region ProxyObject
        public static readonly DependencyProperty ProxyObjectProperty =
            DependencyProperty.RegisterAttached(
            "ProxyObject",
            typeof(object),
            typeof(UIAttProperties),
            new FrameworkPropertyMetadata(defaultValue: default(Object),
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static object GetProxyObject(UIElement target) =>
            target.GetValue(ProxyObjectProperty);
        public static void SetProxyObject(UIElement target, object value) =>
            target.SetValue(ProxyObjectProperty, value);
        #endregion
    }
}
