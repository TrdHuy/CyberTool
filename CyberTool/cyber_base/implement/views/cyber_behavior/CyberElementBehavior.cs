using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;

namespace cyber_base.implement.views.cyber_behavior
{
    public class CyberElementBehavior
    {
        public static double GetSmoothValue(DependencyObject obj)
        {
            return (double)obj.GetValue(SmoothValueProperty);
        }

        public static void SetSmoothValue(DependencyObject obj, double value)
        {
            obj.SetValue(SmoothValueProperty, value);
        }

        public static readonly DependencyProperty SmoothValueProperty =
            DependencyProperty.RegisterAttached("SmoothValue", typeof(double), typeof(CyberElementBehavior), new PropertyMetadata(0.0, OnProgressSmootherChanged));

        private static void OnProgressSmootherChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var anim = new DoubleAnimation((double)e.OldValue, (double)e.NewValue, new TimeSpan(0, 0, 0, 0, 600));

            // Nếu có nhiều sự thay đổi diễn ra đồng thời khi animation trước chưa kết thúc
            // animation sau sẽ đươc compose vào animation trước 
            // Sử dụng thuộc tính HadnoffBehavior
            (d as ProgressBar)?.BeginAnimation(ProgressBar.ValueProperty, anim, HandoffBehavior.Compose);
        }
    }
}
