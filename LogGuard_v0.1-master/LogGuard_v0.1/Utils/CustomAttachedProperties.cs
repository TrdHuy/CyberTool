using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LogGuard_v0._1.Utils
{
    public class CustomAttachedProperties : DependencyObject
    {
        public static string GetButtonTextProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(ButtonTextProperty);
        }

        public static void SetButtonTextProperty(DependencyObject obj, string value)
        {
            obj.SetValue(ButtonTextProperty, value);
        }

        // Using a DependencyPropertyExample as the backing store for MyProperty.  This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.RegisterAttached("SetButtonText", typeof(string), typeof(CustomAttachedProperties), new FrameworkPropertyMetadata(null));

    }
}
