using cyber_base.implement.command;
using cyber_base.implement.extension;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace progtroll.prop.attached_properties
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

        #region IsNumberBox
        public static readonly DependencyProperty IsNumberBoxProperty =
            DependencyProperty.RegisterAttached(
            "IsNumberBox",
            typeof(bool),
            typeof(ElementAttProperties),
            new PropertyMetadata(default(bool),
                new PropertyChangedCallback(OnIsNumberBoxPropertyChangedHandler)));

        private static void OnIsNumberBoxPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBox;
            if (tb != null)
            {
                if ((bool)e.NewValue == true)
                {
                    DataObject.RemovePastingHandler(tb, OnNumberBoxPasting);
                    DataObject.AddPastingHandler(tb, OnNumberBoxPasting);

                    tb.PreviewTextInput -= OnNumberBoxPreviewTextInput;
                    tb.PreviewTextInput += OnNumberBoxPreviewTextInput;
                }
                else
                {
                    DataObject.RemovePastingHandler(tb, OnNumberBoxPasting);
                    tb.PreviewTextInput -= OnNumberBoxPreviewTextInput;
                }
            }
        }

        private static readonly Regex notAllowedRegex = new Regex("[^0-9]+");

        private static void OnNumberBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                string text = (String)e.DataObject.GetData(typeof(String));
                if (notAllowedRegex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }


        private static void OnNumberBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = notAllowedRegex.IsMatch(e.Text);
        }

        public static bool GetIsNumberBox(UIElement target) =>
            (bool)target.GetValue(IsNumberBoxProperty);
        public static void SetIsNumberBox(UIElement target, bool value) =>
            target.SetValue(IsNumberBoxProperty, value);

        #endregion
    }
}
