using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using cyber_base.utils;
using cyber_base.view_model;
using progtroll.implement.project_manager;
using progtroll.implement.ui_event_handler.async_tasks.git_tasks;
using progtroll.models.VOs;
using progtroll.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.actions.project_manager.button
{
    internal class PRT_PM_FetchProjectButtonAction : PM_ViewModelCommandExecuter
    {
        public PRT_PM_FetchProjectButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
           : base(actionID, builderID, viewModel, logger)
        {
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            if (ReleasingProjectManager
                    .Current
                    .CurrentImportedProjectVO == null)
            {
                ProgTroll.Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("Please import project first!");
                return false;
            }

            return base.CanExecute(dataTransfer);
        }

        protected override void ExecuteCommand()
        {
            BaseAsyncTask fetchTask = new CommonGitTask(
                folderPath: ReleasingProjectManager.Current.ProjectPath
                , gitCmd: "git fetch"
                , callback: (result) =>
                {
                    // Append log for user here
                }
                , name: "Fetching"
                , estimatedTime: 4000);
            BaseAsyncTask listAllBranch = new GetAllProjectBranchsTask(PMViewModel.ProjectPath
                , prepareGetAllProjectBranchs: () =>
                {
                    ReleasingProjectManager
                            .Current
                            .CurrentImportedProjectVO?
                            .Branchs.Clear();
                }
                , callback: (result) =>
                {
                    dynamic? newRes = result.Result;
                    if (newRes != null)
                    {
                        ReleasingProjectManager
                            .Current
                            .SetCurrentProjectBranchContextSource(newRes.ContextSource);
                        var branchs = newRes.Branchs;
                    }
                }
                , readBranchCallback: (sender, task, branch, isOnBranch) =>
                {
                    if (branch != null)
                    {
                        ReleasingProjectManager
                            .Current
                            .AddBranchToCurrentProject(branch.Branch);
                    }

                    if (isOnBranch && branch != null)
                    {
                        PMViewModel.ForceSetSelectedBranch(branch);
                    }
                });


            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(fetchTask);
            tasks.Add(listAllBranch);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
               , new CancellationTokenSource()
               , null
               , name: "Fetching"
               , delayTime: 0
               , reportDelay: 100);
            ProgTroll.Current.ServiceManager?.App.OpenMultiTaskBox(
                title: "Fetching"
                , task: multiTask
                , isCancelable: false);

            ReleasingProjectManager
                    .Current
                    .UpdateVersionHistoryTimelineInBackground();
        }

    }
}
