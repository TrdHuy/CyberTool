﻿using LogGuard_v0._1._Config;
using LogGuard_v0._1.Base.AsyncTask;
using LogGuard_v0._1.Implement.Device;
using LogGuard_v0._1.Implement.ViewModels;
using LogGuard_v0._1.Windows.WaitingWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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
        public event OnMainWindowClosingHandler OnMainWindowClosing;

        private WindowDirector _windowDirector;

        private App() : base()
        {
            _instance = this;

            // Modules need to be inited before lauch app
            RUNE.Init();
            ViewModelHelper.Current.Init();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _windowDirector = new WindowDirector();
            _windowDirector.ShowMainWindow();
            _windowDirector.MainScreenWindow.Closing += OnMainScreenWindowClosing;
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

        public LogGuardWaitingBoxResult OpenWaitingTaskBox(string content
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

    public delegate void OnMainWindowClosingHandler(object sender, CancelEventArgs eventArg);
}
