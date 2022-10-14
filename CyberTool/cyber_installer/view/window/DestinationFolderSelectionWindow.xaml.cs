using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_window;
using System.Windows;
using System.Windows.Controls;

namespace cyber_installer.view.window
{
    /// <summary>
    /// Interaction logic for SelectDestinationFolderWindow.xaml
    /// </summary>
    public partial class DestinationFolderSelectionWindow : CyberWindow
    {
        public DestinationFolderSelectionWindow()
        {
            InitializeComponent();
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
                        this.Close();
                        break;
                    }
                case "CloseButton":
                    {
                        PART_DestinationFolderTextBox.Text = "";
                        this.Close();
                        break;
                    }
            }
        }

    }
}
