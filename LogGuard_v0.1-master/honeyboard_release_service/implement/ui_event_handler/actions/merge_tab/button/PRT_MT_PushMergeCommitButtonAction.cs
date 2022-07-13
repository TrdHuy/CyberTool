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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.actions.merge_tab.button
{
    internal class PRT_MT_PushMergeCommitButtonAction : BaseViewModelCommandExecuter
    {
        private string _branchPathForPushing = "";
        private bool _isPushToGerrit = false;
        public PRT_MT_PushMergeCommitButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
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

            if (ReleasingProjectManager
                   .Current
                   .CurrentProjectVO.OnBranch == null)
            {
                HoneyboardReleaseService.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Please select a branch you intend to merge into");
                return false;
            }

            var branchPath = ReleasingProjectManager
                                .Current
                                .CurrentProjectVO
                                .OnBranch
                                .IsRemote
                ? ReleasingProjectManager
                            .Current.CurrentProjectVO.OnBranch.BranchPath
                : "origin/" + ReleasingProjectManager
                            .Current.CurrentProjectVO.OnBranch.BranchPath;
            _branchPathForPushing = "HEAD:" + branchPath.Substring(7);

            var confirmGerritPush = HoneyboardReleaseService
                .Current
                .ServiceManager?
                .App
                .ShowYesNoQuestionBox("Push to Gerrit?");

            if (confirmGerritPush == CyberContactMessage.Yes)
            {
                _branchPathForPushing = "HEAD:refs/for/" + branchPath.Substring(7);
                _isPushToGerrit = true;
            }

            return mTViewModel.MergeTabGitStatus
                == ProjectGitStatus.HavingCommit;
        }

        protected override void ExecuteCommand()
        {
            var mtViewModel = ViewModelManager.Current.MTViewModel;
            var pushTaskName = _isPushToGerrit ? "Pushing to Gerrit" : "Pushing to " + _branchPathForPushing;
            var mergeTaskParam = new string[] { ReleasingProjectManager.Current.ProjectPath
                , _branchPathForPushing};

            var pushResult = PushResult.None;
            var gerritLink = "";
            var errorMes = "";

            BaseAsyncTask pushTask = new GitPushTask(
              param: mergeTaskParam
              , completedCallback: (result) =>
              {
                  if (result.Result != null)
                  {
                      dynamic pushTaskRes = result.Result;
                      pushResult = pushTaskRes.PushResult;
                      errorMes = pushTaskRes.ErrorMessage;
                      gerritLink = pushTaskRes.GerritLink;
                  }
              }
              , name: pushTaskName);
            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(pushTask);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
               , new CancellationTokenSource()
               , null
               , name: pushTaskName
               , delayTime: 0
               , reportDelay: 100);

            HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox(
                title: pushTaskName
                , task: multiTask
                , isCancelable: false
                , multiTaskDoneCallback: (param) =>
                {
                    if (pushResult == PushResult.Success)
                    {
                        if (_isPushToGerrit)
                        {
                            HoneyboardReleaseService
                            .Current
                            .ServiceManager?
                            .App
                            .ShowWaringBox("Push successfully:\n" + gerritLink);
                        }
                        else
                        {
                            HoneyboardReleaseService
                            .Current
                            .ServiceManager?
                            .App
                            .ShowWaringBox("Push successfully");
                        }
                        mtViewModel.MergeTabGitStatus = ProjectGitStatus.None;
                    }
                    else if (pushResult == PushResult.Error)
                    {
                        HoneyboardReleaseService
                            .Current
                            .ServiceManager?
                            .App
                            .ShowWaringBox("Push fail:\n" + errorMes);
                        mtViewModel.MergeTabGitStatus = ProjectGitStatus.None;
                    }
                    else if (pushResult == PushResult.UpToDate)
                    {
                        HoneyboardReleaseService
                            .Current
                            .ServiceManager?
                            .App
                            .ShowWaringBox("Everything up-to-date");
                        mtViewModel.MergeTabGitStatus = ProjectGitStatus.None;
                    }
                });
        }
    }
}
