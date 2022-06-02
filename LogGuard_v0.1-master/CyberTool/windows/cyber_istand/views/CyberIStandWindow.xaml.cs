using cyber_tool.app_resources.controls.cyber_window;
using System;
using System.Collections.Generic;
using System.Linq;
using cyber_tool.definitions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using cyber_base.view.window;
using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_tool.windows.cyber_istand.view_models;

namespace cyber_tool.windows.cyber_istand.views
{
    /// <summary>
    /// Interaction logic for CyberIStandWindow.xaml
    /// </summary>
    public partial class CyberIStandWindow : CyberWindow, IStandBox
    {
        public CyberIStandBoxResult MesResult { get; private set; } = CyberIStandBoxResult.None;
        private BaseAsyncTask MainTask { get; set; }
        private CyberIStandWindowViewModel _context;

        public CyberIStandWindow(string content
            , string title
            , Func<object, AsyncTaskResult, CancellationTokenSource, Task<AsyncTaskResult>> asyncTask
            , Func<object, bool>? canExecute = null
            , Func<object, AsyncTaskResult, Task<AsyncTaskResult>>? callback = null
            , ulong delayTime = 0
            , string taskName = ""
            , ulong estimatedTime = 0
            , Window? owner = null)
        {
            InitializeComponent();
            _context = (CyberIStandWindowViewModel)DataContext;
            if (owner != null)
            {
                Owner = owner;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            _context.Content = content;
            _context.Title = title;
            ContinueBtn.Visibility = Visibility.Collapsed;

            var cts = new CancellationTokenSource();
            var task = new ParamAsyncTask(asyncTask
                , cts
                , this
                , canExecute
                , callback
                , taskName
                , estimatedTime
                , delayTime
                , 100);
            task.OnCompletedChanged -= Task_OnCompletedChanged;
            task.OnCompletedChanged += Task_OnCompletedChanged;

            MainTask = task;


            cancelBtn.Click += (s, e) =>
            {
                MainTask.Cancel();
                MesResult = CyberIStandBoxResult.Cancel;
                CancelIconPath.Visibility = Visibility.Visible;
                IconPath.Visibility = Visibility.Collapsed;
                ContinueBtn.Visibility = Visibility.Visible;
                cancelBtn.Visibility = Visibility.Collapsed;
                _context.Title = "Canceled!";
                _context.Content = "Your process is canceled!~";
            };

            ContinueBtn.Click += (s, e) =>
            {
                this.Close();
            };

            Closing += (s, e) =>
            {
                if (!MainTask.IsCompleted && !MainTask.IsCanceled)
                {
                    MainTask.Cancel();
                    MesResult = CyberIStandBoxResult.Cancel;
                }
            };
        }

        private void Task_OnCompletedChanged(object sender, bool oldValue, bool newValue)
        {
            if (newValue)
            {
                MesResult = MainTask.IsCanceled ? CyberIStandBoxResult.Cancel : CyberIStandBoxResult.Done;
                if (MesResult == CyberIStandBoxResult.Done)
                {
                    IconPath.Visibility = Visibility.Collapsed;
                    CancelIconPath.Visibility = Visibility.Collapsed;
                    SuccessIconPath.Visibility = Visibility.Visible;
                }
                ContinueBtn.Visibility = Visibility.Visible;
                cancelBtn.Visibility = Visibility.Collapsed;
            }
        }

        public new CyberIStandBoxResult Show()
        {
            MainTask.Execute();
            base.ShowDialog();
            return MesResult;
        }

        public void UpdateMessageAndTitle(string message, string title)
        {
            _context.Title = title;
            _context.Content = message;
        }
    }
}
