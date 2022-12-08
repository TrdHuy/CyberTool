using cyber_base.implement.views.cyber_scroll;
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
    /// Interaction logic for InstalledSofwaresTab.xaml
    /// </summary>
    public partial class InstalledSofwaresTab : UserControl
    {
        public InstalledSofwaresTab()
        {
            InitializeComponent();
        }

        private void HandleScrollChangeEvent(object sender, ScrollChangedEventArgs e)
        {
            var scrollView = sender as CyberScrollView;
            if (scrollView != null)
            {
                switch (scrollView.Name)
                {
                    case "ToolsScroller":
                        if (e.VerticalChange > 0)
                        {
                            if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                            {
                                (DataContext as IInstalledTabContext)?.OnScrollDownToBottom(scrollView);
                            }
                        }
                        break;
                }
            }
        }
    }
}
