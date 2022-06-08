using cyber_base.implement.command;
using cyber_base.implement.extension;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace honeyboard_release_service.prop.attached_properties
{
    
    internal class ElementAttProperties : UIElement
    {
        #region ProxyProperty
        public static readonly DependencyProperty ProxyProperty =
            DependencyProperty.RegisterAttached(
            "Proxy",
            typeof(object),
            typeof(ElementAttProperties),
            new FrameworkPropertyMetadata(default(object),
                flags: FrameworkPropertyMetadataOptions.AffectsRender));
        public static object GetProxy(UIElement target) =>
            target.GetValue(ProxyProperty);
        public static void SetProxy(UIElement target, object value) =>
            target.SetValue(ProxyProperty, value);

        #endregion
    }
}
