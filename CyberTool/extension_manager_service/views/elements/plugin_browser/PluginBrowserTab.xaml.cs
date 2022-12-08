using cyber_base.implement.views.cyber_scroll;
using extension_manager_service.@base.view_model;
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

namespace extension_manager_service.views.elements.plugin_browser
{
    /// <summary>
    /// Interaction logic for PluginBrowserTab.xaml
    /// </summary>
    public partial class PluginBrowserTab : UserControl
    {
        #region ViewMode
        public static readonly DependencyProperty ViewModeProperty =
            DependencyProperty.RegisterAttached(
                "ViewMode",
                typeof(PluginItemViewMode),
                typeof(PluginBrowserTab),
                new PropertyMetadata(PluginItemViewMode.Full, new PropertyChangedCallback(OnViewModeChangedCallback)));

        private static void OnViewModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lv = d as ListView;
            if (lv != null && lv.Name == "PART_PluginListView")
            {
                if ((PluginItemViewMode)e.NewValue == PluginItemViewMode.Full)
                {
                    lv.SetValue(Grid.ColumnSpanProperty, 3);
                }
                else if ((PluginItemViewMode)e.NewValue == PluginItemViewMode.Half)
                {
                    lv.SetValue(Grid.ColumnSpanProperty, 1);
                }
            }
        }

        public static PluginItemViewMode GetViewMode(UIElement target) =>
            (PluginItemViewMode)target.GetValue(ViewModeProperty);
        public static void SetViewMode(UIElement target, PluginItemViewMode value) =>
            target.SetValue(ViewModeProperty, value);
        #endregion

        public PluginBrowserTab()
        {
            InitializeComponent();
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void HandleScrollChangeEvent(object sender, ScrollChangedEventArgs e)
        {
            var scrollView = sender as CyberScrollView;
            if (scrollView != null)
            {
                switch (scrollView.Name)
                {
                    case "PluginScroller":
                        if (e.VerticalChange > 0)
                        {
                            if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                            {
                                (DataContext as IEMSScrollViewModel)?.OnScrollDownToBottom(scrollView);
                            }
                        }
                        break;
                }
            }
        }
    }
}
