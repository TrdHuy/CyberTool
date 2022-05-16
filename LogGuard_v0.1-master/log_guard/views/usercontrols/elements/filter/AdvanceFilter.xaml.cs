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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace log_guard.views.usercontrols.elements.filter
{
    /// <summary>
    /// Interaction logic for UC_AdvanceFilter.xaml
    /// </summary>
    public partial class AdvanceFilter : UserControl
    {
        public AdvanceFilter()
        {
            InitializeComponent();
        }

        public void BeginChangeLevelOfRadialPBAnimation()
        {
            if (LogLevelRadialPB == null) return;
            var animTime = 300;
            var destination = LogPercentGrid.ActualWidth + 100;


            DoubleAnimation leaveAnimation = new DoubleAnimation();
            leaveAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animTime / 2));
            leaveAnimation.From = 0;
            leaveAnimation.To = destination;
            leaveAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
            Storyboard.SetTargetName(leaveAnimation, "RadialPBTranslateTransform");
            Storyboard.SetTargetProperty(leaveAnimation,
               new PropertyPath(TranslateTransform.XProperty));

            DoubleAnimation enterAnimation = new DoubleAnimation();
            enterAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animTime / 2));
            enterAnimation.From = -destination;
            enterAnimation.BeginTime = TimeSpan.FromMilliseconds(animTime / 2);
            enterAnimation.To = 0;
            Storyboard.SetTargetName(enterAnimation, "RadialPBTranslateTransform");
            Storyboard.SetTargetProperty(enterAnimation,
               new PropertyPath(TranslateTransform.XProperty));

            Color bgColor = GetPathColor("1");
            Color valColor = GetPathColor("2");

            ColorAnimation radialPBBrushAnimation
                = new ColorAnimation();
            radialPBBrushAnimation.Duration = TimeSpan.FromMilliseconds(1);
            radialPBBrushAnimation.To = bgColor;
            radialPBBrushAnimation.BeginTime = TimeSpan.FromMilliseconds(animTime / 2);
            Storyboard.SetTargetName(radialPBBrushAnimation, "RadialPBBrush");
            Storyboard.SetTargetProperty(
                radialPBBrushAnimation, new PropertyPath(SolidColorBrush.ColorProperty));


            ColorAnimation radialPBPercentContForegroundBrushAnimation
                = new ColorAnimation();
            radialPBPercentContForegroundBrushAnimation.Duration = TimeSpan.FromMilliseconds(1);
            radialPBPercentContForegroundBrushAnimation.To = valColor;
            radialPBPercentContForegroundBrushAnimation.BeginTime = TimeSpan.FromMilliseconds(animTime / 2);
            Storyboard.SetTargetName(radialPBPercentContForegroundBrushAnimation, "RadialPBPercentContForegroundBrush");
            Storyboard.SetTargetProperty(
                radialPBPercentContForegroundBrushAnimation, new PropertyPath(SolidColorBrush.ColorProperty));

            ColorAnimation radialPBValPathBackgroundBrushAnimation
               = new ColorAnimation();
            radialPBValPathBackgroundBrushAnimation.Duration = TimeSpan.FromMilliseconds(1);
            radialPBValPathBackgroundBrushAnimation.To = valColor;
            radialPBValPathBackgroundBrushAnimation.BeginTime = TimeSpan.FromMilliseconds(animTime / 2);
            Storyboard.SetTargetName(radialPBValPathBackgroundBrushAnimation, "RadialPBValPathBackgroundBrush");
            Storyboard.SetTargetProperty(
                radialPBValPathBackgroundBrushAnimation, new PropertyPath(SolidColorBrush.ColorProperty));


            Storyboard sb = new Storyboard();
            //sb.Children.Add(fadeInAnimation);
            //sb.Children.Add(fadeOutAnimation);
            sb.Children.Add(radialPBValPathBackgroundBrushAnimation);
            sb.Children.Add(radialPBPercentContForegroundBrushAnimation);
            sb.Children.Add(radialPBBrushAnimation);
            sb.Children.Add(leaveAnimation);
            sb.Children.Add(enterAnimation);
            sb.Begin(LogLevelRadialPB);

        }

        private void RadialCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            BeginChangeLevelOfRadialPBAnimation();
        }


        private Color GetPathColor(string level)
        {
            var brush = Application.Current.Resources["DebugLevelForeground_Level" + level] as System.Windows.Media.SolidColorBrush;
            if ((bool)InfoRadialCheckBox.IsChecked)
            {
                brush = Application.Current.Resources["InfoLevelForeground_Level" + level] as System.Windows.Media.SolidColorBrush;
            }
            else if ((bool)DebugRadialCheckBox.IsChecked)
            {
                brush = Application.Current.Resources["DebugLevelForeground_Level" + level] as System.Windows.Media.SolidColorBrush;
            }
            else if ((bool)VerboseRadialCheckBox.IsChecked)
            {
                brush = Application.Current.Resources["VerboseLevelForeground_Level" + level] as System.Windows.Media.SolidColorBrush;
            }
            else if ((bool)FatalRadialCheckBox.IsChecked)
            {
                brush = Application.Current.Resources["FatalLevelForeground_Level" + level] as System.Windows.Media.SolidColorBrush;
            }
            else if ((bool)WarningRadialCheckBox.IsChecked)
            {
                brush = Application.Current.Resources["WarningLevelForeground_Level" + level] as System.Windows.Media.SolidColorBrush;
            }
            else if ((bool)ErrorRadialCheckBox.IsChecked)
            {
                brush = Application.Current.Resources["ErrorLevelForeground_Level" + level] as System.Windows.Media.SolidColorBrush;
            }
            return brush.Color;
        }

        /// <summary>
        /// Hàm này để tránh trường hợp bug binding đến từ .Net 4.7.2
        /// Khi người dùng nhập liên tục trong trường search, UpdateSourceTrigger=Propertychanged
        /// hoạt động không chính xác, có thể lỗi đến từ .Net
        /// nhưng khi gán thêm sự kiện textchagned này cho textbox thì nó sẽ hoạt động bình thường
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChangeEventToAvoidDotNetBindingBug(object sender, TextChangedEventArgs e)
        {
            int a = 1;
        }
    }
}
