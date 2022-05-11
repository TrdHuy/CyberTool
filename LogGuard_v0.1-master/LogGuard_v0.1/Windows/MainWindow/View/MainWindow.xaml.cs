using LogGuard_v0._1.AppResources.Controls.LogGWindows;

namespace LogGuard_v0._1.Windows.MainWindow.View
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : LogGuardWindow
    {
        public MainWindow() : base()
        {
            InitializeComponent();
        }

        private void mainFrame_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            int a = 1;
        }

        private void mainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            int a = 1;

        }

        private void mainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            int a = 1;

        }

        private void mainFrame_NavigationProgress(object sender, System.Windows.Navigation.NavigationProgressEventArgs e)
        {
            int a = 1;

        }
    }
}