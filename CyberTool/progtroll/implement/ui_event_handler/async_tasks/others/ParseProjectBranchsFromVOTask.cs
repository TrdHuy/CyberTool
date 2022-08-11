using cyber_base.async_task;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using progtroll.models.VOs;
using progtroll.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.async_tasks.others
{
    internal class ParseProjectBranchsFromVOTask : BaseRTParamAsyncTask
    {
        private CyberTreeViewObservableCollection<ICyberTreeViewItemContext> _contextSourceCache;
        private BranchVO? _onBranch;
        private Dictionary<string, BranchVO>? _branchData;

        public ParseProjectBranchsFromVOTask(object param
            , Action<AsyncTaskResult>? callback = null
            , string name = "Importing branchs from user data") : base(param, name, callback)
        {
            _estimatedTime = 3000;
            _reportDelay = 100;
            _delayTime = 3000;

            _contextSourceCache = new CyberTreeViewObservableCollection<ICyberTreeViewItemContext>();
            switch (param)
            {
                case object[] data:
                    if (data.Length == 2)
                    {
                        _branchData = data[0] as Dictionary<string, BranchVO>;
                        _onBranch = data[1] as BranchVO;
                    }
                    break;
                default:
                    throw new InvalidDataException("Param must be a List of BranchVO");
            }
            ArgumentNullException.ThrowIfNull(_branchData);
            ArgumentNullException.ThrowIfNull(_onBranch);
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            if (_branchData != null && _onBranch != null)
            {
                foreach (var branch in _branchData.Values)
                {
                    CreateBranchSourceFromPath(_contextSourceCache
                        , branch.BranchPath
                        , branch.BranchPath == _onBranch.BranchPath);
                }
            }

        }

        protected override bool IsTaskPossible(object param)
        {
            switch (param)
            {
                case object[] data:
                    return data.Length == 2
                        && _branchData != null
                        && _onBranch != null;
                default:
                    return false;
            }
        }

        protected override void DoCallback(object param, AsyncTaskResult result)
        {
            result.Result = _contextSourceCache;
            result.MesResult = MessageAsyncTaskResult.Done;
        }

        private BranchItemViewModel? CreateBranchSourceFromPath(
            CyberTreeViewObservableCollection<ICyberTreeViewItemContext> source
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
