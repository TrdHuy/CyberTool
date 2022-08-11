using LogGuard_v0._1.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinInterop = System.Windows.Interop;
using System.Runtime.InteropServices;

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
            SourceInitialized += new EventHandler((s, e) =>
            {
                System.IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
                WinInterop.HwndSource.FromHwnd(handle).AddHook(new WinInterop.HwndSourceHook(WindowProc));
            });
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

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
        }

        /// <summary>
        /// Message detail:
        /// https://docs.microsoft.com/en-us/windows/win32/winmsg/wm-getminmaxinfo
        /// </summary>
        private const int WM_GETMINMAXINFO = 0x0024;

        private System.IntPtr WindowProc(
              System.IntPtr hwnd,
              int msg,
              System.IntPtr wParam,
              System.IntPtr lParam,
              ref bool handled)
        {
            switch (msg)
            {
                case WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (System.IntPtr)0;
        }

        private void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = NativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                NativeMethods.GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }
    }
}
