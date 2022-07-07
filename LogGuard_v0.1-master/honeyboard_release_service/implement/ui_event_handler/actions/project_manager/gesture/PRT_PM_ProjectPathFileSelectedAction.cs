using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.implement.project_manager;
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
        public PRT_PM_ProjectPathFileSelectedAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger)
        {
        }

        protected override void ExecuteCommand()
        {
            var branchCache = new CyberTreeViewObservableCollection<ICyberTreeViewItemContext>();

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
                , prepareGetAllProjectBranchs: () =>
                {
                    ReleasingProjectManager
                            .Current
                            .CurrentProjectVO?
                            .Branchs.Clear();
                }
                , callback: (result) =>
                {
                    dynamic? newRes = result.Result;
                    if (newRes != null)
                    {
                        PMViewModel.BranchsSource = newRes.ContextSource;
                        var branchs = newRes.Branchs;
                    }
                }
                , readBranchCallback: (sender, task, branch, isOnBranch) =>
                {
                    if (branch != null)
                    {
                        ReleasingProjectManager
                            .Current.AddProjectBranch(branch.Branch);
                    }

                    if (isOnBranch && branch != null)
                    {
                        PMViewModel.ForceSetSelectedBranch(branch);
                    }
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
                ReleasingProjectManager
                    .Current
                    .UpdateVersionHistoryTimelineInBackground();
            }
            else if (message == CyberContactMessage.Cancel)
            {
            }
        }

    }
}
