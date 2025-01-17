﻿using cyber_base.app;
using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_core.definitions;
using cyber_core.utils;
using cyber_core.windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cyber_core
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ICyberApplication
    {
        private static App? _instance;
        private WindowDirector _WindowDirector;
        private List<ICyberGlobalModule> _globalModules = new List<ICyberGlobalModule>();

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
            var isRunable = IsCurrentProcessNameChanged();
            if (isRunable)
            {
                CyberToolModuleManager.Init();

                base.OnStartup(e);

                _WindowDirector.ShowCyberIFace();

                CyberToolModuleManager.OnIFaceShowed();
            }
            else
            {
                _WindowDirector.ShowWarningBox("Can not run application because executable file name has been changed!\n" +
                    "Please maintain its name is  " + GetCurrentAssemblyName(), false);
            }

        }

        protected override void OnExit(ExitEventArgs e)
        {
            CyberToolModuleManager.Destroy();
            foreach (var module in _globalModules)
            {
                module.OnGlobalModuleDestroy();
            }
            _globalModules.Clear();
            base.OnExit(e);
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

        public void ShowUserControlWindow(UserControl uc
            , CyberOwner ownerType = CyberOwner.Default
            , double width = 500
            , double height = 400
            , Action<object>? windowShowedCallback = null
            , string title = "Floating window")
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                switch (ownerType)
                {
                    case CyberOwner.Default:
                        _WindowDirector.ShowPopupUserControlWindow(uc
                            , CyberOwnerWindow.Default
                            , width
                            , height
                            , windowShowedCallback
                            , title);
                        break;
                    case CyberOwner.ServiceManager:
                        _WindowDirector.ShowPopupUserControlWindow(uc
                            , CyberOwnerWindow.CyberIFace
                            , width
                            , height
                            , windowShowedCallback
                            , title);
                        break;
                }
            });
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

        public string OpenEditTextDialogWindow(string oldText, bool isMultiLine)
        {
            return _WindowDirector.OpenEditTextDialogWindow(oldText, isMultiLine);
        }

        public void RegisterGlobalModule(ICyberGlobalModule globalModule)
        {
            if (!_globalModules.Contains(globalModule))
            {
                _globalModules.Add(globalModule);
                globalModule.OnGlobalModuleStart();
            }
        }

        private bool IsCurrentProcessNameChanged()
        {
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            var pName = Process.GetCurrentProcess().ProcessName;

            return name == pName;
        }

        private string GetCurrentAssemblyName()
        {
            return Assembly.GetExecutingAssembly().GetName().Name ?? "CyberTool";
        }
    }
}
