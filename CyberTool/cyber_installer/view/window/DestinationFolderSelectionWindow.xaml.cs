using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_window;
using cyber_installer.model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace cyber_installer.view.window
{
    /// <summary>
    /// Interaction logic for SelectDestinationFolderWindow.xaml
    /// </summary>
    public partial class DestinationFolderSelectionWindow : CyberWindow
    {
        private static readonly double SIZE_CONVERT_TO_GB = Math.Pow(2, 30);
        private static readonly double SIZE_CONVERT_TO_MB = Math.Pow(2, 20);
        private static readonly double SIZE_CONVERT_TO_KB = Math.Pow(2, 10);

        private long _spaceRequire;

        public DestinationFolderSelectionWindow(ToolVO toolVO)
        {
            InitializeComponent();
            PART_ToolNameTextBlock.Text = "Choose the folder in which to install " + toolVO.Name;
            PART_ToolImage.Source = new BitmapImage(new Uri(toolVO.IconSource));
            _spaceRequire = toolVO.ToolVersions.Last().CompressLength + toolVO.ToolVersions.Last().RawLength;
            SetSpaceTextBlock(_spaceRequire, PART_SpaceRequireTextBlock);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var closeBtn = GetTemplateChild("CloseButton") as Button;
            if (closeBtn != null)
            {
                closeBtn.RemoveRoutedEventHandlers(Button.ClickEvent);
                closeBtn.Click += HandleClickEvent;
            }
        }

        public new string Show()
        {
            base.ShowDialog();
            return PART_DestinationFolderTextBox.Text;
        }

        public new string ShowDialog()
        {
            base.ShowDialog();
            return PART_DestinationFolderTextBox.Text;
        }

        public bool IsCreateDesktopShortcut()
        {
            return PART_CreateShortcutCheckbox.IsChecked ?? false;
        }

        public string GetDestinationFolderPath()
        {
            return PART_DestinationFolderTextBox.Text;
        }

        private void HandleClickEvent(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            switch (btn.Name)
            {
                case "PART_BrowseButton":
                    {
                        using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
                        {
                            System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                            {
                                PART_DestinationFolderTextBox.Text = fbd.SelectedPath;
                                SetAvailableSpaceTextBlock();
                            }
                        }
                        break;
                    }
                case "PART_NextButton":
                    {
                        this.Close();
                        break;
                    }
                case "PART_CancelButton":
                    {
                        PART_DestinationFolderTextBox.Text = "";
                        PART_SpaceAvailableTextBlock.Text = "";
                        this.Close();
                        break;
                    }
                case "CloseButton":
                    {
                        PART_DestinationFolderTextBox.Text = "";
                        PART_SpaceAvailableTextBlock.Text = "";
                        this.Close();
                        break;
                    }
            }
        }

        private void SetAvailableSpaceTextBlock()
        {
            long freeSpace = NativeMethods.GetFreeSpace(PART_DestinationFolderTextBox.Text);
            SetSpaceTextBlock(freeSpace, PART_SpaceAvailableTextBlock);

            if (freeSpace > _spaceRequire)
            {
                PART_SpaceAvailableTextBlock.Foreground = Application.Current.Resources["Foreground_Level3"] as SolidColorBrush;
            }
            else
            {
                PART_SpaceAvailableTextBlock.Foreground = Application.Current.Resources["Foreground_Level4"] as SolidColorBrush;
            }
        }

        private void SetSpaceTextBlock(long space, TextBlock spaceTextBlock)
        {
            double spaceConvertResult;
            if (space > SIZE_CONVERT_TO_GB)
            {
                spaceConvertResult = Math.Round(space / SIZE_CONVERT_TO_GB, 2);
                spaceTextBlock.Text = spaceConvertResult.ToString() + "GB";
            }
            else if (space > SIZE_CONVERT_TO_MB && space < SIZE_CONVERT_TO_GB)
            {
                spaceConvertResult = Math.Round(space / SIZE_CONVERT_TO_MB, 2);
                spaceTextBlock.Text = spaceConvertResult.ToString() + "MB";
            }
            else
            {
                spaceConvertResult = Math.Round(space / SIZE_CONVERT_TO_KB, 2);
                spaceTextBlock.Text = spaceConvertResult.ToString() + "KB";
            }
        }

        private void HandleMouseDownEvent(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var ele = sender as FrameworkElement;
            if (ele != null)
            {
                switch (ele.Name)
                {
                    case "PART_CreateDesktopShortcutDesTb":
                        {
                            PART_CreateShortcutCheckbox.IsChecked = !PART_CreateShortcutCheckbox.IsChecked;
                            break;
                        }
                }
            }
        }

        private void HandleMouseEnterEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ele = sender as FrameworkElement;
            if (ele != null)
            {
                switch (ele.Name)
                {
                    case "PART_CreateDesktopShortcutDesTb":
                        {
                            PART_CreateDesktopShortcutDesTb.TextDecorations = TextDecorations.Underline;
                            PART_CreateDesktopShortcutDesTb.Cursor = Cursors.Hand;
                            break;
                        }
                }
            }
        }

        private void HandleMouseLeaveEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ele = sender as FrameworkElement;
            if (ele != null)
            {
                switch (ele.Name)
                {
                    case "PART_CreateDesktopShortcutDesTb":
                        {
                            PART_CreateDesktopShortcutDesTb.TextDecorations = null;
                            PART_CreateDesktopShortcutDesTb.Cursor = Cursors.None;
                            break;
                        }
                }
            }
        }
    }
}
