using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.actions.project_manager.button
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
                    .CurrentProjectVO == null)
            {
                HoneyboardReleaseService.Current
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

            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(fetchTask);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
               , new CancellationTokenSource()
               , null
               , name: "Fetching"
               , delayTime: 0
               , reportDelay: 100);
            HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox(
                title: "Fetching"
                , task: multiTask
                , isCancelable: false);

            ReleasingProjectManager
                    .Current
                    .UpdateVersionHistoryTimeline();
        }

    }
}
