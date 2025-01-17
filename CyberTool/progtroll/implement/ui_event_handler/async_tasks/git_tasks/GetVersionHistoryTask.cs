﻿using cyber_base.async_task;
using progtroll.implement.log_manager;
using progtroll.models.VOs;
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

namespace progtroll.implement.ui_event_handler.async_tasks.git_tasks
{
    internal class GetVersionHistoryTask : BaseRTParamAsyncTask
    {
        private string _versionFileName;
        private string _projectPath;
        private string _fromCommitId = "";
        private string _toCommitId = "";
        private string _getGitLogCmd = "";

        private static readonly Regex _gitLogRegex =
            new Regex(@"huy.td1_hashid:(?<hashid>[a-z0-9]{5,20}) " +
                @"huy.td1_subject:(?<subject>.+) " +
                @"huy.td1_datetime:(?<datetime>.+) " +
                @"huy.td1_email:(?<email>\S+\@samsung.com)");
        private static readonly Regex _subjectLogRegex = new Regex(@"(?<subjectid>\[\S+\])(?<title>.+)");

        private Action<object, GetVersionHistoryTask>? _versionPropertiesFoundCallback;
        
        public GetVersionHistoryTask(object param
            , Action<AsyncTaskResult>? taskFinishedCallback = null
            , Action<object, GetVersionHistoryTask>? versionPropertiesFoundCallback = null
            , string name = "Getting version history") : base(param, name, taskFinishedCallback)
        {
            switch (param)
            {
                case string[] data:
                    if (data.Length == 2)
                    {
                        _projectPath = data[0];
                        _versionFileName = data[1];

                        _getGitLogCmd = "git log " +
                            "--pretty=format:\"huy.td1_hashid:%h " +
                            "huy.td1_subject:%s " +
                            "huy.td1_datetime:%ad " +
                            "huy.td1_email:%ae\" " +
                            "--date=format:\"%H:%M:%S %Y-%m-%d\" " +
                            "-s " + _versionFileName;
                    }
                    else if (data.Length == 4)
                    {
                        _projectPath = data[0];
                        _versionFileName = data[1];
                        _fromCommitId = data[2];
                        _toCommitId = data[3];

                        _getGitLogCmd = "git log " +
                            "--pretty=format:\"huy.td1_hashid:%h " +
                            "huy.td1_subject:%s " +
                            "huy.td1_datetime:%ad " +
                            "huy.td1_email:%ae\" " +
                            "--date=format:\"%d-%m-%Y %H:%M\" " +
                            _fromCommitId + ".." + _toCommitId;
                    }
                    else
                    {
                        throw new InvalidDataException("Param must be an array of string has at least 2 elements");
                    }
                    break;
                default:
                    throw new InvalidDataException("Param must be an array of string has at least 2 elements");
            }

            _estimatedTime = 8000;
            _reportDelay = 100;
            _delayTime = 7000;
            _versionPropertiesFoundCallback = versionPropertiesFoundCallback;
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            var pSI = new ProcessStartInfo("cmd", "/c" + _getGitLogCmd);
            pSI.WorkingDirectory = _projectPath;
            pSI.RedirectStandardInput = true;
            pSI.RedirectStandardOutput = true;
            pSI.RedirectStandardError = true;
            pSI.CreateNoWindow = true;
            pSI.UseShellExecute = false;
            pSI.StandardOutputEncoding = Encoding.UTF8;

            LogManager.Current.AppendLogLine(_getGitLogCmd, true);
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
                            var subject = match.Groups["subject"].Value ?? "";
                            var subjectId = "";
                            var title = subject;

                            var matchSubject = _subjectLogRegex.Match(subject);
                            if (matchSubject.Success)
                            {
                                subjectId = matchSubject.Groups["subjectid"].Value ?? "";
                                title = matchSubject.Groups["title"].Value ?? "";
                            }

                            dynamic ele = new ExpandoObject();
                            ele.HashId = hashID;
                            ele.DateTime = dateTime;
                            ele.Email = email;
                            ele.SubjectID = subjectId;
                            ele.Title = title;
                            ele.Version = "";

                            _versionPropertiesFoundCallback?.Invoke(ele, this);

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
                   && File.Exists(data[1]);
                default:
                    return false;
            }
        }
    }
}
