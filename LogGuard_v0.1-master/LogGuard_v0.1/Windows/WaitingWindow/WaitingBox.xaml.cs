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
            , Func<Task<AsyncTaskResult>> asyncTask
            , Func<bool> canExecute = null
            , Action<AsyncTaskResult> callback = null
            , long delayTime = 0)
        {
            InitializeComponent();
            MainContent.Content = content;
            Title = title;
            ContinueBtn.Visibility = Visibility.Collapsed;
            var task = new AsyncTask(asyncTask
                   , canExecute
                   , callback
                   , delayTime);
            task.OnCompletedChanged -= Task_OnCompletedChanged;
            task.OnCompletedChanged += Task_OnCompletedChanged;

            MainTask = task;
            var cts = new CancellationTokenSource();
            cancelToken = cts.Token;

            cancelBtn.Click += (s, e) =>
            {
                cts.Cancel();
                CancelIconPath.Visibility = Visibility.Visible;
                IconPath.Visibility = Visibility.Collapsed;
                Title = "Canceled!";
                MainContent.Content = "Your process is canceled!~";
            };
            ContinueBtn.Click += (s, e) =>
            {
                this.Close();
            };

        }

        private void Task_OnCompletedChanged(object sender, bool oldValue, bool newValue)
        {
            if (newValue)
            {
                MesResult = MainTask.IsCanceled ? LogGuardWaitingBoxResult.cancel : LogGuardWaitingBoxResult.Done;
                ContinueBtn.Visibility = Visibility.Visible;
                cancelBtn.Visibility = Visibility.Collapsed;
            }
        }

        public new LogGuardWaitingBoxResult Show()
        {
            AsyncTask.AsyncExecute(MainTask, cancelToken);
            base.ShowDialog();
            return MesResult;
        }
    }
    public enum LogGuardWaitingBoxResult
    {
        None = 0,
        Done = 1,
        cancel = 2,
    }
}
