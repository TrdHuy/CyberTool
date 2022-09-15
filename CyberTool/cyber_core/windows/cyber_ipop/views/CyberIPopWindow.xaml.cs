using cyber_base.implement.views.cyber_window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace cyber_core.windows.cyber_ipop.views
{
    /// <summary>
    /// Interaction logic for CyberIPopWindow.xaml
    /// </summary>
    public partial class CyberIPopWindow : CyberWindow
    {
        private const string MinimizeButtonName = "MinimizeButton";
        private const string SmallmizeButtonName = "SmallmizeButton";
        private const string CloseButtonName = "CloseButton";
        private const string MaximizeButtonName = "MaximizeButton";
        private const string MainBorderName = "MainBorderContainer";

        public CyberIPopWindow()
        {
            InitializeComponent();
        }

        public CyberIPopWindow(UIElement? opener, UIElement? owner)
        {
            InitializeComponent();
            _opener = opener;
            _owner = owner;
            Owner = _owner as Window;
            if (Owner != null)
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private UIElement? _opener;
        private UIElement? _owner;
        private Button? _minimizeBtn;
        private Button? _maximizeBtn;
        private Button? _closeBtn;
        private Button? _smallmizeBtn;
        private Border? _mainBorder;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _minimizeBtn = GetTemplateChild(MinimizeButtonName) as Button;
            _maximizeBtn = GetTemplateChild(MaximizeButtonName) as Button;
            _closeBtn = GetTemplateChild(CloseButtonName) as Button;
            _smallmizeBtn = GetTemplateChild(SmallmizeButtonName) as Button;
            _mainBorder = GetTemplateChild(MainBorderName) as Border;
            var animTime = 200;

            if (_mainBorder == null || _closeBtn == null || _minimizeBtn == null)
            {
                return;
            }
            _mainBorder.Loaded += (s, e) =>
            {
                StartOpenAnimation(animTime);
            };

            _closeBtn.Click += (s, e) =>
            {
                StartCloseAnimation(animTime);
            };

            _minimizeBtn.Click += (s, e) =>
            {
                this.WindowState = WindowState.Minimized;
            };

        }

        private void StartOpenAnimation(int animTime)
        {
            Storyboard expandBoard = new Storyboard();
            DoubleAnimation scaleXAnim = new DoubleAnimation();
            Storyboard.SetTargetName(scaleXAnim, "MainBorderCtn_ScaleTransform");
            Storyboard.SetTargetProperty(scaleXAnim, new PropertyPath(ScaleTransform.ScaleXProperty));
            scaleXAnim.From = 0.0d;
            scaleXAnim.To = 1.0d;
            scaleXAnim.Duration = TimeSpan.FromMilliseconds(animTime);

            DoubleAnimation scaleYAnim = new DoubleAnimation();
            Storyboard.SetTargetName(scaleYAnim, "MainBorderCtn_ScaleTransform");
            Storyboard.SetTargetProperty(scaleYAnim, new PropertyPath(ScaleTransform.ScaleYProperty));
            scaleYAnim.From = 0.0d;
            scaleYAnim.To = 1.0d;
            scaleYAnim.Duration = TimeSpan.FromMilliseconds(animTime);

            expandBoard.Children.Add(scaleXAnim);
            expandBoard.Children.Add(scaleYAnim);


            Storyboard moveBoard = new Storyboard();

            DoubleAnimation fadeAnim = new DoubleAnimation();
            Storyboard.SetTargetName(fadeAnim, "mainWindow");
            Storyboard.SetTargetProperty(fadeAnim, new PropertyPath(Window.OpacityProperty));
            fadeAnim.From = 0d;
            fadeAnim.To = 1d;
            fadeAnim.Duration = TimeSpan.FromMilliseconds(animTime);

            moveBoard.Children.Add(fadeAnim);


            expandBoard.Begin(_mainBorder);
            moveBoard.Begin(this);
        }

        private void StartCloseAnimation(int animTime)
        {
            Storyboard dispandBoard = new Storyboard();
            DoubleAnimation scaleXAnim = new DoubleAnimation();
            Storyboard.SetTargetName(scaleXAnim, "MainBorderCtn_ScaleTransform");
            Storyboard.SetTargetProperty(scaleXAnim, new PropertyPath(ScaleTransform.ScaleXProperty));
            scaleXAnim.From = 1.0d;
            scaleXAnim.To = 0.0d;
            scaleXAnim.Duration = TimeSpan.FromMilliseconds(animTime);

            DoubleAnimation scaleYAnim = new DoubleAnimation();
            Storyboard.SetTargetName(scaleYAnim, "MainBorderCtn_ScaleTransform");
            Storyboard.SetTargetProperty(scaleYAnim, new PropertyPath(ScaleTransform.ScaleYProperty));
            scaleYAnim.From = 1.0d;
            scaleYAnim.To = 0.0d;
            scaleYAnim.Duration = TimeSpan.FromMilliseconds(animTime);

            dispandBoard.Children.Add(scaleXAnim);
            dispandBoard.Children.Add(scaleYAnim);


            Storyboard moveBoard = new Storyboard();

            DoubleAnimation fadeAnim = new DoubleAnimation();
            Storyboard.SetTargetName(fadeAnim, "mainWindow");
            Storyboard.SetTargetProperty(fadeAnim, new PropertyPath(Window.OpacityProperty));
            fadeAnim.From = 1d;
            fadeAnim.To = 0d;
            fadeAnim.Duration = TimeSpan.FromMilliseconds(animTime);

            moveBoard.Children.Add(fadeAnim);


            dispandBoard.Completed += (s2, e2) =>
            {
                this.Close();
            };
            dispandBoard.Begin(_mainBorder);
            moveBoard.Begin(this);
        }

    }
}
