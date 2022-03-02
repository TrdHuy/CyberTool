using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LogGuard_v0._1.LogGuard.Control
{
    public class ScrollViewWatcher: ScrollViewer
    {

        public static readonly DependencyProperty ScrollBarBackgroundProperty =
            DependencyProperty.Register(
                "ScrollBarBackground",
                typeof(Brush),
                typeof(ScrollViewWatcher),
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
                typeof(ScrollViewWatcher),
                new UIPropertyMetadata(default(Brush)));

        public Brush ThumbBackground
        {
            get { return (Brush)GetValue(ThumbBackgroundProperty); }
            set { SetValue(ThumbBackgroundProperty, value); }
        }

        public ScrollViewWatcher()
        {
            DefaultStyleKey = typeof(ScrollViewWatcher);
        }
    }
}
