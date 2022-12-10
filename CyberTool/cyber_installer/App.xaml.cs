using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.implement.utils;
using cyber_installer.implement.app_support_modules;
using cyber_installer.implement.modules;
using cyber_installer.model;
using cyber_installer.view.window;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private Logger _appLogger = new Logger("App", "cyber_installer");
        private WindowDirector _windowDirector;
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
            ModuleManager.Init();
            _instance = this;
            _windowDirector = new WindowDirector();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var arg = Environment.GetCommandLineArgs();
            _appLogger.I("StartupEventArgs: " + String.Join(',', e.Args));
            _appLogger.I("Environment command line args: " + String.Join(',', e.Args));
            
            _windowDirector.Init();
            _windowDirector.ShowMainWindow();
            ModuleManager.OnMainWindowShowed();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ModuleManager.Destroy();
            base.OnExit(e);
        }

        public string ShowDestinationFolderWindow(ToolVO toolVO)
        {
            return _windowDirector.ShowDestinationFolderWindow(toolVO);
        }

        public CyberContactMessage OpenMultiTaskBox(string title
            , MultiAsyncTask tasks
            , bool isCancelable = true
            , Action<object>? multiTaskDoneCallback = null
            , bool isUseMultiTaskReport = true)
        {
            var message = CyberContactMessage.None;
            App.Current.Dispatcher.Invoke(() =>
            {
                message = _windowDirector.OpenMultiTaskBox(title, tasks, isCancelable, multiTaskDoneCallback, isUseMultiTaskReport);
            });
            return message;
        }

        public CyberContactMessage ShowWaringBox(string warning, bool isDialog = true)
        {
            var message = CyberContactMessage.None;
            App.Current.Dispatcher.Invoke(() =>
            {
                message = _windowDirector.ShowWarningBox(warning, isDialog);
            });
            return message;
        }

        public CyberContactMessage ShowYesNoQuestionBox(string question, bool isDialog = true)
        {
            var message = CyberContactMessage.None;
            App.Current.Dispatcher.Invoke(() =>
            {
                message = _windowDirector.ShowYesNoQuestionBox(question, isDialog);
            });
            return message;
        }
    }
}
