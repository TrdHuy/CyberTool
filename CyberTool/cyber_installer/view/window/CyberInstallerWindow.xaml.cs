using cyber_base.implement.views.cyber_window;
using cyber_installer.implement.modules.update_manager;
using cyber_installer.implement.modules.utils;
using cyber_installer.view.usercontrols.tabs.@base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
        private StackPanel? _controlButtonPanel;

        public CyberInstallerWindow()
        {
            InitializeComponent();
            CyberInstallerUpdateManager.Current.IsUpdateableChanged += HandleUpdateableEventChanged;
        }

        private void HandleUpdateableEventChanged(object sender, bool isUpdateable)
        {
            PART_UpdateButton.Visibility = Visibility.Visible;
            PART_AvailableUpdatePopup.IsOpen = true;
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            var offset = PART_AvailableUpdatePopup.HorizontalOffset;
            PART_AvailableUpdatePopup.HorizontalOffset = offset + 1;
            PART_AvailableUpdatePopup.HorizontalOffset = offset;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _controlButtonPanel = this.GetTemplateChild(WindowControlPanelName) as StackPanel;

            var parentOfUpdateButton = PART_UpdateButton.Parent as Panel;
            parentOfUpdateButton?.Children.Remove(PART_UpdateButton);
            _controlButtonPanel?.Children.Insert(0, PART_UpdateButton);
            PART_UpdateButton.Visibility = Visibility.Collapsed;
            PART_UpdateButton.ToolTipOpening += HandleToolTipOpeningEvent;
        }

        private void HandleToolTipOpeningEvent(object sender, ToolTipEventArgs e)
        {
            var btn = sender as FrameworkElement;
            switch (btn?.Name)
            {
                case "PART_UpdateButton":
                    {
                        PART_AvailableUpdatePopup.IsOpen = true;
                        e.Handled = true;
                        break;
                    }
            }
        }

        private async void HandleButtonClickEvent(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            switch (btn?.Name)
            {
                case "PART_CloseUpdatePopupButton":
                    {
                        PART_AvailableUpdatePopup.IsOpen = false;
                        break;
                    }
                case "PART_UpdateButton":
                    {
                        await CyberInstallerUpdateManager.Current.UpdateLatestCyberInstallerVersion();
                        break;
                    }
            }
        }
    }
}
