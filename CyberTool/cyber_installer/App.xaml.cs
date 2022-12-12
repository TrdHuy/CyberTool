using cyber_base.async_task;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

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
        private TaskHandleManager _taskHandleManager;
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
            _windowDirector = new WindowDirector();
            _taskHandleManager = new TaskHandleManager();
            ModuleManager.Init();
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

        public void RegisterManageableTask(string taskTypeKey, string taskName, int maxCore, int initCore)
        {
            _taskHandleManager.GenerateNewTaskSemaphore(taskTypeKey, taskName, maxCore, initCore);
        }

        public bool IsTaskAvailable(string taskTypeKey)
        {
            return _taskHandleManager.IsTaskAvailable(taskTypeKey);
        }

        public string ShowDestinationFolderWindow(ToolVO toolVO)
        {
            return _windowDirector.ShowDestinationFolderWindow(toolVO);
        }

        public async Task<CyberContactMessage> ExecuteManageableMultipleTasks(string taskTypeKey
            , MultiAsyncTask tasks
            , bool isBybassIfSemaphoreNotAvaild = false
            , int semaphoreTimeOut = 2000
            , bool isCancelable = true
            , Action<object>? multiTaskDoneCallback = null
            , bool isUseMultiTaskReport = true)
        {
            var message = CyberContactMessage.None;

            await _taskHandleManager.ExecuteTask(
                taskTypeKey: taskTypeKey
                , mainFunc: (taskInfo) =>
                {
                    message = _windowDirector.OpenMultiTaskBox(taskInfo.Name, tasks, isCancelable, multiTaskDoneCallback, isUseMultiTaskReport);
                }
                , bypassIfSemaphoreNotAvaild: isBybassIfSemaphoreNotAvaild
                , semaphoreTimeOut: semaphoreTimeOut);

            return message;
        }

        public async Task<CyberContactMessage> ExecuteManageableSingleTask(string taskTypeKey
            , Func<object, AsyncTaskResult, CancellationTokenSource, Task<AsyncTaskResult>> asyncTask
            , Func<object, bool>? canExecute = null
            , Func<object, AsyncTaskResult, Task<AsyncTaskResult>>? callback = null
            , string boxDisplayContent = ""
            , bool isBybassIfSemaphoreNotAvaild = false
            , int semaphoreTimeOut = 2000
            , ulong delayTime = 0
            , ulong estimatedTime = 0
            , string taskName = "")
        {
            var message = CyberContactMessage.None;

            await _taskHandleManager.ExecuteTask(
                taskTypeKey: taskTypeKey
                , mainFunc: (taskInfo) =>
                {
                    var content = string.IsNullOrEmpty(boxDisplayContent) ? taskInfo.Name : boxDisplayContent;
                    message = _windowDirector.OpenWaitingTaskBox(content
                        , taskInfo.Name
                        , asyncTask
                        , canExecute
                        , callback
                        , delayTime);
                }
                , bypassIfSemaphoreNotAvaild: isBybassIfSemaphoreNotAvaild
                , semaphoreTimeOut: semaphoreTimeOut);

            return message;
        }

        public CyberContactMessage ShowErrorBox(string error)
        {
            var message = CyberContactMessage.None;
            App.Current.Dispatcher.Invoke(() =>
            {
                message = _windowDirector.ShowErrorBox(error);
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
