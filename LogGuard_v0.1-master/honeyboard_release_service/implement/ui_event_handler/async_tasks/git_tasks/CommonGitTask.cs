using cyber_base.async_task;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks
{
    internal class CommonGitTask : BaseRTParamAsyncTask
    {
        private string _folderPath = "";
        private string _outputCache = "";
        private string _cmd = "";

        public CommonGitTask(string folderPath
           , string gitCmd
           , Action<AsyncTaskResult>? callback = null
           , string name = "Common git task"
           , ulong estimatedTime = 8000) : base(folderPath, name, callback)
        {
            _folderPath = folderPath;
            _cmd = gitCmd;
            _estimatedTime = estimatedTime;
            _reportDelay = 100;
        }

        protected override void DoCallback(object param, AsyncTaskResult result)
        {
            result.Result = _outputCache;
        }

        protected override async Task DoAsyncMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            try
            {
                string proPath = _folderPath ?? "";
                string cmd = _cmd;

                var pSI = new ProcessStartInfo("cmd", "/c" + cmd);

                pSI.WorkingDirectory = proPath;
                pSI.RedirectStandardInput = true;
                pSI.RedirectStandardOutput = true;
                pSI.RedirectStandardError = true;
                pSI.CreateNoWindow = true;
                pSI.UseShellExecute = false;
                pSI.StandardOutputEncoding = Encoding.UTF8;
                using (Process? process = Process.Start(pSI))
                {
                    if (process != null)
                    {
                        if (process != null)
                        {
                            process.OutputDataReceived -= OnDataReceived;
                            process.ErrorDataReceived -= OnDataReceived;
                            process.OutputDataReceived += OnDataReceived;
                            process.ErrorDataReceived += OnDataReceived;
                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();
                            await process.WaitForExitAsync(token.Token);
                        }
                    }
                }

                _result.MesResult = MessageAsyncTaskResult.Done;
            }
            catch (Exception e)
            {
                _result.MesResult = MessageAsyncTaskResult.Aborted;
                throw new InvalidOperationException(e.Message);
            }
            finally
            {
            }
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _outputCache += e.Data + "\n";
            }
        }

        protected override bool IsTaskPossible(object param)
        {
            return !string.IsNullOrEmpty(_folderPath)
                  && Directory.Exists(_folderPath)
                  && !string.IsNullOrEmpty(_cmd);
        }
    }
}
