using cyber_installer.implement.modules;
using cyber_installer.view.window;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace cyber_installer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static App? _instance;
        private CyberInstallerWindow? _mainWindow;

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
        
        private App() : base()
        {
            _instance = this;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ModuleManager.Init();
            base.OnStartup(e);
            if(_mainWindow == null)
            {
                _mainWindow = new CyberInstallerWindow();
            }
            _mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ModuleManager.Destroy();
            base.OnExit(e);
        }

        public string ShowDestinationFolderWindow()
        {
            var destinationFolderWindow = new DestinationFolderSelectionWindow();
            return destinationFolderWindow.Show();
        }
    }
}
