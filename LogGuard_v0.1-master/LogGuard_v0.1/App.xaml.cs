using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LogGuard_v0._1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static App _instance;
        public static new App Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new App();
                }
                return _instance;
            }
        }
        private WindowDirector _windowDirector;

        private App() : base()
        {
            _instance = this;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _windowDirector = new WindowDirector();
            _windowDirector.ShowMainWindow();
        }

        public void ShowPopupCControl(ContentControl cc, UIElement opener, OwnerWindow ownerWindow = OwnerWindow.Default, double width = 500, double height = 400)
        {
            _windowDirector.ShowPopupCustomControlWindow(cc, opener, ownerWindow, width, height);
        }
    }
}
