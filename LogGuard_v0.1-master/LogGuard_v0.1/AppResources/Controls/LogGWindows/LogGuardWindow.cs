using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LogGuard_v0._1.AppResources.Controls.LogGWindows
{
    public class LogGuardWindow : Window
    {
        #region ChromeBackground
        public static readonly DependencyProperty ChromeBackgroundProperty = DependencyProperty.RegisterAttached(
               "ChromeBackground",
               typeof(Brush),
               typeof(LogGuardWindow),
               new PropertyMetadata(Brushes.Transparent));

        public static Brush GetChromeBackground(UIElement obj)
        {
            return (Brush)obj.GetValue(ChromeBackgroundProperty);
        }

        public static void SetChromeBackground(UIElement obj, Brush value)
        {
            obj.SetValue(ChromeBackgroundProperty, value);
        }
        #endregion

        private const string MinimizeButtonName = "MinimizeButton";
        private const string SmallmizeButtonName = "SmallmizeButton";
        private const string CloseButtonName = "CloseButton";
        private const string MaximizeButtonName = "MaximizeButton";

        public LogGuardWindow()
        {
            DefaultStyleKey = typeof(LogGuardWindow);

        }

        private Button _minimizeBtn;
        private Button _maximizeBtn;
        private Button _closeBtn;
        private Button _smallmizeBtn;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _minimizeBtn = GetTemplateChild(MinimizeButtonName) as Button;
            _maximizeBtn = GetTemplateChild(MaximizeButtonName) as Button;
            _closeBtn = GetTemplateChild(CloseButtonName) as Button;
            _smallmizeBtn = GetTemplateChild(SmallmizeButtonName) as Button;


            _maximizeBtn.Click += (s, e) =>
            {
                this.WindowState = WindowState.Maximized;
            };

            _smallmizeBtn.Click += (s, e) =>
            {
                this.WindowState = WindowState.Normal;
            };

            _closeBtn.Click += (s, e) =>
            {
                this.Close();
            };

            _minimizeBtn.Click += (s, e) =>
            {
                this.WindowState = WindowState.Minimized;
            };

        }

    }
}
