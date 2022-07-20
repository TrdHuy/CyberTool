using cyber_base.implement.extension;
using cyber_base.implement.views.cyber_scroll;
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

namespace honeyboard_release_service.views.elements.log_monitor
{
    /// <summary>
    /// Interaction logic for LogMonitor.xaml
    /// </summary>
    public partial class LogMonitor : UserControl
    {
        public LogMonitor()
        {
            InitializeComponent();
            this.ConsoleTextBox.TextChanged += (sender, e) => ConsoleTextBox.ScrollToEnd();
            this.ConsoleTextBox.SelectionChanged += (sender, e) => MoveCustomCaret();
            this.ConsoleTextBox.LostFocus += (sender, e) => Caret.Visibility = Visibility.Collapsed;
            this.ConsoleTextBox.GotFocus += (sender, e) => Caret.Visibility = Visibility.Visible;
            
            Loaded += LogMonitor_Loaded;
        }

        private void LogMonitor_Loaded(object sender, RoutedEventArgs e)
        {
            var sc = ConsoleTextBox.FindChild<CyberScrollView>("PART_ContentHost");
            if (sc != null)
            {
                sc.ScrollChanged += (s, e) => MoveCustomCaret();
            }

        }

        private void MoveCustomCaret()
        {
            var caretLocation = ConsoleTextBox.GetRectFromCharacterIndex(ConsoleTextBox.CaretIndex).Location;

            if (!double.IsInfinity(caretLocation.X))
            {
                Canvas.SetLeft(Caret, caretLocation.X);
            }

            if (!double.IsInfinity(caretLocation.Y))
            {
                Canvas.SetTop(Caret, caretLocation.Y + 10);
            }
        }
    }
}
