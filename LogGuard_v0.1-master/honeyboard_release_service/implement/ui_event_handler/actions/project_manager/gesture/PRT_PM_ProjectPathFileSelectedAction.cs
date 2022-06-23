using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.io_tasks;
using honeyboard_release_service.models.VOs;
using honeyboard_release_service.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace honeyboard_release_service.implement.ui_event_handler.actions.project_manager.gesture
{
    internal class PRT_PM_ProjectPathFileSelectedAction : PM_ViewModelCommandExecuter
    {

        private bool _isOnBranchSet = false;
        private bool _isLatestVersionSet = false;
        private BaseAsyncTask? _getVersionHistoryTaskCache;
        public PRT_PM_ProjectPathFileSelectedAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger)
        {
        }

        protected async override void ExecuteCommand()
        {
            var branchCache = new CyberTreeViewObservableCollection<ICyberTreeViewItem>();

            BaseAsyncTask findVersionPathTask = new FindVersionPropertiesFileTask(PMViewModel.ProjectPath
                , (result) =>
                {
                    if (result.MesResult == MessageAsyncTaskResult.Done
                    || result.MesResult == MessageAsyncTaskResult.Finished)
                    {
                        PMViewModel.VersionPropertiesPath = result.Result?.ToString() ?? "";
                    }
                });
            BaseAsyncTask listAllBranch = new GetAllProjectBranchsTask(PMViewModel.ProjectPath
                , callback: (result) =>
                {
                    PMViewModel.BranchsSource = branchCache;
                }
                , readBranchCallback: (sender, task, branch) =>
                {
                    AddPath(branchCache, branch, task.Result);
                });

            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(findVersionPathTask);
            tasks.Add(listAllBranch);

            MultiAsyncTask multiTask = new MultiAsyncTask(mainFunc: tasks
                , cancellationTokenSource: new CancellationTokenSource()
                , name: "Importing project"
                , delayTime: 0
                , reportDelay: 100);
            var message = HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox("Importing project", multiTask);

            if (message != CyberContactMessage.Cancel
                && PMViewModel.VersionPropertiesPath != "")
            {
                if (_getVersionHistoryTaskCache != null
                    && !_getVersionHistoryTaskCache.IsCompleted
                    && !_getVersionHistoryTaskCache.IsCanceled
                    && !_getVersionHistoryTaskCache.IsFaulted)
                {
                    _getVersionHistoryTaskCache.Cancel();
                }
                BaseAsyncTask getVersionHistory = new GetVersionHistoryTask(
                    new string[] { PMViewModel.ProjectPath
                    , PMViewModel.VersionPropertiesPath }
                    , versionPropertiesFoundCallback: (prop, task) =>
                    {
                        dynamic data = prop;
                        CommitVO vVO = new CommitVO()
                        {
                            Name = data.Title,
                            ReleaseDateTime = DateTime.ParseExact(data.DateTime, "HH:mm:ss yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture),
                            AuthorEmail = data.Email,
                            CommitId = data.HashId,
                        };
                        if (!_isLatestVersionSet)
                        {
                            PMViewModel.LatestCommitVO = vVO;
                            _isLatestVersionSet = true;
                        }
                        PMViewModel.CurrentProjectVO?.AddCurrentBranchVersionVO(vVO);
                        PMViewModel.VersionHistoryItemContexts.Add(new VersionHistoryItemViewModel(vVO));
                    }
                    , taskFinishedCallback: (s) =>
                    {
                        PMViewModel.IsLoadingProjectVersionHistory = false;
                    });
                _getVersionHistoryTaskCache = getVersionHistory;
                PMViewModel.VersionHistoryItemContexts.Clear();
                PMViewModel.VersionHistoryListTipVisibility = Visibility.Collapsed;
                PMViewModel.IsLoadingProjectVersionHistory = true;
                await getVersionHistory.Execute();
            }
            else if (message == CyberContactMessage.Cancel)
            {
            }
        }

        private void AddPath(CyberTreeViewObservableCollection<ICyberTreeViewItem> source,
            string? path,
            AsyncTaskResult result)
        {
            if (result.Result == null)
            {
                return;
            }

            dynamic res = result.Result;
            var isShouldSelectItem = res.OnBranch != "" && !_isOnBranchSet;

            if (string.IsNullOrEmpty(path)) return;

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
                    PMViewModel.CurrentProjectVO?.AddProjectBranch(bVO);
                }
                parents = current as BranchItemViewModel;
            }

            if (isShouldSelectItem)
            {
                if (parents != null)
                {
                    PMViewModel.ForceSetSelectedBranch(parents);
                    parents.IsSelected = true;
                    _isOnBranchSet = true;
                }
            }
        }

    }
}
