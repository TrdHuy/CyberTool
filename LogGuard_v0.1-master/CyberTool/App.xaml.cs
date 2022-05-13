using cyber_base.app;
using cyber_base.utils.async_task;
using cyber_extension.dll_base.extension;
using cyber_tool.plugins;
using cyber_tool.utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cyber_tool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ICyberApplication
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

        public Application CyberApp
        {
            get
            {
                return this;
            }
        }


        private App() : base()
        {
            _instance = this;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            CyberToolModuleManager.Init();
            
            //CyberPluginsManager.Current.LoadExternalPlugin();

            base.OnStartup(e);
        }

        public void ShowPopupCControl(ContentControl cc
            , UIElement opener
            , OwnerWindow ownerWindow = OwnerWindow.Default
            , double width = 500
            , double height = 400
            , object dataContext = null
            , Action<object> windowShowedCallback = null
            , string title = "Floating window")
        {
            _windowDirector.ShowPopupCustomControlWindow(cc, opener, ownerWindow, width, height, dataContext, windowShowedCallback, title);
        }

        public Windows.MessageWindow.LogGuardMesBoxResult ShowEscapeCaptureLogWarningBox()
        {
            return _windowDirector.ShowEscapeCaptureLogWarningBox();
        }

        public Windows.MessageWindow.LogGuardMesBoxResult ShowErrorBox(string error)
        {
            return _windowDirector.ShowErrorBox(error);
        }

        public Windows.MessageWindow.LogGuardMesBoxResult ShowWaringBox(string warning, bool isDialog = true)
        {
            return _windowDirector.ShowWarningBox(warning, isDialog);
        }

        public string OpenSaveFileDialogWindow()
        {
            return _windowDirector.OpenSaveLogFileDialogWindow();
        }

        public string OpenFileChooserDialogWindow(string title = "Choose a log file", string filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log")
        {
            return _windowDirector.OpenFileChooserDialogWindow(title, filter);
        }

        public void OpenWaitingTaskBox(string content
            , string title
            , Func<object, CancellationToken, Task<AsyncTaskResult>> asyncTask
            , Func<bool> canExecute = null
            , Action<object, AsyncTaskResult> callback = null
            , long delayTime = 0)
        {
            return _windowDirector.OpenWaitingTaskBox(content, title, asyncTask, canExecute, callback, delayTime);
        }


        private void OnMainScreenWindowClosing(object sender, CancelEventArgs e)
        {
            OnMainWindowClosing?.Invoke(sender, e);
        }

    }
}
