﻿using cyber_installer.view.usercontrols.list_item.available_item.@base;
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

namespace cyber_installer.view.usercontrols.list_item.available_item
{
    /// <summary>
    /// Interaction logic for AvailableItem.xaml
    /// </summary>
    public partial class AvailableItemViewHolder : UserControl
    {
        #region ItemStatus
        public static readonly DependencyProperty ItemStatusProperty =
            DependencyProperty.Register(
                "ItemStatus",
                typeof(ItemStatus),
                typeof(AvailableItemViewHolder),
                new PropertyMetadata(ItemStatus.None, new PropertyChangedCallback(OnItemStatusChangedCallback)));

        private static void OnItemStatusChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as AvailableItemViewHolder;
            ctrl?.OnItemStatusChanged((ItemStatus)e.OldValue, (ItemStatus)e.NewValue);
        }

        public ItemStatus ItemStatus
        {
            get => (ItemStatus)GetValue(ItemStatusProperty);
            set => SetValue(ItemStatusProperty, value);
        }
        #endregion
        
        public AvailableItemViewHolder()
        {
            InitializeComponent();
        }

        private void OnItemStatusChanged(ItemStatus oldStatus, ItemStatus newStatus)
        {
            PART_InstallSoftwareButton.Visibility = Visibility.Hidden;
            PART_UpdateSoftwareButton.Visibility = Visibility.Hidden;
            PART_SwHandlingProgressPanel.Visibility = Visibility.Hidden;
            switch (newStatus)
            {
                case ItemStatus.Downloadable:
                    PART_InstallSoftwareButton.Visibility = Visibility.Visible;
                    break;
                case ItemStatus.Updateable:
                    PART_UpdateSoftwareButton.Visibility = Visibility.Visible;
                    break;
                case ItemStatus.Installable:
                    PART_InstallSoftwareButton.Visibility = Visibility.Visible;
                    break;
                case ItemStatus.Downloading:
                    PART_SwHandlingProgressPanel.Visibility = Visibility.Visible;
                    PART_HandlingTitleTextBlock.Text = "Downloading";
                    break;
                case ItemStatus.Installing:
                    PART_SwHandlingProgressPanel.Visibility = Visibility.Visible;
                    PART_HandlingTitleTextBlock.Text = "Installing";
                    break;
            }
        }

        
    }
}
