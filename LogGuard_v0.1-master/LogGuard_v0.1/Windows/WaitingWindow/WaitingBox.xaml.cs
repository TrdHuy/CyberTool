using LogGuard_v0._1.AppResources.Controls.LogGWindows;
using LogGuard_v0._1.Base.AsyncTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LogGuard_v0._1.Windows.WaitingWindow
{
    /// <summary>
    /// Interaction logic for WaitingBox.xaml
    /// </summary>
    public partial class WaitingBox : LogGuardWindow
    {
        public LogGuardWaitingBoxResult MesResult { get; private set; } = LogGuardWaitingBoxResult.None;
        private AsyncTask MainTask { get; set; }
        private CancellationToken cancelToken { get; set; }
        public WaitingBox(string content
            , string title
            , Func<object, CancellationToken, Task<AsyncTaskResult>> asyncTask
            , Func<bool> canExecute = null
            , Action<object, AsyncTaskResult> callback = null
            , long delayTime = 0
            , Window owner = null)
        {
            InitializeComponent();
            if (owner != null)
            {
                Owner = owner;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            MainContent.Content = content;
            Title = title;
            ContinueBtn.Visibility = Visibility.Collapsed;

            var cts = new CancellationTokenSource();
            cancelToken = cts.Token;
            var task = new AsyncTask(asyncTask
                   , canExecute
                   , callback
                   , delayTime
                   , cts);
            task.OnCompletedChanged -= Task_OnCompletedChanged;
            task.OnCompletedChanged += Task_OnCompletedChanged;

            MainTask = task;
           

            cancelBtn.Click += (s, e) =>
            {
                cts.Cancel();
                MesResult = LogGuardWaitingBoxResult.cancel;
                CancelIconPath.Visibility = Visibility.Visible;
                IconPath.Visibility = Visibility.Collapsed;
                ContinueBtn.Visibility = Visibility.Visible;
                cancelBtn.Visibility = Visibility.Collapsed;
                Title = "Canceled!";
                MainContent.Content = "Your process is canceled!~";
            };

            ContinueBtn.Click += (s, e) =>
            {
                this.Close();
            };

            Closing += (s, e) =>
            {
                if (!MainTask.IsCompleted)
                {
                    cts.Cancel();
                    MesResult = LogGuardWaitingBoxResult.cancel;
                }
            };
        }

        private void Task_OnCompletedChanged(object sender, bool oldValue, bool newValue)
        {
            if (newValue)
            {
                MesResult = MainTask.IsCanceled ? LogGuardWaitingBoxResult.cancel : LogGuardWaitingBoxResult.Done;
                if (MesResult == LogGuardWaitingBoxResult.Done)
                {
                    IconPath.Visibility = Visibility.Collapsed;
                    CancelIconPath.Visibility = Visibility.Collapsed;
                    SuccessIconPath.Visibility = Visibility.Visible;
                }
                ContinueBtn.Visibility = Visibility.Visible;
                cancelBtn.Visibility = Visibility.Collapsed;
            }
        }

        public new LogGuardWaitingBoxResult Show()
        {
            AsyncTask.ParamAsyncExecute(MainTask, this);
            base.ShowDialog();
            return MesResult;
        }

        public void UpdateMessageAndTitle(string message, string title)
        {
            Title = title;
            MainContent.Content = message;
        }
    }
    public enum LogGuardWaitingBoxResult
    {
        None = 0,
        Done = 1,
        cancel = 2,
    }
}
