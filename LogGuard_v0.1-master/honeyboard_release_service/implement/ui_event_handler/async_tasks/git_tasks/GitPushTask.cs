using cyber_base.async_task;
using honeyboard_release_service.implement.log_manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks
{
    internal enum PushResult
    {
        None = 0,
        Success = 1,
        Error = 2,
        UpToDate = 3,
    }

    internal class GitPushTask : BaseRTParamAsyncTask
    {
        private static readonly Regex _gitErrorMessageRegex =
           new Regex(@"error:\s+(?<message>.*)");
        private static readonly Regex _gerritLinkMessageRegex =
           new Regex(@"remote:\s+(?<link>https*://\S*)\s+(?<other>.*)");
        private static readonly string _upToDateString = "Everything up-to-date";

        private string _folderPath = "";
        private string _branchPathForPushing = "";
        private string _outputCache = "";
        private PushResult _pushStatus = PushResult.None;
        private bool _isPushToGerrit = false;
        private string _message = "";
        private string _gerritLinkCache = "";

        public GitPushTask(object param
           , Action<AsyncTaskResult>? completedCallback = null
           , string name = "Pushing")
           : base(param, name, completedCallback)
        {
            switch (param)
            {
                case string[] data:
                    if (data.Length == 2)
                    {
                        _folderPath = data[0];
                        _branchPathForPushing = data[1];
                    }
                    break;
                default:
                    throw new InvalidDataException("Param must be an array of string with 2 elements");
            }

            _isPushToGerrit = _branchPathForPushing.Contains("HEAD:refs/for/");
            _estimatedTime = 10000;
            _reportDelay = 100;
            _delayTime = 3000;
        }

        protected override void DoCallback(object param, AsyncTaskResult result)
        {
            if (_pushStatus == PushResult.None)
            {
                _pushStatus = PushResult.Success;
            }
            dynamic res = new ExpandoObject();
            res.PushResult = _pushStatus;
            res.OutputCache = _outputCache;
            res.GerritLink = _gerritLinkCache;
            res.ErrorMessage = _message;
            result.Result = res;
        }

        protected override async Task DoAsyncMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            try
            {
                string proPath = _folderPath ?? "";
                string cmd = "git push --no-thin origin " + _branchPathForPushing;
                var pSI = new ProcessStartInfo("cmd", "/c" + cmd);

                pSI.WorkingDirectory = proPath;
                pSI.RedirectStandardInput = true;
                pSI.RedirectStandardOutput = true;
                pSI.RedirectStandardError = true;
                pSI.CreateNoWindow = true;
                pSI.UseShellExecute = false;
                pSI.StandardOutputEncoding = Encoding.UTF8;

                LogManager.Current.AppendLogLine(cmd, true);

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
                LogManager.Current.AppendLogLine(e.Data);


                if (_pushStatus == PushResult.None)
                {
                    var matchError = _gitErrorMessageRegex.Match(e.Data);
                    if (matchError.Length > 0)
                    {
                        _message = matchError.Groups["message"].Value ?? "";
                        _pushStatus = PushResult.Error;
                    }
                    else if (e.Data.Contains(_upToDateString))
                    {
                        _message = _upToDateString;
                        _pushStatus = PushResult.UpToDate;
                    }
                }

                if (_isPushToGerrit && string.IsNullOrEmpty(_gerritLinkCache))
                {
                    var matchRemote = _gerritLinkMessageRegex.Match(e.Data);
                    if (matchRemote.Length > 0)
                    {
                        _gerritLinkCache = matchRemote.Groups["link"].Value ?? "";
                        _pushStatus = PushResult.Success;
                    }
                }
            }
        }

        protected override bool IsTaskPossible(object param)
        {
            return !string.IsNullOrEmpty(_folderPath)
                  && Directory.Exists(_folderPath)
                  && !string.IsNullOrEmpty(_branchPathForPushing);
        }
    }
}
