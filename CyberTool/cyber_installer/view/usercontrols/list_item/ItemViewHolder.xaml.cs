using cyber_installer.view.usercontrols.list_item.available_item.@base;
using cyber_installer.view.window;
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
using static cyber_base.implement.views.cyber_anim.LoadingAnimation;

namespace cyber_installer.view.usercontrols.list_item
{
    /// <summary>
    /// Interaction logic for ItemViewHolder.xaml
    /// </summary>
    public partial class ItemViewHolder : UserControl
    {
        #region ItemStatus
        public static readonly DependencyProperty ItemStatusProperty =
            DependencyProperty.Register(
                "ItemStatus",
                typeof(ItemStatus),
                typeof(ItemViewHolder),
                new PropertyMetadata(ItemStatus.None, new PropertyChangedCallback(OnItemStatusChangedCallback)));

        private static void OnItemStatusChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ItemViewHolder;
            ctrl?.OnItemStatusChanged((ItemStatus)e.OldValue, (ItemStatus)e.NewValue);
        }

        public ItemStatus ItemStatus
        {
            get => (ItemStatus)GetValue(ItemStatusProperty);
            set => SetValue(ItemStatusProperty, value);
        }
        #endregion

        public ItemViewHolder()
        {
            InitializeComponent();
        }

        private void OnItemStatusChanged(ItemStatus oldStatus, ItemStatus newStatus)
        {
            PART_SingleSoftwareCommandButton.Visibility = Visibility.Hidden;
            PART_SwHandlingProgressPanel.Visibility = Visibility.Hidden;
            PART_OtherItemStatusContentTb.Visibility = Visibility.Hidden;
            switch (newStatus)
            {
                case ItemStatus.Downloadable:
                    PART_SingleSoftwareCommandButton.Visibility = Visibility.Visible;
                    PART_SingleSoftwareCommandButton.Content = "Download & Install";
                    break;
                case ItemStatus.Updateable:
                    PART_SingleSoftwareCommandButton.Visibility = Visibility.Visible;
                    PART_SingleSoftwareCommandButton.Content = "Update";
                    break;
                case ItemStatus.Installable:
                    PART_SingleSoftwareCommandButton.Visibility = Visibility.Visible;
                    PART_SingleSoftwareCommandButton.Content = "Install";
                    break;
                case ItemStatus.Downloading:
                    PART_SwHandlingProgressPanel.Visibility = Visibility.Visible;
                    PART_HandlingTitleTextBlock.Text = "Downloading";
                    break;
                case ItemStatus.Installing:
                    PART_SwHandlingProgressPanel.Visibility = Visibility.Visible;
                    PART_HandlingTitleTextBlock.Text = "Installing";
                    break;
                case ItemStatus.Installed:
                    PART_SingleSoftwareCommandButton.Visibility = Visibility.Visible;
                    PART_SingleSoftwareCommandButton.Content = "Uninstall";
                    break;
                case ItemStatus.Uninstalling:
                    PART_SwHandlingProgressPanel.Visibility = Visibility.Visible;
                    PART_HandlingTitleTextBlock.Text = "Uninstalling";
                    break;
                case ItemStatus.UpToDate:
                    PART_OtherItemStatusContentTb.Visibility = Visibility.Visible;
                    PART_OtherItemStatusContentTb.Text = "Up to date";
                    break;
                case ItemStatus.InstallFailed:
                    PART_SingleSoftwareCommandButton.Visibility = Visibility.Visible;
                    PART_SingleSoftwareCommandButton.Content = "Re-install";
                    break;
            }
        }

        private void HandleBusyStatusChanged(object sender, IsBusyChangedEventArgs args)
        {
            if (args.NewValue)
            {
                PART_ItemStatusPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                PART_ItemStatusPanel.Visibility = Visibility.Visible;
            }
        }

    }
}