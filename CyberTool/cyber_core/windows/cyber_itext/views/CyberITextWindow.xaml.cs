using cyber_base.implement.views.cyber_window;
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

namespace cyber_core.windows.cyber_itext.views
{
    /// <summary>
    /// Interaction logic for CyberITextWindow.xaml
    /// </summary>
    public partial class CyberITextWindow : CyberWindow
    {
        private const string CloseButtonName = "CloseButton";

        private string _oldText;
        private string _revision;

        public CyberITextWindow(string oldText
            , bool isMultilines = false)
        {
            InitializeComponent();

            _oldText = oldText;
            _revision = oldText;
            PART_SearchTextBox.Text = oldText;

            if (isMultilines)
            {
                PART_SearchTextBox.AcceptsReturn = true;
                PART_SearchTextBox.MaxWidth = 500;
            }

            PART_SearchTextBox.TextChanged += (s, e) =>
            {
                _revision = PART_SearchTextBox.Text;
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var closeBtn = GetTemplateChild(CloseButtonName) as Button;

            if (closeBtn != null)
            {
                closeBtn.Click += (s, e) =>
                {
                    _revision = _oldText;
                };
            }
        }

        public new string ShowDialog()
        {
            base.ShowDialog();
            return _revision;
        }

        public new string Show()
        {
            base.ShowDialog();
            return _revision;
        }

        private void HandleButtonClickEvent(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                switch (btn.Name)
                {
                    case "PART_RedoButton":
                        PART_SearchTextBox.Text = _oldText;
                        break;
                    case "PART_ModifyBtn":
                        this.Close();
                        break;
                    case "PART_AbortBtn":
                        _revision = _oldText;
                        this.Close();
                        break;
                }
            }
        }

    }
}
