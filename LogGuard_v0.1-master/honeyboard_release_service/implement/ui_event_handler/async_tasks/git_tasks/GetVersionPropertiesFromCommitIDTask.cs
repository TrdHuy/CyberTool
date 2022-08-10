using cyber_base.async_task;
using honeyboard_release_service.implement.log_manager;
using honeyboard_release_service.models.VOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks
{
    internal class GetVersionPropertiesFromCommitIDTask : BaseRTParamAsyncTask
    {
        private string _versionFileName;
        private string _projectPath;
        private string _hashid;
        
        public GetVersionPropertiesFromCommitIDTask(object param
            , Action<AsyncTaskResult>? callback = null
            , string name = "Getting version properties") : base(param, name, callback)
        {
            switch (param)
            {
                case string[] data:
                    if (data.Length == 3)
                    {
                        _projectPath = data[0];
                        _versionFileName = data[1];
                        _hashid = data[2];
                    }
                    else
                    {
                        throw new InvalidDataException("Param must be an array of string has 3 elements");
                    }
                    break;
                default:
                    throw new InvalidDataException("Param must be an array of string has 3 elements");
            }

            _isEnableReport = false;
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            string gitShowCommitCmd = "git show " + _hashid + ":" + _versionFileName;
            var pSI = new ProcessStartInfo("cmd", "/c" + gitShowCommitCmd);
            pSI.WorkingDirectory = _projectPath;
            pSI.RedirectStandardInput = true;
            pSI.RedirectStandardOutput = true;
            pSI.RedirectStandardError = true;
            pSI.CreateNoWindow = true;
            pSI.UseShellExecute = false;
            pSI.StandardOutputEncoding = Encoding.UTF8;

            LogManager.Current.AppendLogLine(gitShowCommitCmd, true);
            using (var process = Process.Start(pSI))
            {
                if (process != null)
                {
                    result.Result = process.StandardOutput.ReadToEnd();
                }
            }
        }


        protected override bool IsTaskPossible(object param)
        {
            switch (param)
            {
                case string[] data:
                    return !string.IsNullOrEmpty(data[1])
                   && File.Exists(data[0] + "\\" + data[1])
                   && !string.IsNullOrEmpty(data[2]);
                default:
                    return false;
            }
        }
    }
}
