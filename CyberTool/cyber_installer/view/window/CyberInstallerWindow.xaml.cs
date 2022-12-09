﻿using cyber_base.implement.views.cyber_window;
using cyber_installer.implement.modules.update_manager;
using cyber_installer.view.usercontrols.tabs.@base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private StackPanel? _controlButtonPanel;
        private const string CIBS_PATH = "cibs/cibs.exe";
        private const string CIBS_CALLER_ID = "CyberInstallerWindow{0367E847-B5C3-4CDD-9C34-B78A769AF73C}";
        private const string CIBS_UPDATE_CYBER_INSTALLER_CMD = "UpdateCyberInstaller";

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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _controlButtonPanel = this.GetTemplateChild(WindowControlPanelName) as StackPanel;
           
            var parentOfUpdateButton = PART_UpdateButton.Parent as Panel;
            parentOfUpdateButton?.Children.Remove(PART_UpdateButton);
            _controlButtonPanel?.Children.Insert(0, PART_UpdateButton);
            PART_UpdateButton.Visibility = Visibility.Collapsed;
        }

        private void HandleUpdateButtonClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists(CIBS_PATH))
            {
                Process p = new Process();
                p.StartInfo.FileName = CIBS_PATH;
                var callerID = CIBS_CALLER_ID;
                var updateCmd = CIBS_UPDATE_CYBER_INSTALLER_CMD;
                var currentCIProcessID = Process.GetCurrentProcess().Id;
                var args = callerID + " " + updateCmd + " " + currentCIProcessID;
                p.StartInfo.Arguments = args;
                p.Start();
                Process.GetCurrentProcess().CloseMainWindow();
            }
            
        }
    }
}