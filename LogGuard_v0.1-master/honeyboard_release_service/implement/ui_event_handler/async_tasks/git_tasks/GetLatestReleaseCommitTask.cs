using cyber_base.async_task;
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
    internal class GetLatestReleaseCommitTask : BaseRTParamAsyncTask
    {
        class SubjectDO
        {
            public int Count;
            public string TaskId = "";
            public string LatestHashId = "";
            public string LatestSubject = "";
        }

        private static readonly Regex _releaseCLSubjectRegex = new Regex(@"huy.td1_hashid:(?<hashid>[a-z0-9]{10}) huy.td1_subject:\[(?<taskid>\S+)\](?<subject>.*) huy.td1_description:(?<description>.*)");

        private string _folderPath;
        private string _versionFileName;
        private int _logCount = 10;
        private Dictionary<string, SubjectDO> _taskIdsMap;

        public GetLatestReleaseCommitTask(object param
            , Action<AsyncTaskResult>? completedCallback = null
            , string name = "Get latest release commit!")
            : base(param, name, completedCallback)
        {
            _folderPath = "";
            _versionFileName = "";
            switch (param)
            {
                case string[] data:
                    if (data.Length == 2)
                    {
                        _folderPath = data[0];
                        _versionFileName = data[1];
                    }
                    break;
                default:
                    throw new InvalidDataException("Param must be an array of string has 2 elements");
            }

            _taskIdsMap = new Dictionary<string, SubjectDO>();
            _estimatedTime = 3000;
            _reportDelay = 100;
            _delayTime = 3000;
        }

        protected override void DoCallback(object param, AsyncTaskResult result)
        {
            var max = 0;
            SubjectDO mostUseCL = new SubjectDO();
            foreach (var taskId in _taskIdsMap)
            {
                if (max < taskId.Value.Count)
                {
                    max = taskId.Value.Count;
                    mostUseCL = taskId.Value;
                }
            }
            dynamic res = new ExpandoObject();
            res.TaskId = mostUseCL.TaskId;
            res.HashId = mostUseCL.LatestHashId;
            res.Subject = mostUseCL.LatestSubject;
            result.Result = res;
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            try
            {
                string cmd = "git log -n " + _logCount + " --pretty=\"format:huy.td1_hashid:%h huy.td1_subject:%s huy.td1_description:%b\" --no-decorate -s " + _versionFileName;
                var pSI = new ProcessStartInfo("cmd", "/c" + cmd);
                pSI.WorkingDirectory = _folderPath;
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
                        process.OutputDataReceived -= OnDataReceived;
                        process.ErrorDataReceived -= OnDataReceived;
                        process.OutputDataReceived += OnDataReceived;
                        //process.ErrorDataReceived += OnDataReceived;
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
                var raw = e.Data;
                var match = _releaseCLSubjectRegex.Match(raw);
                if (match.Length > 0)
                {
                    var taskID = match.Groups["taskid"].ToString();
                    var hashID = match.Groups["hashid"].ToString();
                    var subject = match.Groups["subject"].ToString();

                    if (_taskIdsMap.ContainsKey(taskID))
                    {
                        _taskIdsMap[taskID].Count++;
                    }
                    else
                    {
                        _taskIdsMap.Add(taskID, new SubjectDO()
                        {
                            Count = 1,
                            TaskId = taskID,
                            LatestHashId = hashID,
                            LatestSubject = subject
                        });
                    }
                }
            }
        }

        protected override bool IsTaskPossible(object param)
        {
            switch (param)
            {
                case string[] data:
                    if (data.Length == 2)
                    {
                        return !string.IsNullOrEmpty(data[0])
                       && Directory.Exists(data[0])
                       && !string.IsNullOrEmpty(data[1])
                       && File.Exists(data[0] + "\\" + data[1]);
                    }
                    return false;

                default:
                    return false;
            }
        }
    }
}
