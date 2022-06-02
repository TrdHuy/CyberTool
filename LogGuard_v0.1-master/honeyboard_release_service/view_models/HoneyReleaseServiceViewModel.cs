using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.implement.command;
using cyber_base.view_model;
using honeyboard_release_service.view_models.calendar_notebook;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models
{
    internal class HoneyReleaseServiceViewModel : BaseViewModel
    {
        private string _displayData = "";
        private string _projectPath = "";
        private string _cmd = "";
        private CalendarNoteBookViewModel _calendarNoteBookContext;

        [Bindable(true)]
        public CalendarNoteBookViewModel CalendarNoteBookContext
        {
            get
            {
                return _calendarNoteBookContext;
            }
            set
            {
                _calendarNoteBookContext = value;
                InvalidateOwn();
            }

        }

        [Bindable(true)]
        public string Cmd
        {
            get
            {
                return _cmd;
            }
            set
            {
                _cmd = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string ProjectPath
        {
            get
            {
                return _projectPath;
            }
            set
            {
                _projectPath = value;
                InvalidateOwn();
            }
        }


        [Bindable(true)]
        public string DisplayData
        {
            get
            {
                return _displayData;
            }
            set
            {
                _displayData = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public BaseDotNetCommandImpl ExecuteCmd { get; set; }

        [Bindable(true)]
        public BaseDotNetCommandImpl ClearCmd { get; set; }

        [Bindable(true)]
        public BaseDotNetCommandImpl StopCmd { get; set; }

        public HoneyReleaseServiceViewModel()
        {
            ExecuteCmd = new BaseDotNetCommandImpl(OnExecuteCmd);
            ClearCmd = new BaseDotNetCommandImpl(OnClearCmd);
            StopCmd = new BaseDotNetCommandImpl(OnStopCmd);
            _calendarNoteBookContext = new CalendarNoteBookViewModel(this);
        }

        private void OnStopCmd(object obj)
        {
            if (_TaskCache != null
                && !_TaskCache.IsCompleted
                && !_TaskCache.IsCanceled)
            {
                _TaskCache?.Cancel();
            }
        }
        private void OnClearCmd(object obj)
        {
            DisplayData = "";
        }

        CancellationTokenSource? _TokenCache;
        ParamAsyncTask? _TaskCache;

        private void OnExecuteCmd(object obj)
        {
            Debug.WriteLine("Current task id:" + Task.CurrentId);

            if (_TaskCache != null
                && !_TaskCache.IsCompleted
                && !_TaskCache.IsCanceled)
            {
                _TaskCache.Cancel();
                Debug.WriteLine("Task canceled");

            }

            _TokenCache = new CancellationTokenSource();

            _TaskCache = new ParamAsyncTask(OnDoCommand
                    , _TokenCache
                    , this
                    , null
                    , OnCallback);
            _TaskCache.Execute();
        }


        private async Task<AsyncTaskResult> OnCallback(object data, AsyncTaskResult obj)
        {
            if (obj.MesResult == MessageAsyncTaskResult.Done)
            {
                Debug.WriteLine("Async Task completed:" + Task.CurrentId);
            }
            return obj;
        }

        private async Task<AsyncTaskResult> OnDoCommand(object data
            , AsyncTaskResult result
            , CancellationTokenSource token)
        {
            var res = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);

            try
            {
                string proPath = String.IsNullOrEmpty(ProjectPath) ? @"C:\SCS\AndroidProject\HoneyBoard" : ProjectPath;
                string cmd = String.IsNullOrEmpty(Cmd) ? "git branch -a" : Cmd;

                var pSI = new ProcessStartInfo("cmd", "/c" + cmd);

                pSI.WorkingDirectory = proPath;
                pSI.RedirectStandardInput = true;
                pSI.RedirectStandardOutput = true;
                pSI.RedirectStandardError = true;
                pSI.CreateNoWindow = false;
                pSI.UseShellExecute = false;
                pSI.StandardOutputEncoding = System.Text.Encoding.UTF8;
                using (Process? process = Process.Start(pSI))
                {
                    if (process != null)
                    {
                        process.OutputDataReceived += OnDataReceived;
                        process.ErrorDataReceived += OnDataReceived;
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                    }
                }

                res.MesResult = MessageAsyncTaskResult.Done;
            }
            catch (Exception ex)
            {
                res.MesResult = MessageAsyncTaskResult.Aborted;
            }
            finally
            {
            }
            Debug.WriteLine("Task completed:" + Task.CurrentId);
            return res;
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            DisplayData += (e.Data ?? "") + "\n";
        }
    }
}
