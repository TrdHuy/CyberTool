using cyber_base.async_task;
using cyber_base.implement.async_task;
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
using System.Windows;

namespace progtroll.implement.ui_event_handler.actions.project_manager.gesture
{
    internal class PRT_PM_SelectedBranchChangedAction : PM_ViewModelCommandExecuter
    {
        public PRT_PM_SelectedBranchChangedAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger)
        {
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            if (ReleasingProjectManager
                .Current
                .CurrentImportedProjectVO == null)
            {
                return false;
            }
            return base.CanExecute(dataTransfer);
        }

        protected override void ExecuteCommand()
        {
            BaseAsyncTask checkoutTask = new CommonGitTask(folderPath: PMViewModel.ProjectPath
               , gitCmd: "git checkout " 
                    + ReleasingProjectManager.Current.SelectedBranchPath 
                    + " --force"
               , null
               , name: "Checking out branch: " 
                    + ReleasingProjectManager.Current.SelectedBranchPath
               , estimatedTime: 2000);

            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(checkoutTask);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                , new CancellationTokenSource()
                , null
                , name: "Checking out"
                , delayTime: 0
                , reportDelay: 100);

            HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox("Checking out", multiTask, isCancelable: false);

            ReleasingProjectManager
                    .Current
                    .UpdateVersionHistoryTimelineInBackground();
        }

    }
}

