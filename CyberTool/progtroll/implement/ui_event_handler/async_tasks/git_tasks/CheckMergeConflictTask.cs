using cyber_base.async_task;
using progtroll.implement.log_manager;
using progtroll.models.VOs;
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

namespace progtroll.implement.ui_event_handler.async_tasks.git_tasks
{
    internal class CheckMergeConflictTask : BaseRTParamAsyncTask
    {
        private enum CheckingState
        {
            None = 0,
            ConflictID = 1,
            ConflictFile = 2,
        }

        private static readonly Regex _mergeCLSubjectRegex
            = new Regex(@"huy.td1_hashid:(?<hashid>[a-z0-9]{5,20}) " +
                @"huy.td1_subject:(?<title>.+) " +
                @"huy.td1_datetime:(?<datetime>\d{2}:\d{2}:\d{2} \d{4}-\d{2}-\d{2}) " +
                @"huy.td1_email:(?<email>\S+\@samsung.com)");
        private string _folderPath = "";
        private List<CommitVO> _mergeConflictCommitCache;
        private List<string> _mergeConflictFileCache;
        private CheckingState _checkingState = CheckingState.None;

        public CheckMergeConflictTask(object param
           , Action<AsyncTaskResult>? completedCallback = null
           , string name = "Checking merge conflict!"
           , Func<object, bool>? canExecute = null
            )
           : base(param, name, completedCallback
                 , canExecute: canExecute)
        {
            switch (param)
            {
                case string data:
                    _folderPath = data;
                    break;
                default:
                    throw new InvalidDataException("Param must be a string of project path");
            }
            _estimatedTime = 3000;
            _reportDelay = 100;
            _delayTime = 3000;
            _mergeConflictCommitCache = new List<CommitVO>();
            _mergeConflictFileCache = new List<string>();
        }

        protected override void DoCallback(object param, AsyncTaskResult result)
        {
            dynamic res = new ExpandoObject();
            res.MergeConflictCommits = _mergeConflictCommitCache;
            res.MergeConflictFiles = _mergeConflictFileCache;
            result.Result = res;
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            try
            {
                _checkingState = CheckingState.ConflictID;
                string getConflictIdCmd = "git log "
                     + "--pretty=format:\"huy.td1_hashid:%h "
                        + "huy.td1_subject:%s "
                        + "huy.td1_datetime:%ad "
                        + "huy.td1_email:%ae\" "
                    + "--date=format:\"%H:%M:%S %Y-%m-%d\" "
                    + " --merge"
                    + " --no-decorate -s ";
                var pSI = new ProcessStartInfo("cmd", "/c" + getConflictIdCmd);
                pSI.WorkingDirectory = _folderPath;
                pSI.RedirectStandardInput = true;
                pSI.RedirectStandardOutput = true;
                pSI.RedirectStandardError = true;
                pSI.CreateNoWindow = true;
                pSI.UseShellExecute = false;
                pSI.StandardOutputEncoding = Encoding.UTF8;
                LogManager.Current.AppendLogLine(getConflictIdCmd, true);
                _mergeConflictCommitCache.Clear();
                using (Process? process = Process.Start(pSI))
                {
                    if (process != null)
                    {
                        process.OutputDataReceived -= OnDataReceived;
                        process.ErrorDataReceived -= OnDataReceived;
                        process.OutputDataReceived += OnDataReceived;
                        process.ErrorDataReceived += OnDataReceived;
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                    }
                }

                _checkingState = CheckingState.ConflictFile;
                string getConflictFile = "git diff --name-only --diff-filter=U --relative";
                pSI = new ProcessStartInfo("cmd", "/c" + getConflictFile);
                pSI.WorkingDirectory = _folderPath;
                pSI.RedirectStandardInput = true;
                pSI.RedirectStandardOutput = true;
                pSI.RedirectStandardError = true;
                pSI.CreateNoWindow = true;
                pSI.UseShellExecute = false;
                pSI.StandardOutputEncoding = Encoding.UTF8;
                LogManager.Current.AppendLogLine(getConflictFile, true);
                _mergeConflictFileCache.Clear();
                using (Process? process = Process.Start(pSI))
                {
                    if (process != null)
                    {
                        process.OutputDataReceived -= OnDataReceived;
                        process.ErrorDataReceived -= OnDataReceived;
                        process.OutputDataReceived += OnDataReceived;
                        process.ErrorDataReceived += OnDataReceived;
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
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
            if (e.Data != null)
            {
                switch (_checkingState)
                {
                    case CheckingState.ConflictID:
                        {
                            LogManager.Current.AppendLogLine(e.Data);
                            var raw = e.Data;
                            var match = _mergeCLSubjectRegex.Match(raw);
                            if (match.Length > 0)
                            {
                                var hashID = match.Groups["hashid"].Value ?? "";
                                var dateTime = match.Groups["datetime"].Value ?? "";
                                var email = match.Groups["email"].Value ?? "";
                                var title = match.Groups["title"].Value ?? "";

                                var commit = new CommitVO()
                                {
                                    CommitDateTime = DateTime.ParseExact(dateTime, "HH:mm:ss yyyy-MM-dd",
                                               System.Globalization.CultureInfo.InvariantCulture),
                                    CommitId = hashID,
                                    CommitTitle = title,
                                    AuthorEmail = email,
                                };
                                _mergeConflictCommitCache.Add(commit);
                            }
                        }
                        break;
                    case CheckingState.ConflictFile:
                        {
                            LogManager.Current.AppendLogLine(e.Data);
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                _mergeConflictFileCache.Add(e.Data);
                            }
                        }
                        break;
                }

            }
        }

        protected override bool IsTaskPossible(object param)
        {
            switch (param)
            {
                case string data:
                    return !string.IsNullOrEmpty(data)
                        && Directory.Exists(data);
                default:
                    return false;
            }
        }
    }
}
