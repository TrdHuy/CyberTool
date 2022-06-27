using cyber_base.async_task;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using honeyboard_release_service.implement.log_manager;
using honeyboard_release_service.models.VOs;
using honeyboard_release_service.view_models.project_manager.items;
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
        Action<object, GetAllProjectBranchsTask, BranchItemViewModel?, bool>? _onReadBranchCallback;
        Action? _prepareGetAllProjectBranchs;

        private CyberTreeViewObservableCollection<ICyberTreeViewItemContext> _contextSourceCache;
        private List<BranchVO> _branchCache;

        public GetAllProjectBranchsTask(object param
            , Action<AsyncTaskResult>? callback = null
            , Action? prepareGetAllProjectBranchs = null
            , Action<object, GetAllProjectBranchsTask, BranchItemViewModel?, bool>? readBranchCallback = null
            , string name = "Getting all project's branchs") : base(param, name, callback)
        {
            _estimatedTime = 8000;
            _reportDelay = 100;
            _delayTime = 7000;
            _onReadBranchCallback = readBranchCallback;
            _prepareGetAllProjectBranchs = prepareGetAllProjectBranchs;

            _contextSourceCache = new CyberTreeViewObservableCollection<ICyberTreeViewItemContext>();
            _branchCache = new List<BranchVO>();
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            _prepareGetAllProjectBranchs?.Invoke();

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

        protected override void DoCallback(object param, AsyncTaskResult result)
        {
            dynamic newRes = new ExpandoObject();
            newRes.ContextSource = _contextSourceCache;
            newRes.Branchs = _branchCache;
            result.Result = newRes;
            result.MesResult = MessageAsyncTaskResult.Done;
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
                    LogManager.Current.AppendLogLine(e.Data);
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
                        var branchContext = CreateBranchSourceFromPath(
                            source: _contextSourceCache
                            , branchs: _branchCache
                            , path: branch
                            , isSetOnBranch: true);
                        _onReadBranchCallback?.Invoke(sender, this, branchContext, true);

                    }
                    else if (res.HEADBranch == ""
                        && raw.Contains("HEAD ->"))
                    {
                        var idx = raw.IndexOf("HEAD ->");
                        res.HEADBranch = raw.Substring(idx + 8);
                    }
                    else
                    {
                        var branchContext = CreateBranchSourceFromPath(
                            source: _contextSourceCache
                            , branchs: _branchCache
                            , path: raw
                            , isSetOnBranch: false);
                        _onReadBranchCallback?.Invoke(sender, this, branchContext, false);

                    }
                }
            }
        }

        private BranchItemViewModel? CreateBranchSourceFromPath(
            CyberTreeViewObservableCollection<ICyberTreeViewItemContext> source
            , List<BranchVO> branchs
            , string? path
            , bool isSetOnBranch = false)
        {
            if (string.IsNullOrEmpty(path)) return null;

            var splits = path.Split("/", StringSplitOptions.TrimEntries);
            string rootFolder = "";
            int startFolderIndex = 1;
            int lenght = splits.Length;
            var isRemote = false;
            BranchItemViewModel? parents;

            if (splits[0].Equals("remotes", StringComparison.CurrentCultureIgnoreCase))
            {
                rootFolder = "Remote";
                isRemote = true;
            }
            else if (splits[0].Equals("origin", StringComparison.CurrentCultureIgnoreCase))
            {
                rootFolder = "Remote";
                isRemote = true;
                startFolderIndex = 0;
            }
            else
            {
                rootFolder = "Local";
                startFolderIndex = 0;
            }
            parents = source[rootFolder] as BranchItemViewModel;

            if (parents == null)
            {
                var bVO = new BranchVO("", rootFolder);
                parents = new BranchItemViewModel(bVO);
                source.Add(parents);
            }

            string branchPath = "";
            for (int i = startFolderIndex; i < lenght; i++)
            {
                var current = parents?.Items[splits[i]];

                if (i == lenght - 1)
                    branchPath += splits[i];
                else
                    branchPath += splits[i] + "/";

                if (current == null)
                {
                    var bVO = new BranchVO(path: i == lenght - 1 ? branchPath : ""
                        , title: splits[i]
                        , isNode: i == lenght - 1
                        , isRemote: isRemote);
                    current = new BranchItemViewModel(bVO);
                    parents?.AddItem(current);

                    if (bVO.IsNode
                        && !string.IsNullOrEmpty(bVO.BranchPath))
                    {
                        branchs.Add(bVO);
                    }
                }
                parents = current as BranchItemViewModel;
            }

            if (isSetOnBranch)
            {
                if (parents != null)
                {
                    parents.IsSelected = true;
                }
            }

            return parents;
        }
    }
}
