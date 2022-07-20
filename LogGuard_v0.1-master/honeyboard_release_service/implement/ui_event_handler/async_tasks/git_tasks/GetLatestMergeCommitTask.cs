using cyber_base.async_task;
using honeyboard_release_service.implement.log_manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks
{
    internal class GetLatestMergeCommitTask : BaseRTParamAsyncTask
    {
        private static readonly Regex _mergeCLSubjectRegex
            = new Regex(@"huy.td1_hashid:(?<hashid>[a-z0-9]{5,20}) " +
                @"huy.td1_subject:(?<subject>.+) " +
                @"huy.td1_description:(?<description>.*)");
        private static readonly Regex _subjectLogRegex = new Regex(@"(?<subjectid>\[\S+\])(?<title>.+)");
        private string _folderPath = "";
        private object? _resultCache;

        public GetLatestMergeCommitTask(object param
           , Action<AsyncTaskResult>? completedCallback = null
           , string name = "Get latest merge commit!")
           : base(param, name, completedCallback)
        {
            _folderPath = "";
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
        }

        protected override void DoCallback(object param, AsyncTaskResult result)
        {
            result.Result = _resultCache;
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            try
            {
                string cmd = "git log -n 1"
                    + " --pretty=\"format:huy.td1_hashid:%h huy.td1_subject:%s huy.td1_description:%b\"" 
                    + " --merges --no-decorate -s ";
                var pSI = new ProcessStartInfo("cmd", "/c" + cmd);
                pSI.WorkingDirectory = _folderPath;
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
                LogManager.Current.AppendLogLine(e.Data);
                var raw = e.Data;
                var match = _mergeCLSubjectRegex.Match(raw);
                if (match.Length > 0)
                {
                    var taskID = "";
                    var hashID = match.Groups["hashid"].ToString();
                    var subject = match.Groups["subject"].ToString();
                    var des = match.Groups["description"].ToString();

                    var matchSubject = _subjectLogRegex.Match(subject);
                    if (matchSubject.Success)
                    {
                        taskID = matchSubject.Groups["subjectid"].Value ?? "";
                        subject = matchSubject.Groups["title"].Value ?? "";
                    }

                    dynamic result = new ExpandoObject();
                    result.Subject = subject;
                    result.Description = des;
                    result.HashID = hashID;
                    result.TaskId = taskID;

                    _resultCache = result;
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
