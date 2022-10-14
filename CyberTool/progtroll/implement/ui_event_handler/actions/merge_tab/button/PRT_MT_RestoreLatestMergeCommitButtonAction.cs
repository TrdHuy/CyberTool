using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view_model;
using progtroll.implement.project_manager;
using progtroll.implement.ui_event_handler.async_tasks.git_tasks;
using progtroll.implement.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.actions.merge_tab.button
{
    internal class PRT_MT_RestoreLatestMergeCommitButtonAction : BaseViewModelCommandExecuter
    {
        public PRT_MT_RestoreLatestMergeCommitButtonAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger)
        {
        }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var mTViewModel = ViewModelManager.Current.MTViewModel;
            var projectPath = ReleasingProjectManager.Current.ProjectPath;

            if (string.IsNullOrEmpty(projectPath))
            {
                ProgTroll.Current.ServiceManager?.App.ShowWaringBox("Please import project and version properties file first!");
                return;
            }
            
            BaseAsyncTask getLatestMergeCommitTask = new GetLatestMergeCommitTask(projectPath
               , (result) =>
               {
                   if (result.Result == null) return;
                   dynamic res = result.Result;
                   mTViewModel.CommitTitle = res.Subject;
                   mTViewModel.TaskID = res.TaskId;
                   mTViewModel.CommitDescription = res.Description;
               });


            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(getLatestMergeCommitTask);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                , new CancellationTokenSource()
                , null
                , name: "Restoring latest merge commit"
                , delayTime: 0
                , reportDelay: 100);

            ProgTroll.Current.ServiceManager?.App.OpenMultiTaskBox("Restoring latest merge commit", multiTask);
        }

    }
}
