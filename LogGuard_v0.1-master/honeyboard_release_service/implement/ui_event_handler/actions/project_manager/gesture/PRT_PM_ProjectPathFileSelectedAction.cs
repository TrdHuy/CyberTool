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
        private BaseAsyncTask? _getVersionHistoryTaskCache;
        public PRT_PM_ProjectPathFileSelectedAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger)
        {
        }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();

            var branchCache = new CyberTreeViewObservableCollection<ICyberTreeViewItem>();

            BaseAsyncTask findVersionPathTask = new FindVersionPropertiesFileTask(PMViewModel.ProjectPath
                , (result) =>
                {
                    if (result.MesResult == MessageAsyncTaskResult.Done
                    || result.MesResult == MessageAsyncTaskResult.Finished)
                    {
                        PMViewModel.VersionPropertiesPath = result.Result?.ToString();
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

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                , new CancellationTokenSource()
                , Callback
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
                        VersionPropertiesVO vpVO = new VersionPropertiesVO();

                        try
                        {
                            vpVO.Version = data.Version;
                            vpVO.Major = data.Major;
                            vpVO.Minor = data.Minor;
                            vpVO.Patch = data.Patch;
                            vpVO.Revision = data.Revision;
                        }
                        catch { }

                        VersionVO vVO = new VersionVO()
                        {
                            Name = vpVO.Version,
                            ReleaseDateTime = DateTime.ParseExact(data.DateTime, "HH:mm:ss yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture),
                            AuthorEmail = data.Email,
                            CommitId = data.HashId,
                            Properties = vpVO,
                        };
                        if (vVO.Name == "")
                        {
                            vVO.Name = data.Title;
                        }
                        PMViewModel.VersionHistoryItemContexts.Add(new VersionHistoryItemViewModel(vVO));
                    }
                    , callback: (s) =>
                    {
                        PMViewModel.IsLoadingProjectVersionHistory = false; 
                    });
                _getVersionHistoryTaskCache = getVersionHistory;
                PMViewModel.VersionHistoryItemContexts.Clear();
                PMViewModel.VersionHistoryListTipVisibility = Visibility.Collapsed;
                PMViewModel.IsLoadingProjectVersionHistory = true;
                getVersionHistory.Execute();
            }

        }

        private async Task<AsyncTaskResult> Callback(List<AsyncTaskResult> results, AsyncTaskResult result)
        {
            return result;
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

            var splits = path.Split("/", System.StringSplitOptions.TrimEntries);
            var rootFolder = "";
            int startFolderIndex = 1;
            int lenght = splits.Length;
            var isRemote = false;
            BranchItemViewModel? parents;

            if (splits[0].Equals("remotes", System.StringComparison.CurrentCultureIgnoreCase))
            {
                rootFolder = "Remote";
                isRemote = true;
            }
            else if (splits[0].Equals("origin", System.StringComparison.CurrentCultureIgnoreCase))
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
                parents = new BranchItemViewModel("", rootFolder);
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


                if (current == null && i < lenght - 1)
                {
                    current = new BranchItemViewModel("", splits[i]);
                    parents?.AddItem(current);
                }
                else if (current == null && i == lenght - 1)
                {
                    current = new BranchItemViewModel(branchPath
                        , splits[i]
                        , isNode: true
                        , isRemote: isRemote);
                    parents?.AddItem(current);
                }
                parents = current as BranchItemViewModel;
            }

            if (isShouldSelectItem && parents != null)
            {
                PMViewModel.SelectedItem = parents;
                parents.IsSelected = true;
                _isOnBranchSet = true;
            }
        }

    }
}
