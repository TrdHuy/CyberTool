using cyber_base.implement.command;
using cyber_base.utils.async_task;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models
{
    internal class HoneyReleaseServiceViewModel : BaseViewModel
    {
        private string _displayData = "";
        private string _projectPath = "";
        private string _cmd = "";

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
        }

        private void OnStopCmd(object obj)
        {
            if (_TaskCache != null && !_TaskCache.IsCompleted)
            {
                AsyncTask.CancelAsyncExecute(_TaskCache);
            }
        }
        private void OnClearCmd(object obj)
        {
            DisplayData = "";
        }

        CancellationTokenSource _TokenCache;
        AsyncTask _TaskCache;

        private void OnExecuteCmd(object obj)
        {
            Debug.WriteLine("Current task id:" + Task.CurrentId);

            if (_TaskCache != null && !_TaskCache.IsCompleted)
            {
                AsyncTask.CancelAsyncExecute(_TaskCache);
                Debug.WriteLine("Task canceled");

            }

            _TokenCache = new CancellationTokenSource();

            _TaskCache = new AsyncTask(OnDoCommand
                    , null
                    , OnCallback
                    , 0
                    , _TokenCache);
            AsyncTask.ParamAsyncExecute(_TaskCache
               , param: null
               , isAsyncCallback: true);
        }


        private void OnCallback(object data, AsyncTaskResult obj)
        {
            if (obj.MesResult == MessageAsyncTaskResult.Done)
            {
                Debug.WriteLine("Async Task completed:" + Task.CurrentId);
            }
        }

        private async Task<AsyncTaskResult> OnDoCommand(object? data, CancellationToken token)
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
