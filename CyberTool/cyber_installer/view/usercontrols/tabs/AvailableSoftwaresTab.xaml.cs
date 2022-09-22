using cyber_installer.view.usercontrols.tabs.@base;
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

namespace cyber_installer.view.usercontrols.tabs
{
    /// <summary>
    /// Interaction logic for AvailableSoftwaresTab.xaml
    /// </summary>
    public partial class AvailableSoftwaresTab : UserControl
    {
        public AvailableSoftwaresTab()
        {
            InitializeComponent();
            Loaded += HandleAvailableSoftwaresTabLoaded;
            Unloaded += HandleAvailableSoftwaresTabUnloaded;
        }

        private void HandleAvailableSoftwaresTabUnloaded(object sender, RoutedEventArgs e)
        {
            var context = DataContext as IAvailableTabContext;
            context?.OnLoaded(this);
        }

        private void HandleAvailableSoftwaresTabLoaded(object sender, RoutedEventArgs e)
        {
            var context = DataContext as IAvailableTabContext;
            context?.OnUnloaded(this);
        }
    }
}
