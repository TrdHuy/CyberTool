using cyber_base.async_task;
using honeyboard_release_service.models.VOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks
{
    internal class GetVersionHistoryTask : BaseRTParamAsyncTask
    {
        private string _versionFileName;
        private string _projectPath;
        private static readonly Regex _gitLogRegex =
            new Regex(@"huy.td1_hashid:(?<hashid>[a-z0-9]{5,20}) " +
                @"huy.td1_subject:((?<subjectid>\[\S+\])(?<title>.+)) " +
                @"huy.td1_datetime:(?<datetime>\d{2}:\d{2}:\d{2} \d{4}-\d{2}-\d{2}) " +
                @"huy.td1_email:(?<email>\S+\@samsung.com)");
        private static readonly Regex _majorRegex = new Regex(@"\s*(?<property>" + VersionPropertiesVO.VERSION_MAJOR_PROPERTY_NAME + @")=(?<value>\d+)");
        private static readonly Regex _minorRegex = new Regex(@"\s*(?<property>" + VersionPropertiesVO.VERSION_MINOR_PROPERTY_NAME + @")=(?<value>\d+)");
        private static readonly Regex _patchRegex = new Regex(@"\s*(?<property>" + VersionPropertiesVO.VERSION_PATCH_PROPERTY_NAME + @")=(?<value>\d+)");
        private static readonly Regex _revisionRegex = new Regex(@"\s*(?<property>" + VersionPropertiesVO.VERSION_REVISION_PROPERTY_NAME + @")=(?<value>\d+)");
        private Action<object, GetVersionHistoryTask>? _versionPropertiesFoundCallback;

        public GetVersionHistoryTask(object param
            , Action<AsyncTaskResult>? callback = null
            , Action<object, GetVersionHistoryTask>? versionPropertiesFoundCallback = null
            , string name = "Getting version history") : base(param, name, callback)
        {
            switch (param)
            {
                case string[] data:
                    if (data.Length == 2)
                    {
                        _projectPath = data[0];
                        _versionFileName = data[1];
                    }
                    else
                    {
                        throw new InvalidDataException("Param must be an array of string has 2 elements");
                    }
                    break;
                default:
                    throw new InvalidDataException("Param must be an array of string has 2 elements");
            }

            _estimatedTime = 8000;
            _reportDelay = 100;
            _delayTime = 7000;
            _versionPropertiesFoundCallback = versionPropertiesFoundCallback;
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            string getGitLogCmd = "git log " +
                "--pretty=format:\"huy.td1_hashid:%h " +
                    "huy.td1_subject:%s " +
                    "huy.td1_datetime:%ad " +
                    "huy.td1_email:%ae\" " +
                "--date=format:\"%H:%M:%S %Y-%m-%d\" " +
                "-s " + _versionFileName;
            var pSI = new ProcessStartInfo("cmd", "/c" + getGitLogCmd);
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

                        var match = _gitLogRegex.Match(line);
                        if (match.Success)
                        {
                            var hashID = match.Groups["hashid"].Value ?? "";
                            var dateTime = match.Groups["datetime"].Value ?? "";
                            var email = match.Groups["email"].Value ?? "";
                            var subjectId = match.Groups["subjectid"].Value ?? "";
                            var title = match.Groups["title"].Value ?? "";
                            var properties = GetPatchVersion("", token);

                            dynamic ele = new ExpandoObject();
                            ele.HashId = hashID;
                            ele.DateTime = dateTime;
                            ele.Email = email;
                            ele.SubjectID = subjectId;
                            ele.Title = title;
                            ele.Version = "";

                            if (properties != null)
                            {
                                if (properties.ContainsKey("major"))
                                {
                                    ele.Major = properties["major"];
                                    ele.Version += properties["major"];
                                }
                                if (properties.ContainsKey("minor"))
                                {
                                    ele.Minor = properties["minor"];
                                    ele.Version += "." + properties["minor"];
                                }
                                if (properties.ContainsKey("patch"))
                                {
                                    ele.Patch = properties["patch"];
                                    ele.Version += "." + properties["patch"];
                                }
                                if (properties.ContainsKey("revision"))
                                {
                                    ele.Revision = properties["revision"];
                                    ele.Version += "." + properties["revision"];
                                }
                            }

                            _versionPropertiesFoundCallback?.Invoke(ele, this);

                        }
                    }

                }
            }
        }

        private Dictionary<string, string>? GetPatchVersion(string hashID, CancellationTokenSource token)
        {
            if (string.IsNullOrEmpty(hashID)) return null;

            var propertiesMap = new Dictionary<string, string>();
            string gitShowCommitCmd = "git show " + hashID + ":" + _versionFileName;
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
                                propertiesMap.Add(match.Groups["property"].ToString(), match.Groups["value"].ToString());
                            }
                        }
                    }

                }
            }

            return propertiesMap;
        }

        protected override bool IsTaskPossible(object param)
        {
            switch (param)
            {
                case string[] data:
                    return !string.IsNullOrEmpty(data[1])
                   && File.Exists(data[0] + "\\" + data[1]);
                default:
                    return false;
            }
        }
    }
}
