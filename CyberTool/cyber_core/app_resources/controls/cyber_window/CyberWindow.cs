using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinInterop = System.Windows.Interop;
using System.Windows.Media;
using cyber_core.utils;
using System.Runtime.InteropServices;

namespace cyber_core.app_resources.controls.cyber_window
{
    public class CyberWindow : Window
    {
        #region ChromeBackground
        public static readonly DependencyProperty ChromeBackgroundProperty = DependencyProperty.RegisterAttached(
               "ChromeBackground",
               typeof(Brush),
               typeof(CyberWindow),
               new PropertyMetadata(Brushes.Transparent));

        public Brush ChromeBackground
        {
            get { return (Brush)GetValue(ChromeBackgroundProperty); }
            set { SetValue(ChromeBackgroundProperty, value); }
        }
        #endregion

        #region IsWindowButtonEnabled
        public static readonly DependencyProperty IsWindowButtonEnabledProperty = DependencyProperty.RegisterAttached(
               "IsWindowButtonEnabled",
               typeof(bool),
               typeof(CyberWindow),
               new PropertyMetadata(true, new PropertyChangedCallback(OnWindowButtonEnabledChangedCallback)));

        private static void OnWindowButtonEnabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as CyberWindow;
            var enabled = (bool)e.NewValue;
            if (ctrl != null && ctrl._windowControlPanel != null)
            {
                ctrl._windowControlPanel.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsWindowButtonEnabled
        {
            get { return (bool)GetValue(IsWindowButtonEnabledProperty); }
            set { SetValue(IsWindowButtonEnabledProperty, value); }
        }

        #endregion

        private const string MinimizeButtonName = "MinimizeButton";
        private const string SmallmizeButtonName = "SmallmizeButton";
        private const string CloseButtonName = "CloseButton";
        private const string MaximizeButtonName = "MaximizeButton";
        private const string WindowControlPanelName = "WindowControlPanel";

        public CyberWindow()
        {
            DefaultStyleKey = typeof(CyberWindow);
            SourceInitialized += new EventHandler((s, e) =>
            {
                System.IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
                WinInterop.HwndSource.FromHwnd(handle).AddHook(new WinInterop.HwndSourceHook(WindowProc));
            });
        }



        private Button? _minimizeBtn;
        private Button? _maximizeBtn;
        private Button? _closeBtn;
        private Button? _smallmizeBtn;
        private StackPanel? _windowControlPanel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _minimizeBtn = GetTemplateChild(MinimizeButtonName) as Button;
            _maximizeBtn = GetTemplateChild(MaximizeButtonName) as Button;
            _closeBtn = GetTemplateChild(CloseButtonName) as Button;
            _smallmizeBtn = GetTemplateChild(SmallmizeButtonName) as Button;
            _windowControlPanel = GetTemplateChild(WindowControlPanelName) as StackPanel;

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

            _windowControlPanel.Visibility = IsWindowButtonEnabled ? Visibility.Visible : Visibility.Collapsed;

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

        /// <summary>
        /// Nonclient area double left click event
        /// Message detail:
        /// https://docs.microsoft.com/vi-vn/windows/win32/inputdev/wm-nclbuttondown
        /// </summary>
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;
        
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
                case WM_NCLBUTTONDBLCLK:
                    if(WindowState == WindowState.Normal)
                    {
                        WindowState = WindowState.Maximized;
                    }
                    else if (WindowState == WindowState.Maximized)
                    {
                        WindowState = WindowState.Normal;
                    }
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
