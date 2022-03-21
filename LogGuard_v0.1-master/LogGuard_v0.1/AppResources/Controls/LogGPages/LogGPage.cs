using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Base.ViewModel.ViewModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LogGuard_v0._1.AppResources.Controls.LogGPages
{
    public class LogGPage : Page
    {
        public LogGPage() :base()
        {
            Loaded += LogGPage_Loaded;
            Unloaded += LogGPage_Unloaded;
        }

        private void LogGPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            VMManagerMarkupExtension.OnPageViewModelUnloaded(DataContext?.GetType());
        }

        private void LogGPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            VMManagerMarkupExtension.OnPageViewModelLoaded(DataContext?.GetType());
        }
    }
}
