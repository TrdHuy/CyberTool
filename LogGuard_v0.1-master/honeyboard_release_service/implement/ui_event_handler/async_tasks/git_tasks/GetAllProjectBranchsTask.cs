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
    internal class GetAllProjectBranchsTask : BaseRTParamAsyncTask
    {
        Action<object, GetAllProjectBranchsTask, string>? _onReadBranchCallback;

        public GetAllProjectBranchsTask(object param
            , Action<AsyncTaskResult>? callback = null
            , Action<object, GetAllProjectBranchsTask, string>? readBranchCallback = null
            , string name = "Getting all project's branchs") : base(param, name, callback)
        {
            _estimatedTime = 8000;
            _reportDelay = 100;
            _delayTime = 7000;
            _onReadBranchCallback = readBranchCallback;
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            dynamic tmp = new ExpandoObject();
            tmp.Data = "";
            tmp.OnBranch = "";
            tmp.HEADBranch = "";
            _result.Result = tmp;
            try
            {
                string proPath = param.ToString() ?? "";
                string cmd = "git branch -a";

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

        protected override bool IsTaskPossible(object param)
        {
            switch (param)
            {
                case string proPath:
                    return !(string.IsNullOrEmpty(proPath)
                        || !Directory.Exists(proPath));
                default:
                    return false;
            }
        }
        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_result.Result == null)
            {
                return;
            }

            lock (_result.Result)
            {
                dynamic res = _result.Result;

                if (e.Data != null)
                {
                    var raw = e.Data;
                    res.Data += raw + "\n";
                    if (res.OnBranch == ""
                         && raw.StartsWith("*"))
                    {
                        var match = Regex.Match(raw, @"\* \((?<head>HEAD detached (from|at) )(?<branch>\S+)\)");
                        string branch = "";
                        if (match.Length > 0)
                        {
                            branch = match.Groups["branch"].ToString();
                        }
                        else
                        {
                            branch = raw.Substring(1);
                        }
                        res.OnBranch = branch;
                        _onReadBranchCallback?.Invoke(sender, this, branch);
                    }
                    else if (res.HEADBranch == ""
                        && raw.Contains("HEAD ->"))
                    {
                        var idx = raw.IndexOf("HEAD ->");
                        res.HEADBranch = raw.Substring(idx + 8);
                    }
                    else
                    {
                        _onReadBranchCallback?.Invoke(sender, this, raw);
                    }
                }


            }
        }

    }
}
