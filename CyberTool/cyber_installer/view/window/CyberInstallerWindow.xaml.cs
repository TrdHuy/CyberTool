using cyber_base.implement.views.cyber_window;
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
using System.Windows.Shapes;

namespace cyber_installer.view.window
{
    /// <summary>
    /// Interaction logic for CyberInstallerWindow.xaml
    /// </summary>
    public partial class CyberInstallerWindow : CyberWindow
    {
        public CyberInstallerWindow()
        {
            InitializeComponent();
        }

        private void HandleLoadedEvent(object sender, RoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            switch (item?.Name)
            {
                case "PART_AvailableSoftwaresTab":
                    {
                        if (PART_TabControl.SelectedItem == PART_AvailableSoftwaresTabItem)
                        {
                            var context = PART_AvailableSoftwaresTab.DataContext as IAvailableTabContext;
                            context?.OnTabOpened(PART_AvailableSoftwaresTab);
                        }
                        break;
                    }
            }
        }

        private void HandleUnloadedEvent(object sender, RoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            switch (item?.Name)
            {
                case "PART_AvailableSoftwaresTab":
                    {
                        var context = PART_AvailableSoftwaresTab.DataContext as IAvailableTabContext;
                        context?.OnTabClosed(PART_AvailableSoftwaresTab);
                        break;
                    }
            }
        }
    }
}
