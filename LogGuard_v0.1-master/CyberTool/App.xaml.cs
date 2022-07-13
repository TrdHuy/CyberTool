using cyber_base.app;
using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_extension.dll_base.extension;
using cyber_tool.definitions;
using cyber_tool.plugins;
using cyber_tool.utils;
using cyber_tool.windows;
using cyber_tool.windows.cyber_istand.views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private static App? _instance;
        private WindowDirector _WindowDirector;

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
                return Current;
            }
        }


        private App() : base()
        {
            _instance = this;
            _WindowDirector = new WindowDirector();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            CyberToolModuleManager.Init();

            //CyberPluginsManager.Current.LoadExternalPlugin();

            base.OnStartup(e);

            _WindowDirector.ShowCyberIFace();

            CyberToolModuleManager.OnIFaceShowed();
        }


        public CyberContactMessage OpenWaitingTaskBox(string content
            , string title
            , Func<object, AsyncTaskResult, CancellationTokenSource, Task<AsyncTaskResult>> asyncTask
            , Func<object, bool>? canExecute = null
            , Func<object, AsyncTaskResult, Task<AsyncTaskResult>>? callback = null
            , ulong delayTime = 0
            , ulong estimatedTime = 0
            , string taskName = "")
        {
            return _WindowDirector.OpenWaitingTaskBox(content, title, asyncTask, canExecute, callback, delayTime);

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
                message = _WindowDirector.OpenMultiTaskBox(title, tasks, isCancelable, multiTaskDoneCallback, isUseMultiTaskReport);
            });
            return message;
        }

        public CyberContactMessage ShowWaringBox(string warning, bool isDialog = true)
        {
            var message = CyberContactMessage.None;
            App.Current.Dispatcher.Invoke(() =>
            {
                message = _WindowDirector.ShowWarningBox(warning, isDialog);
            });
            return message;
        }

        public CyberContactMessage ShowYesNoQuestionBox(string question, bool isDialog = true)
        {
            var message = CyberContactMessage.None;
            App.Current.Dispatcher.Invoke(() =>
            {
                message = _WindowDirector.ShowYesNoQuestionBox(question, isDialog);
            });
            return message;
        }

        public void ShowPopupCControl(ContentControl cc
           , UIElement opener
           , CyberOwner ownerType = CyberOwner.Default
           , double width = 500
           , double height = 400
           , object? dataContext = null
           , Action<object>? windowShowedCallback = null
           , string title = "Floating window")
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                switch (ownerType)
                {
                    case CyberOwner.Default:
                        _WindowDirector.ShowPopupCustomControlWindow(cc
                            , opener
                            , CyberOwnerWindow.Default
                            , width
                            , height
                            , dataContext
                            , windowShowedCallback
                            , title);
                        break;
                    case CyberOwner.ServiceManager:
                        _WindowDirector.ShowPopupCustomControlWindow(cc
                            , opener
                            , CyberOwnerWindow.CyberIFace
                            , width
                            , height
                            , dataContext
                            , windowShowedCallback
                            , title);
                        break;
                }
            });
        }

        public string OpenSaveFileDialogWindow()
        {
            return _WindowDirector.OpenSaveLogFileDialogWindow();
        }

        public string OpenFileChooserDialogWindow(string title = "Choose a log file", string filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log")
        {
            return _WindowDirector.OpenFileChooserDialogWindow(title, filter);
        }

        public string OpenFolderChooserDialogWindow()
        {
            return _WindowDirector.OpenFolderChooserDialogWindow();
        }

    }
}
