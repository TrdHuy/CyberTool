using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace cyber_tool.app_resources.controls.cyber_scroll
{
    public class CyberScrollView: ScrollViewer
    {
        public static readonly DependencyProperty IsMouseOverExpandScrollbarProperty =
            DependencyProperty.Register(
                "IsMouseOverExpandScrollbar",
                typeof(bool),
                typeof(CyberScrollView),
                new PropertyMetadata(false));

        public bool IsMouseOverExpandScrollbar
        {
            get { return (bool)GetValue(IsMouseOverExpandScrollbarProperty); }
            set { SetValue(IsMouseOverExpandScrollbarProperty, value); }
        }

        public static readonly DependencyProperty ScrollHeaderHeightProperty =
            DependencyProperty.Register(
                "ScrollHeaderHeight",
                typeof(double),
                typeof(CyberScrollView),
                new PropertyMetadata(default(double)));

        public double ScrollHeaderHeight
        {
            get { return (double)GetValue(ScrollHeaderHeightProperty); }
            set { SetValue(ScrollHeaderHeightProperty, value); }
        }

        public static readonly DependencyProperty ScrollBarBackgroundProperty =
            DependencyProperty.Register(
                "ScrollBarBackground",
                typeof(Brush),
                typeof(CyberScrollView),
                new UIPropertyMetadata(default(Brush)));

        public Brush ScrollBarBackground
        {
            get { return (Brush)GetValue(ScrollBarBackgroundProperty); }
            set { SetValue(ScrollBarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ThumbBackgroundProperty =
            DependencyProperty.Register(
                "ThumbBackground",
                typeof(Brush),
                typeof(CyberScrollView),
                new UIPropertyMetadata(default(Brush)));

        public Brush ThumbBackground
        {
            get { return (Brush)GetValue(ThumbBackgroundProperty); }
            set { SetValue(ThumbBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register(
                "HeaderBackground",
                typeof(Brush),
                typeof(CyberScrollView),
                new UIPropertyMetadata(default(Brush)));

        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        public CyberScrollView()
        {
            DefaultStyleKey = typeof(CyberScrollView);
        }
    }
}
