using extension_manager_service.views.elements.plugin_browser.items.@base;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace extension_manager_service.views.elements.plugin_browser.items
{
    /// <summary>
    /// Interaction logic for PluginItemViewHolder.xaml
    /// </summary>
    public partial class StretchPluginItemViewHolder : UserControl
    {

        private readonly string EMS_FULL_STAR_RATE = "m -235.97277,-18.179234 a 2.9830526,3.1073536 0 0 0 -2.94177,1.79937 l -4.75553,10.7681903 a 3.9277965,4.0914639 0 0 1 -2.92495,2.326159 l -11.93194,2.063931 a 2.9830181,3.1073176 0 0 0 -1.53454,5.3484711 l 5.41093,5.2018799 a 11.501122,11.980363 0 0 1 3.57252,10.5900647 l -1.20431,8.322308 a 2.9829763,3.107274 0 0 0 4.40904,3.17299 l 6.42179,-3.753506 a 11.500618,11.979837 0 0 1 10.77308,-0.266315 l 7.22522,3.764602 a 2.9828861,3.1071801 0 0 0 4.25989,-3.387324 l -2.22918,-11.623196 a 3.927704,4.0913676 0 0 1 1.05348,-3.672914 l 8.48951,-8.9752436 a 2.9830145,3.1073138 0 0 0 -1.77672,-5.2672931 l -7.31326,-0.894719 a 11.501176,11.980419 0 0 1 -8.86572,-6.3804403 l -3.72165,-7.470223 a 2.9830526,3.1073536 0 0 0 -2.41589,-1.666792 z";
        private readonly string EMS_HALF_STAR_RATE = "m -183.13347,-19.383129 a 2.9830532,3.1076334 0 0 0 -2.89388,1.798612 l -4.75527,10.7690708 a 3.9277972,4.0918324 0 0 1 -2.92488,2.3261924 l -11.93209,2.0640172 a 2.9830186,3.1075973 0 0 0 -1.53428,5.3490032 l 5.41053,5.2025741 a 11.501124,11.981441 0 0 1 3.57291,10.5908753 l -1.20458,8.323364 a 2.9829768,3.1075538 0 0 0 4.40903,3.17301 l 6.42183,-3.753886 a 11.50062,11.980916 0 0 1 5.43068,-1.513289 v 0.0022 c 1.29944,0.109441 2.58439,0.458697 3.78478,1.055695 l 7.23315,3.595074 c 2.96653,1.475643 6.4628,-1.179001 5.81308,-4.412821 l -2.22932,-11.101228 c -0.18513,-0.921075 0.0987,-1.872387 0.76171,-2.54153 l 8.49819,-8.5726168 c 2.32877,-2.34898933 0.86926,-6.4766164 -2.42569,-6.8612175 l -7.31945,-0.8543548 c -3.43507,-0.4014776 -6.44366,-2.4676763 -8.03672,-5.5196589 l -3.72432,-7.137388 c -0.49739,-0.952971 -1.35128,-1.656757 -2.35541,-1.981652 z m 0,2.434939 c 0.1718,0.151187 0.31766,0.332948 0.42684,0.542115 l 3.72484,7.1352328 c 1.92366,3.6853822 5.56204,6.1863793 9.71,6.6711812 l 7.32152,0.8538172 c 1.64087,0.1915291 2.28988,2.03067563 1.13016,3.2004657 l -8.49612,8.5758471 c -1.16681,1.17751 -1.67248,2.854966 -1.34669,4.475806 l 2.23087,11.103381 c 0.32355,1.610351 -1.23523,2.792926 -2.71249,2.058096 l -7.23057,-3.595073 h -0.002 c -1.5057,-0.74853 -3.12343,-1.172645 -4.7563,-1.286108 z";
        private readonly string EMS_EMPTY_STAR_RATE = "m -132.48633,-73.304687 c -1.56223,-0.121715 -3.04241,0.746351 -3.69726,2.169921 l -4.38477,9.529297 c -0.36365,0.790423 -1.09286,1.345779 -1.95117,1.488282 l -10.99805,1.828125 c -3.01287,0.500553 -4.17185,4.381439 -1.92773,6.453125 l 4.98633,4.603515 c 2.34061,2.160099 3.45723,5.336877 2.98242,8.486328 l -1.10938,7.363282 c -0.45505,3.020133 2.87726,5.322331 5.54102,3.828125 L -137.125,-30.875 c 2.77774,-1.558576 6.14364,-1.637945 8.99219,-0.212891 l 6.66015,3.330079 c 2.73179,1.366902 5.95183,-1.092375 5.35352,-4.087891 l -2.05274,-10.283203 c -0.17048,-0.8532 0.0906,-1.733683 0.70118,-2.353516 l 7.82617,-7.941406 c 2.14449,-2.175889 0.79983,-5.99921 -2.23438,-6.355469 l -6.74023,-0.791015 c -3.16325,-0.371892 -5.9334,-2.286202 -7.40039,-5.113282 l -3.42969,-6.611328 c -0.5961,-1.148819 -1.74676,-1.909131 -3.03711,-2.009765 z m -0.15625,1.99414 c 0.60407,0.04711 1.13891,0.399684 1.41797,0.9375 l 3.42969,6.609375 c 1.77144,3.413804 5.12167,5.730614 8.9414,6.179688 l 6.74219,0.791015 c 1.51103,0.177416 2.10897,1.881258 1.04102,2.964844 l -7.82422,7.943359 c -1.07448,1.090737 -1.54025,2.645086 -1.24024,4.146485 l 2.05469,10.285156 c 0.29794,1.491682 -1.13769,2.586929 -2.49805,1.90625 l -6.6582,-3.330078 h -0.002 c -3.43941,-1.719955 -7.51152,-1.623938 -10.86524,0.257812 l -5.91992,3.320313 c -1.32646,0.744064 -2.81058,-0.281232 -2.58398,-1.785156 l 1.10937,-7.365235 c 0.57336,-3.803082 -0.77713,-7.645504 -3.60351,-10.253906 l -4.98828,-4.603516 c -1.1175,-1.031634 -0.59993,-2.760505 0.90039,-3.009765 l 10.99804,-1.826172 c 1.51038,-0.250763 2.8015,-1.236044 3.44141,-2.626953 l 4.38281,-9.529297 c 0.30762,-0.668733 0.99074,-1.068895 1.72461,-1.011719 z";

        #region ItemBorder
        public static readonly DependencyProperty ItemBorderProperty =
            DependencyProperty.Register(
                "ItemBorder",
                typeof(Brush),
                typeof(StretchPluginItemViewHolder),
                new PropertyMetadata(default(Brush)));

        public Brush ItemBorder
        {
            get => (Brush)GetValue(ItemBorderProperty);
            set => SetValue(ItemBorderProperty, value);
        }
        #endregion

        #region ItemBackground
        public static readonly DependencyProperty ItemBackgroundProperty =
            DependencyProperty.Register(
                "ItemBackground",
                typeof(Brush),
                typeof(StretchPluginItemViewHolder),
                new PropertyMetadata(default(Brush)));

        public Brush ItemBackground
        {
            get => (Brush)GetValue(ItemBackgroundProperty);
            set => SetValue(ItemBackgroundProperty, value);
        }
        #endregion

        #region Rates
        public static readonly DependencyProperty RatesProperty =
            DependencyProperty.Register(
                "Rates",
                typeof(double),
                typeof(StretchPluginItemViewHolder),
                new PropertyMetadata(0d, new PropertyChangedCallback(OnRatesChangedCallback)));

        private static void OnRatesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as StretchPluginItemViewHolder;
            ctrl?.DrawRateStars((double)e.NewValue);
        }

        public double Rates
        {
            get => (double)GetValue(RatesProperty);
            set => SetValue(RatesProperty, value);
        }
        #endregion

        #region ViewMode
        public static readonly DependencyProperty ViewModeProperty =
            DependencyProperty.Register(
                "ViewMode",
                typeof(PluginItemViewMode),
                typeof(StretchPluginItemViewHolder),
                new PropertyMetadata(PluginItemViewMode.Full, new PropertyChangedCallback(OnViewModeChangeCallback)));

        private static void OnViewModeChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as StretchPluginItemViewHolder;
            ctrl?.UpdateCurrentViewMode((PluginItemViewMode)e.NewValue);
        }

        public PluginItemViewMode ViewMode
        {
            get => (PluginItemViewMode)GetValue(ViewModeProperty);
            set => SetValue(ViewModeProperty, value);
        }
        #endregion

        public StretchPluginItemViewHolder()
        {
            InitializeComponent();
            DrawRateStars(Rates);
        }

        private void DrawRateStars(double rates)
        {
            var newRoundRate = Math.Round(rates * 2, MidpointRounding.AwayFromZero) / 2;
            PART_RatePanel.Children.Clear();
            for (int i = 1; i <= 5; i++)
            {
                var star = new Path()
                {
                    Margin = new Thickness(3),
                    Width = 10.56,
                    Height = 10.56,
                    Stretch = Stretch.Uniform,
                };

                if (i <= newRoundRate)
                {
                    star.SetResourceReference(Path.FillProperty, "ObjectFill_Type1");
                    star.Data = Geometry.Parse(EMS_FULL_STAR_RATE);
                }
                else if (i > newRoundRate && i - newRoundRate == 0.5d)
                {
                    star.SetResourceReference(Path.FillProperty, "ObjectFill_Type1");
                    star.Data = Geometry.Parse(EMS_HALF_STAR_RATE);
                }
                else
                {
                    star.SetResourceReference(Path.FillProperty, "Foreground_Level1");
                    star.Data = Geometry.Parse(EMS_EMPTY_STAR_RATE);
                }
                PART_RatePanel.Children.Add(star);
            }
        }

        private void UpdateCurrentViewMode(PluginItemViewMode newMode)
        {
            switch (newMode)
            {
                case PluginItemViewMode.Half:
                    PART_DownloadsTB.Visibility = Visibility.Collapsed;
                    PART_InstallButton.Visibility = Visibility.Collapsed;
                    PART_RatePanel.Visibility = Visibility.Collapsed;
                    PART_InstallButton2.Visibility = Visibility.Visible;
                    break;
                case PluginItemViewMode.Full:
                    PART_DownloadsTB.Visibility = Visibility.Visible;
                    PART_InstallButton.Visibility = Visibility.Visible;
                    PART_RatePanel.Visibility = Visibility.Visible;
                    PART_InstallButton2.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }


}
