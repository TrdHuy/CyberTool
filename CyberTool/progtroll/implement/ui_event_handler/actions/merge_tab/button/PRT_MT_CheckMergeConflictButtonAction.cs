using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view.window;
using cyber_base.view_model;
using progtroll.definitions;
using progtroll.implement.project_manager;
using progtroll.implement.ui_event_handler.async_tasks.git_tasks;
using progtroll.implement.view_model;
using progtroll.models.VOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.actions.merge_tab.button
{
    internal class PRT_MT_CheckMergeConflictButtonAction : BaseViewModelCommandExecuter
    {
        public PRT_MT_CheckMergeConflictButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, viewModel, logger)
        {
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            var mTViewModel = ViewModelManager.Current.MTViewModel;
            return mTViewModel.MergeTabGitStatus
                == ProjectGitStatus.HavingUnmergeFile;
        }

        protected override void ExecuteCommand()
        {
            var mTViewModel = ViewModelManager.Current.MTViewModel;
            var abortMergeProcess = CyberContactMessage.None;

            BaseAsyncTask checkConflictTask = new CheckMergeConflictTask(
                param: ReleasingProjectManager.Current.ProjectPath
              , completedCallback: (result) =>
              {
                  if (result.MesResult != MessageAsyncTaskResult.DoneWithoutExecuted
                     && result.Result != null)
                  {
                      dynamic res = result.Result;
                      var conflictCommitsList = res.MergeConflictCommits as List<CommitVO>;
                      var conflictFilesList = res.MergeConflictFiles as List<string>;

                      var message = "";
                      if (conflictCommitsList != null)
                      {
                          message += "#Conflict commit(s):\n";
                          foreach (var conflictCommit in conflictCommitsList)
                          {
                              message += conflictCommit.CommitId + " " + conflictCommit.CommitTitle + "\n";
                          }
                      }

                      if (conflictFilesList != null)
                      {
                          message += "#Conflict file(s):\n";
                          foreach (var conflictFile in conflictFilesList)
                          {
                              message += conflictFile + "\n";
                          }
                      }

                      message += "Do you want to abort merge process?";
                      abortMergeProcess = ProgTroll
                        .Current
                        .ServiceManager?
                        .App
                        .ShowYesNoQuestionBox(message) ?? CyberContactMessage.None;

                  }

              });

            BaseAsyncTask abortMergeTask = new CommonGitTask(
                folderPath: ReleasingProjectManager.Current.ProjectPath
              , gitCmd: "git merge --abort"
              , callback: (result) =>
              {
                  // Append log for user here
              }
              , canExecute: (param) =>
              {
                  return abortMergeProcess == CyberContactMessage.Yes;
              }
              , name: "Aborting"
              , estimatedTime: 2000);

            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(checkConflictTask);
            tasks.Add(abortMergeTask);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
               , new CancellationTokenSource()
               , null
               , name: "Checking conflict"
               , delayTime: 0
               , reportDelay: 100);

            ProgTroll.Current.ServiceManager?.App.OpenMultiTaskBox(
                title: "Checking conflict"
                , task: multiTask
                , isCancelable: false
                , isUseMultiTaskReport: false
                , multiTaskDoneCallback: (param) =>
                {
                    var waitingBox = param as IStandBox;

                    if (abortMergeProcess == CyberContactMessage.Yes)
                    {
                        waitingBox?.UpdateMessageAndTitle("Aborted merge process", "Finished");
                        mTViewModel.MergeTabGitStatus = ProjectGitStatus.None;
                    }
                });
        }
    }
}
