using cyber_base.implement.views.cyber_scroll;
using cyber_installer.view.usercontrols.tabs.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cyber_installer.view.usercontrols.tabs
{
    public abstract class BaseSoftwaresStatusTab : UserControl
    {
        protected abstract ListView SoftwareListView { get; }

        public BaseSoftwaresStatusTab()
        {
            Loaded += HandleLoadedEvenet;
            Unloaded += HandleUnloadedEvenet;
            Initialized += HandleInitializedEvent;
        }

        private void HandleUnloadedEvenet(object sender, RoutedEventArgs e)
        {
            var context = DataContext as ISoftwareStatusTabContext;
            context?.OnTabClosed(this);
        }

        private void HandleInitializedEvent(object? sender, EventArgs e)
        {
            SoftwareListView.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(HandleScrollChangeEvent));
        }

        private void HandleLoadedEvenet(object sender, RoutedEventArgs e)
        {
            var context = DataContext as ISoftwareStatusTabContext;
            context?.OnTabOpened(this);
        }

        public void HandleScrollChangeEvent(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0)
            {
                if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                {
                    (DataContext as ISoftwareStatusTabContext)?.OnScrollDownToBottom(sender);
                }
            }
        }
    }
}
