using cyber_base.async_task;
using progtroll.implement.log_manager;
using progtroll.implement.project_manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.async_tasks.git_tasks
{
    internal enum MergeResult
    {
        None = 0,
        Success = 1,
        Error = 2,
        Conflict = 3,
        UpToDate = 4,
    }

    internal class MergeBranchTask : BaseRTParamAsyncTask
    {
        private string[] CONFLICT_SIGNALS = new string[] { "CONFLICT (content):"
            ,"CONFLICT (add/add):"};
        private string[] ERROR_SIGNALS = new string[] { "error:" };
        private string[] UP_TO_DATE_SIGNAL = new string[] { "Already up-to-date." };

        private string _folderPath = "";
        private string _inceptionBranchPath = "";
        private string _mergeMessage = "";

        private bool _isSetMergeResult = false;
        private MergeResult _mergeResult;
        private string _outputCache = "";

        public MergeBranchTask(object param
           , Action<AsyncTaskResult>? completedCallback = null
           , string name = "Merging branch")
           : base(param, name, completedCallback)
        {
            switch (param)
            {
                case string[] data:
                    if (data.Length == 3)
                    {
                        _folderPath = data[0];
                        _inceptionBranchPath = data[1];
                        _mergeMessage = data[2];
                    }
                    break;
                default:
                    throw new InvalidDataException("Param must be an array of string with 3 elements");
            }
            _estimatedTime = 3000;
            _reportDelay = 100;
            _delayTime = 3000;
        }

        protected override void DoCallback(object param, AsyncTaskResult result)
        {
            if (!_isSetMergeResult)
            {
                _mergeResult = MergeResult.Success;
                _isSetMergeResult = true;
            }

            dynamic res = new ExpandoObject();
            res.MergeResult = _mergeResult;
            res.OutputCache = _outputCache;
            result.Result = res;
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            try
            {
                string cmd = "git merge " + _inceptionBranchPath + " -m " + _mergeMessage;
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
                _outputCache += e.Data + "\n";
                LogManager.Current.AppendLogLine(e.Data);
                if (!_isSetMergeResult)
                {

                    if (CONFLICT_SIGNALS.Any(e.Data.Contains))
                    {
                        _mergeResult = MergeResult.Conflict;
                        _isSetMergeResult = true;
                    }
                    else if (ERROR_SIGNALS.Any(e.Data.Contains))
                    {
                        _mergeResult = MergeResult.Error;
                        _isSetMergeResult = true;
                    }
                    else if (UP_TO_DATE_SIGNAL.Any(e.Data.Contains))
                    {
                        _mergeResult = MergeResult.UpToDate;
                        _isSetMergeResult = true;
                    }
                }
            }
        }

        protected override bool IsTaskPossible(object param)
        {
            switch (param)
            {
                case string[] data:
                    return data.Length == 3
                        && !string.IsNullOrEmpty(_folderPath)
                        && !string.IsNullOrEmpty(_mergeMessage)
                        && Directory.Exists(_folderPath)
                        && ReleasingProjectManager
                            .Current
                            .GetBranchOfCurrentProjectFromPath(_inceptionBranchPath) != null;
                default:
                    return false;
            }
        }
    }
}
