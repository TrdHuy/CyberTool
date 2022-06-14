using cyber_base.async_task;
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
    internal class GetReleasedCommitVersionPropertiesTask : BaseRTParamAsyncTask
    {
        private string _versionFileName;
        private string _projectPath;
        private string _hashid;
        private Action<object, object, GetReleasedCommitVersionPropertiesTask>? _versionPropertiesFoundCallback;
        private static readonly Regex _majorRegex = new Regex(@"\s*(?<property>" + VersionPropertiesVO.VERSION_MAJOR_PROPERTY_NAME + @")=(?<value>\d+)");
        private static readonly Regex _minorRegex = new Regex(@"\s*(?<property>" + VersionPropertiesVO.VERSION_MINOR_PROPERTY_NAME + @")=(?<value>\d+)");
        private static readonly Regex _patchRegex = new Regex(@"\s*(?<property>" + VersionPropertiesVO.VERSION_PATCH_PROPERTY_NAME + @")=(?<value>\d+)");
        private static readonly Regex _revisionRegex = new Regex(@"\s*(?<property>" + VersionPropertiesVO.VERSION_REVISION_PROPERTY_NAME + @")=(?<value>\d+)");

        public GetReleasedCommitVersionPropertiesTask(object param
            , Action<AsyncTaskResult>? callback = null
            , Action<object, object, GetReleasedCommitVersionPropertiesTask>? versionPropertiesFoundCallback = null
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
            _versionPropertiesFoundCallback = versionPropertiesFoundCallback;
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            var propertiesMap = new Dictionary<string, string>();
            string gitShowCommitCmd = "git show " + _hashid + ":" + _versionFileName;
            var pSI = new ProcessStartInfo("cmd", "/c" + gitShowCommitCmd);
            pSI.WorkingDirectory = _projectPath;
            pSI.RedirectStandardInput = true;
            pSI.RedirectStandardOutput = true;
            pSI.RedirectStandardError = true;
            pSI.CreateNoWindow = true;
            pSI.UseShellExecute = false;
            pSI.StandardOutputEncoding = Encoding.UTF8;

            using (var process = Process.Start(pSI))
            {
                if (process != null)
                {
                    string? line;
                    while ((line = process.StandardOutput.ReadLine()) != null)
                    {
                        if (token.IsCancellationRequested)
                        {
                            throw new OperationCanceledException();
                        }

                        if (!line.StartsWith("#"))
                        {
                            Regex? selectedRegex = null;
                            if (_majorRegex.IsMatch(line))
                            {
                                selectedRegex = _majorRegex;
                            }
                            else if (_minorRegex.IsMatch(line))
                            {
                                selectedRegex = _minorRegex;
                            }
                            else if (_patchRegex.IsMatch(line))
                            {
                                selectedRegex = _patchRegex;
                            }
                            else if (_revisionRegex.IsMatch(line))
                            {
                                selectedRegex = _revisionRegex;
                            }

                            if (selectedRegex != null)
                            {
                                var match = selectedRegex.Match(line);
                                _versionPropertiesFoundCallback?.Invoke(
                                    match.Groups["property"].ToString()
                                    , match.Groups["value"].ToString()
                                    , this);
                            }
                        }
                    }

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
