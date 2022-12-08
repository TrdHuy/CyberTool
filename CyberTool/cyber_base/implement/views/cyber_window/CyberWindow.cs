using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WinInterop = System.Windows.Interop;
using System.Windows.Media;
using System.Runtime.InteropServices;
using cyber_base.implement.utils;

namespace cyber_base.implement.views.cyber_window
{
    public class CyberWindow : Window
    {
        private class WindowSizeManager
        {
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

            /// <summary>
            /// Sent one time to a window, after it has exited the moving or sizing modal loop.
            /// Message detail:
            /// https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-exitsizemove
            /// </summary>
            private const int WM_EXITSIZEMOVE = 0x0232;

            /// <summary>
            /// Sent to a window whose size, position, or place in the Z order has 
            /// changed as a result of a call to the SetWindowPos function or 
            /// another window-management function.
            /// Message detail:
            /// https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-windowposchanged
            /// </summary>
            private const int WM_WINDOWPOSCHANGED = 0x0047;

            /// <summary>
            /// Sent to a window whose size, position, or place in the Z order is about
            /// to change as a result of a call to the SetWindowPos function or another 
            /// window-management function.
            /// Message detail:
            /// https://docs.microsoft.com/en-us/windows/win32/winmsg/wm-windowposchanging
            /// </summary>
            private const int WM_WINDOWPOSCHANGING = 0x0046;

            /// <summary>
            /// A message that is sent to all top-level windows when the 
            /// SystemParametersInfo function changes a system-wide setting or when policy
            /// settings have changed.
            /// Message detail:
            /// https://docs.microsoft.com/en-us/windows/win32/winmsg/wm-settingchange
            /// </summary>
            private const int WM_SETTINGCHANGE = 0x001A;

            /// <summary>
            /// Sent to a window after its size has changed.
            /// Message detail:
            /// https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-size
            /// </summary>
            private const int WM_SIZE = 0x0005;

            /// <summary>
            /// System parameter info
            /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-systemparametersinfoa
            /// </summary>
            private const int SPI_SETWORKAREA = 0x002F;

            private const int SHADOW_DEF = 5;

            private CyberWindow _cyberWindow;
            private MINMAXINFO _cyberMinMaxInfo;
            private RECT _previousCyberWorkArea;
            private RECT _currentCyberWorkArea;
            private WindowState _newState;
            private bool _isInitialized = false;
            private bool _isCyberWindowDockCache = false;

            public WindowState NewState
            {
                get
                {
                    return _newState;
                }
                private set
                {
                    _newState = value;
                    if (_newState == WindowState.Maximized)
                    {
                        _cyberWindow.UpdateWindowShadowEffect(0);
                        SetWindowFullScreen();
                    }
                    else if (_newState == WindowState.Normal)
                    {
                        var shadowDef = IsCyberWindowDockCache ? 0 : SHADOW_DEF;
                        _cyberWindow.UpdateWindowShadowEffect(shadowDef);
                        SetWindowBackToLastNormalSize();
                    }
                    else
                    {
                        _cyberWindow.UpdateWindowShadowEffect(SHADOW_DEF);
                    }
                }
            }
            public WindowState OldState { get; private set; }
            public double InitialWidth { get; private set; }
            public double InitialHeight { get; private set; }
            public double LastNormalWidthCache { get; private set; }
            public double LastNormalHeightCache { get; private set; }

            private bool IsCyberWindowDockCache
            {
                get
                {
                    return _isCyberWindowDockCache;
                }
                set
                {
                    if (value != _isCyberWindowDockCache)
                    {
                        _isCyberWindowDockCache = value;
                        if (_cyberWindow.WindowState == WindowState.Maximized)
                        {
                            _cyberWindow.UpdateWindowShadowEffect(0);
                        }
                        else
                        {
                            _cyberWindow.UpdateWindowShadowEffect(_isCyberWindowDockCache ? 0 : SHADOW_DEF);
                        }
                    }
                }
            }

            public WindowSizeManager(CyberWindow window)
            {
                _cyberWindow = window;
            }

            public void NotifyCyberWindowStateChange()
            {
                OldState = NewState;
                NewState = _cyberWindow.WindowState;
            }

            public void InitWindowSizeManager(double initWidth, double initHeight)
            {
                InitialWidth = initWidth;
                InitialHeight = initHeight;
                LastNormalWidthCache = initWidth;
                LastNormalHeightCache = initHeight;
                _isInitialized = true;
                OldState = _cyberWindow.WindowState;
                NewState = _cyberWindow.WindowState;
            }

            public IntPtr WindowProc(
                 IntPtr hwnd,
                 int msg,
                 IntPtr wParam,
                 IntPtr lParam,
                 ref bool handled)
            {
                switch (msg)
                {
                    case WM_EXITSIZEMOVE:
                        {
                            RECT cyberRect = new RECT();
                            NativeMethods.GetWindowRect(hwnd, ref cyberRect);

                            int MONITOR_DEFAULTTONEAREST = 0x00000002;
                            IntPtr monitor = NativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

                            if (monitor != IntPtr.Zero)
                            {
                                MONITORINFO monitorInfo = new MONITORINFO();
                                NativeMethods.GetMonitorInfo(monitor, monitorInfo);
                                RECT rcWorkArea = monitorInfo.rcWork;
                                RECT rcMonitorArea = monitorInfo.rcMonitor;
                                SetWorkAreaInfo(rcWorkArea);
                                SetWindowDockInfo(cyberRect);
                            }
                            break;
                        }
                    case WM_SETTINGCHANGE:
                        {
                            if (wParam.ToInt32() == SPI_SETWORKAREA)
                            {
                                if (_cyberWindow.WindowState == WindowState.Maximized)
                                {
                                    var isShouldUpdateWindowHeight = _previousCyberWorkArea.bottom < _currentCyberWorkArea.bottom;
                                    if (isShouldUpdateWindowHeight)
                                    {

                                        // Chi tiết chế độ hiển thị:
                                        // https://source.dot.net/#PresentationFramework/System/Windows/Standard/NativeMethods.cs
                                        int SHOWNORMAL = 1;
                                        int SHOWMAXIMIZED = 3;

                                        // Hiển thị window ở trạng thái normal sau đó là maximized
                                        // để cập nhật chiều cao cửa sổ
                                        NativeMethods.ShowWindow(hwnd, SHOWNORMAL);
                                        NativeMethods.ShowWindow(hwnd, SHOWMAXIMIZED);
                                    }

                                }
                                handled = true;
                            }
                            break;
                        }
                    case WM_GETMINMAXINFO:
                        {
                            MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                            // Adjust the maximized size and position to fit the work area of the correct monitor
                            int MONITOR_DEFAULTTONEAREST = 0x00000002;
                            IntPtr monitor = NativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

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
                                SetMinMaxInfo(mmi);
                                SetWorkAreaInfo(rcWorkArea);
                            }
                            Marshal.StructureToPtr(mmi, lParam, true);
                            break;
                        }
                }
                return (IntPtr)0;
            }

            private void SetWindowFullScreen()
            {
                if (_cyberMinMaxInfo.ptMaxSize.x > 0 && _cyberMinMaxInfo.ptMaxSize.y > 0)
                {
                    LastNormalWidthCache = _cyberWindow.ActualWidth;
                    LastNormalHeightCache = _cyberWindow.ActualHeight;
                    _cyberWindow.Height = GetHeightByPixel(_cyberMinMaxInfo.ptMaxSize.y);
                    _cyberWindow.Width = GetWidthByPixel(_cyberMinMaxInfo.ptMaxSize.x);
                }
            }

            private void SetWindowBackToLastNormalSize()
            {
                _cyberWindow.Height = LastNormalHeightCache;
                _cyberWindow.Width = LastNormalWidthCache;
            }

            private void SetMinMaxInfo(MINMAXINFO mmi)
            {
                if (_cyberMinMaxInfo.ptMaxPosition.x != mmi.ptMaxPosition.x
                    || _cyberMinMaxInfo.ptMaxPosition.y != mmi.ptMaxPosition.y
                    || _cyberMinMaxInfo.ptMaxSize.x != mmi.ptMaxSize.x
                    || _cyberMinMaxInfo.ptMaxSize.y != mmi.ptMaxSize.y)
                {
                    _cyberMinMaxInfo.ptMaxPosition.x = mmi.ptMaxPosition.x;
                    _cyberMinMaxInfo.ptMaxPosition.y = mmi.ptMaxPosition.y;
                    _cyberMinMaxInfo.ptMaxSize.x = mmi.ptMaxSize.x;
                    _cyberMinMaxInfo.ptMaxSize.y = mmi.ptMaxSize.y;
                }
            }

            private void SetWorkAreaInfo(RECT rcWorkArea)
            {
                if (_currentCyberWorkArea != rcWorkArea)
                {
                    _previousCyberWorkArea.left = _currentCyberWorkArea.left;
                    _previousCyberWorkArea.top = _currentCyberWorkArea.top;
                    _previousCyberWorkArea.right = _currentCyberWorkArea.right;
                    _previousCyberWorkArea.bottom = _currentCyberWorkArea.bottom;

                    _currentCyberWorkArea.left = rcWorkArea.left;
                    _currentCyberWorkArea.top = rcWorkArea.top;
                    _currentCyberWorkArea.right = rcWorkArea.right;
                    _currentCyberWorkArea.bottom = rcWorkArea.bottom;
                }
            }

            private void SetWindowDockInfo(RECT newRect)
            {
                if (newRect.IsEmpty) return;

                double dockModeWidth = _currentCyberWorkArea.Width / 2;
                var dpi = NativeMethods.GetDeviceCaps();
                double cyberMinWidthInPixel = GetSizeInPixel(_cyberWindow.MinWidth, dpi.X);
                double cyberMaxWidthInPixel = GetSizeInPixel(_cyberWindow.MaxWidth, dpi.X);
                double cyberMinHeightInPixel = GetSizeInPixel(_cyberWindow.MinHeight, dpi.Y);
                double cyberMaxHeightInPixel = GetSizeInPixel(_cyberWindow.MaxHeight, dpi.Y);

                if (cyberMinWidthInPixel > _currentCyberWorkArea.Width / 2)
                {
                    dockModeWidth = cyberMinWidthInPixel;
                }
                else if (cyberMaxWidthInPixel < _currentCyberWorkArea.Width / 2)
                {
                    dockModeWidth = cyberMaxWidthInPixel;
                }

                double dockModeHeight = _currentCyberWorkArea.Height;
                if (cyberMinHeightInPixel > _currentCyberWorkArea.Height)
                {
                    dockModeHeight = cyberMinHeightInPixel;
                }
                else if (cyberMaxHeightInPixel < _currentCyberWorkArea.Height)
                {
                    dockModeHeight = cyberMaxHeightInPixel;
                }

                var isLeftDocked = newRect.left == _currentCyberWorkArea.left
                    && newRect.top == _currentCyberWorkArea.top
                    && newRect.Height == dockModeHeight
                    && newRect.Width == dockModeWidth;


                var isRightDocked = newRect.right == _currentCyberWorkArea.right
                    && newRect.top == _currentCyberWorkArea.top
                    && newRect.Height == dockModeHeight
                    && newRect.Width == dockModeWidth;

                IsCyberWindowDockCache = isLeftDocked || isRightDocked;
            }

            private double GetSizeInPixel(double size, double dpi)
            {
                return size * dpi / 96;
            }

            private double GetWidthByPixel(double pixel)
            {
                var dpi = NativeMethods.GetDeviceCaps();
                return pixel * 96 / dpi.X;
            }

            private double GetHeightByPixel(double pixel)
            {
                var dpi = NativeMethods.GetDeviceCaps();
                return pixel * 96 / dpi.Y;
            }
        }

        private void UpdateWindowShadowEffect(int shadowDef)
        {
            if (_botShadowRowDefinition != null)
            {
                _botShadowRowDefinition.Height = new GridLength(shadowDef);
            }

            if (_leftShadowColumnDefinition != null)
            {
                _leftShadowColumnDefinition.Width = new GridLength(shadowDef);
            }

            if (_rightShadowColumnDefinition != null)
            {
                _rightShadowColumnDefinition.Width = new GridLength(shadowDef);
            }
        }


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
        protected const string WindowControlPanelName = "WindowControlPanel";
        private const string BotShadowRowDefName = "BotShadowRowDef";
        private const string RightShadowColDefName = "RightShadowColumnDef";
        private const string LeftShadowColDefName = "LeftShadowColumnDef";

        private Button? _minimizeBtn;
        private Button? _maximizeBtn;
        private Button? _closeBtn;
        private Button? _smallmizeBtn;
        private StackPanel? _windowControlPanel;
        private RowDefinition? _botShadowRowDefinition;
        private ColumnDefinition? _leftShadowColumnDefinition;
        private ColumnDefinition? _rightShadowColumnDefinition;

        private WindowSizeManager _windowSizeManager;

        public CyberWindow()
        {
            _windowSizeManager = new WindowSizeManager(this);

            DefaultStyleKey = typeof(CyberWindow);
            SourceInitialized += new EventHandler((s, e) =>
            {
                System.IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
                WinInterop.HwndSource.FromHwnd(handle).AddHook(new WinInterop.HwndSourceHook(_windowSizeManager.WindowProc));
            });

            Initialized += OnCyberWindowInitialized;
        }

        private void OnCyberWindowInitialized(object? sender, EventArgs e)
        {
            _windowSizeManager.InitWindowSizeManager(Width, Height);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _minimizeBtn = GetTemplateChild(MinimizeButtonName) as Button ?? throw new ArgumentNullException();
            _maximizeBtn = GetTemplateChild(MaximizeButtonName) as Button ?? throw new ArgumentNullException();
            _closeBtn = GetTemplateChild(CloseButtonName) as Button ?? throw new ArgumentNullException();
            _smallmizeBtn = GetTemplateChild(SmallmizeButtonName) as Button ?? throw new ArgumentNullException();
            _windowControlPanel = GetTemplateChild(WindowControlPanelName) as StackPanel ?? throw new ArgumentNullException();
            _botShadowRowDefinition = GetTemplateChild(BotShadowRowDefName) as RowDefinition ?? throw new ArgumentNullException();
            _leftShadowColumnDefinition = GetTemplateChild(LeftShadowColDefName) as ColumnDefinition ?? throw new ArgumentNullException();
            _rightShadowColumnDefinition = GetTemplateChild(RightShadowColDefName) as ColumnDefinition ?? throw new ArgumentNullException();

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
            _windowSizeManager.NotifyCyberWindowStateChange();
            base.OnStateChanged(e);
        }

    }
}
