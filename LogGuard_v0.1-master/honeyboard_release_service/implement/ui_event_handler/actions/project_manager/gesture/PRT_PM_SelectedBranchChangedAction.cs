using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using honeyboard_release_service.models.VOs;
using honeyboard_release_service.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace honeyboard_release_service.implement.ui_event_handler.actions.project_manager.gesture
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
                .CurrentProjectVO == null)
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

