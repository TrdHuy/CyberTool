using cyber_base.app;
using cyber_base.definition;
using cyber_base.utils.async_task;
using cyber_extension.dll_base.extension;
using cyber_tool.definitions;
using cyber_tool.plugins;
using cyber_tool.utils;
using cyber_tool.windows;
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
        private static App _instance;
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

            _WindowDirector = new WindowDirector();
            //CyberPluginsManager.Current.LoadExternalPlugin();

            base.OnStartup(e);

            _WindowDirector.ShowCyberIFace();

            CyberToolModuleManager.OnIFaceShowed();
        }


        public CyberContactMessage OpenWaitingTaskBox(string content
            , string title
            , Func<object, CancellationToken, Task<AsyncTaskResult>> asyncTask
            , Func<bool> canExecute = null
            , Action<object, AsyncTaskResult> callback = null
            , long delayTime = 0)
        {
            return _WindowDirector.OpenWaitingTaskBox(content, title, asyncTask, canExecute, callback, delayTime);

        }

    }
}
