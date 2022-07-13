using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view.window;
using cyber_base.view_model;
using honeyboard_release_service.definitions;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using honeyboard_release_service.implement.view_model;
using honeyboard_release_service.models.VOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.actions.merge_tab.button
{
    internal class PRT_MT_CreateMergeCommitButtonAction : BaseViewModelCommandExecuter
    {
        private string _destinationBranchPath = "";
        private string _inceptionBranchPath = "";
        private string _branchPathForPushing = "";

        public PRT_MT_CreateMergeCommitButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, viewModel, logger)
        {
        }

        protected override bool CanExecute(object? dataTransfer)
        {

            var mTViewModel = ViewModelManager.Current.MTViewModel;

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

            if (string.IsNullOrEmpty(mTViewModel.TaskID))
            {
                HoneyboardReleaseService.Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("You must enter task id");
                return false;
            }

            if (string.IsNullOrEmpty(mTViewModel.CommitTitle))
            {
                HoneyboardReleaseService.Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("You must enter commit title");
                return false;
            }

            if (string.IsNullOrEmpty(mTViewModel
                    .SelectedInceptionBranch))
            {
                HoneyboardReleaseService.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Please select a branch you intend to merge");
                return false;
            }

            var inceptionBranchVO = ReleasingProjectManager
                    .Current
                    .GetBranchOfCurrentProjectFromPath(mTViewModel.SelectedInceptionBranch);
            var destinationBranchVO = ReleasingProjectManager
                    .Current
                    .CurrentProjectVO.OnBranch;

            if (destinationBranchVO == null)
            {
                HoneyboardReleaseService.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Please select a branch you intend to merge into");
                return false;
            }

            if (inceptionBranchVO == null)
            {
                HoneyboardReleaseService.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Branch " + mTViewModel.SelectedInceptionBranch + " not found!");
                return false;
            }

            _inceptionBranchPath = inceptionBranchVO
                .IsRemote ? inceptionBranchVO.BranchPath
                : "origin/" + inceptionBranchVO.BranchPath;

            _destinationBranchPath = destinationBranchVO
                .IsRemote ? destinationBranchVO.BranchPath
                : "origin/" + destinationBranchVO.BranchPath;
            _branchPathForPushing = "HEAD:refs/for/" + _destinationBranchPath.Substring(7);

            if (ReleasingProjectManager
                .Current
                .GetBranchOfCurrentProjectFromPath(_inceptionBranchPath) == null)
            {
                HoneyboardReleaseService.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Branch " + _inceptionBranchPath + " not found!");
                return false;
            }

            if (ReleasingProjectManager
                .Current
                .GetBranchOfCurrentProjectFromPath(_destinationBranchPath) == null)
            {
                HoneyboardReleaseService.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Branch " + _destinationBranchPath + " not found!");
                return false;
            }

            var confirm = HoneyboardReleaseService
                .Current
                .ServiceManager?
                .App
                .ShowYesNoQuestionBox("Do you want to merge '" + _inceptionBranchPath + "' into '" + _destinationBranchPath + "'?");
            return confirm == CyberContactMessage.Yes;
        }

        protected override void ExecuteCommand()
        {
            var mTViewModel = ViewModelManager.Current.MTViewModel;

            BaseAsyncTask fetchTask = new CommonGitTask(
                folderPath: ReleasingProjectManager.Current.ProjectPath
                , gitCmd: "git fetch"
                , callback: (result) =>
                {
                    // Append log for user here
                }
                , name: "Fetching"
                , estimatedTime: 4000);

            BaseAsyncTask checkoutTask = new CommonGitTask(
                folderPath: ReleasingProjectManager.Current.ProjectPath
                , gitCmd: "git checkout " + _destinationBranchPath
                , callback: (result) =>
                {
                    // Append log for user here
                }
                , name: "Checking out branch: " + _destinationBranchPath
                , estimatedTime: 2000);
            BaseAsyncTask resetTask = new CommonGitTask(
                folderPath: ReleasingProjectManager.Current.ProjectPath
               , gitCmd: "git reset --hard " + _destinationBranchPath
               , callback: (result) =>
               {
                   // Append log for user here
               }
               , name: "Reseting"
               , estimatedTime: 2000);
            BaseAsyncTask cleanTask = new CommonGitTask(
                folderPath: ReleasingProjectManager.Current.ProjectPath
              , gitCmd: "git clean -f -fd"
              , callback: (result) =>
              {
                  // Append log for user here
              }
              , name: "Cleaning"
              , estimatedTime: 2000);

            var mergeTaskParam = new string[] { ReleasingProjectManager.Current.ProjectPath
                , _inceptionBranchPath
                , "\"[" + mTViewModel.TaskID + "]" + mTViewModel.CommitTitle + "\""};
            BaseAsyncTask mergeTask = new MergeBranchTask(
                param: mergeTaskParam
              , completedCallback: (result) =>
              {
                  // Append log for user here
              }
              , name: "Merging " + _inceptionBranchPath + " into " + _destinationBranchPath);

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
                      abortMergeProcess = HoneyboardReleaseService
                        .Current
                        .ServiceManager?
                        .App
                        .ShowYesNoQuestionBox(message) ?? CyberContactMessage.None;

                  }

              }
              , canExecute: (param) =>
              {
                  if (mergeTask.IsCompleted && mergeTask.Result.Result != null)
                  {
                      dynamic mergeTaskRes = mergeTask.Result.Result;
                      var mergeResult = mergeTaskRes?.MergeResult;
                      var outputCache = mergeTaskRes?.OutputCache;
                      return mergeResult == MergeResult.Conflict;
                  }
                  return false;
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
            tasks.Add(fetchTask);
            tasks.Add(checkoutTask);
            tasks.Add(resetTask);
            tasks.Add(cleanTask);
            tasks.Add(mergeTask);
            tasks.Add(checkConflictTask);
            tasks.Add(abortMergeTask);

            var taskName = "Merging '" + _inceptionBranchPath + "' into '" + _destinationBranchPath + "'";
            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                , new CancellationTokenSource()
                , null
                , name: taskName
                , delayTime: 0
                , reportDelay: 100);
            HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox(
                title: taskName
                , task: multiTask
                , isCancelable: false
                , multiTaskDoneCallback: (param) =>
                {
                    var waitingBox = param as IStandBox;

                    if (abortMergeProcess == CyberContactMessage.Yes)
                    {
                        waitingBox?.UpdateMessageAndTitle("Aborted merge process", "Finished");
                        mTViewModel.MergeTabGitStatus = ProjectGitStatus.None;
                    }
                    else if (mergeTask.IsCompleted && mergeTask.Result.Result != null)
                    {
                        dynamic mergeTaskRes = mergeTask.Result.Result;
                        var mergeResult = mergeTaskRes?.MergeResult;
                        if (mergeResult != null && mergeResult == MergeResult.Success)
                        {
                            waitingBox?.UpdateMessageAndTitle("Merge sucess", "Finished");
                            mTViewModel.MergeTabGitStatus = ProjectGitStatus.HavingCommit;
                            HoneyboardReleaseService
                                .Current
                                .ServiceManager?
                                .App
                                .ShowWaringBox("Merge successfully!");
                        }
                        else if (mergeResult != null && mergeResult == MergeResult.UpToDate)
                        {
                            waitingBox?.UpdateMessageAndTitle("Already up-to-date.", "Finished");
                            mTViewModel.MergeTabGitStatus = ProjectGitStatus.None;
                            HoneyboardReleaseService
                                .Current
                                .ServiceManager?
                                .App
                                .ShowWaringBox("Already up-to-date!");
                        }
                        else if (mergeResult != null && mergeResult == MergeResult.Error)
                        {
                            waitingBox?.UpdateMessageAndTitle("Merge error", "Finished");
                            mTViewModel.MergeTabGitStatus = ProjectGitStatus.None;
                        }
                        else if (mergeResult != null && mergeResult == MergeResult.Conflict)
                        {
                            waitingBox?.UpdateMessageAndTitle("Merge conflict", "Finished");
                            mTViewModel.MergeTabGitStatus = ProjectGitStatus.HavingUnmergeFile;
                        }
                    }
                });
        }
    }
}
