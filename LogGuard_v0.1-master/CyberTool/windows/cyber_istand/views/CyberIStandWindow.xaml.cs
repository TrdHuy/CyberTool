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

        #region SingleTaskWindow
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
            SingleTaskDisplayPanel.Visibility = Visibility.Visible;
            MultiTaskDisplayPanel.Visibility = Visibility.Collapsed;

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
            task.OnCompletedChanged -= SingleTaskCompletedChanged;
            task.OnCompletedChanged += SingleTaskCompletedChanged;

            MainTask = task;

            CancelBtn.Click += (s, e) =>
            {
                MainTask.Cancel();
                MesResult = CyberIStandBoxResult.Cancel;
                CancelIconPath.Visibility = Visibility.Visible;
                IconPath.Visibility = Visibility.Collapsed;
                ContinueBtn.Visibility = Visibility.Visible;
                CancelBtn.Visibility = Visibility.Collapsed;
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

        private void SingleTaskCompletedChanged(object sender, bool oldValue, bool newValue)
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
                CancelBtn.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region MultiTaskWindow
        public CyberIStandWindow(string title
            , MultiAsyncTask tasks
            , Window? owner = null)
        {
            InitializeComponent();
            MainTask = tasks;
            SingleTaskDisplayPanel.Visibility = Visibility.Collapsed;
            MultiTaskDisplayPanel.Visibility = Visibility.Visible;

            _context = (CyberIStandWindowViewModel)DataContext;
            _context.Title = title;
            _context.Content = "Waiting to be assigned!";

            if (owner != null)
            {
                Owner = owner;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            if (tasks.TaskCount <= 1)
            {
                MainTaskProgress.Visibility = Visibility.Collapsed;
                MainTaskLabel.Visibility = Visibility.Collapsed;
                SubTaskContentPanel.Margin = new Thickness(20, 0, 20, 0);
            }
            else
            {
                tasks.ProgressChanged -= OnMultiTaskProgressChanged;
                tasks.ProgressChanged += OnMultiTaskProgressChanged;
            }
            tasks.CurrentTaskChanged -= OnCurrentTaskChanged;
            tasks.CurrentTaskChanged += OnCurrentTaskChanged;

            tasks.OnCompletedChanged -= MultiTaskCompletedChanged;
            tasks.OnCompletedChanged += MultiTaskCompletedChanged;

            CancelBtn.Click += (s, e) =>
            {
                MainTask.Cancel();
                MesResult = CyberIStandBoxResult.Cancel;
                CancelIconPath.Visibility = Visibility.Visible;
                IconPath.Visibility = Visibility.Collapsed;
                ContinueBtn.Visibility = Visibility.Visible;
                CancelBtn.Visibility = Visibility.Collapsed;
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

        private void MultiTaskCompletedChanged(object sender, bool oldValue, bool newValue)
        {
            if (newValue)
            {
                MesResult = MainTask.IsCanceled ? CyberIStandBoxResult.Cancel : CyberIStandBoxResult.Done;
                if (MesResult == CyberIStandBoxResult.Done)
                {
                    _context.Title = "Done";
                }
                else if (MesResult == CyberIStandBoxResult.Cancel)
                {
                    _context.Title = "Aborted";
                }
                ContinueBtn.Visibility = Visibility.Visible;
                CancelBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void OnMultiTaskProgressChanged(object sender, double currentProgress)
        {
            _context.TotalPercent = currentProgress;
        }

        private void OnCurrentTaskChanged(object sender, BaseAsyncTask? oldTask, BaseAsyncTask? newTask)
        {
            if (oldTask != null)
            {
                oldTask.ProgressChanged -= OnCurrentTaskProgressChanged;
            }
            if (newTask != null)
            {
                newTask.ProgressChanged -= OnCurrentTaskProgressChanged;
                newTask.ProgressChanged += OnCurrentTaskProgressChanged;
                _context.Content = newTask.Name ?? "An anonymous task is being executed";
            }
        }

        private void OnCurrentTaskProgressChanged(object sender, double currentProgress)
        {
            _context.CurrentTaskPercent = currentProgress;
        }

        #endregion


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
