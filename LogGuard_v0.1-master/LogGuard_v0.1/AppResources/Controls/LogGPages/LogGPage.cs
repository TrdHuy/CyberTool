using LogGuard_v0._1.Base.ViewModel;
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
            var pageVM = DataContext as IPageViewModel;
            pageVM?.OnUnloaded();
        }

        private void LogGPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var pageVM = DataContext as IPageViewModel;
            pageVM?.OnLoaded();
        }
    }
}
