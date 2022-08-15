using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view_model;
using progtroll.definitions;
using progtroll.implement.project_manager;
using progtroll.implement.ui_event_handler.async_tasks.git_tasks;
using progtroll.implement.view_model;
using progtroll.view_models.tab_items;
using System.Collections.Generic;
using System.Threading;

namespace progtroll.implement.ui_event_handler.actions.release_tab.button
{
    internal class PRT_RT_PushReleaseCommitAction : BaseViewModelCommandExecuter
    {
        private ReleaseTabViewModel RTViewModel;

        private string _branchPathForPushing = "";
        private bool _isPushToGerrit = false;

        public PRT_RT_PushReleaseCommitAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger) : base(actionID, builderID, viewModel, logger)
        {
            RTViewModel = ViewModelManager.Current.RTViewModel;
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            if (ReleasingProjectManager
                    .Current
                    .CurrentImportedProjectVO == null)
            {
                HoneyboardReleaseService.Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("Please import project first!");
                return false;
            }

            if (ReleasingProjectManager
                   .Current
                   .CurrentImportedProjectVO.OnBranch == null)
            {
                HoneyboardReleaseService.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Please select a branch you intend to merge into");
                return false;
            }

            var branchPath = ReleasingProjectManager
                                .Current
                                .CurrentImportedProjectVO
                                .OnBranch
                                .IsRemote
            ? ReleasingProjectManager
                        .Current
                        .CurrentImportedProjectVO
                        .OnBranch
                        .BranchPath

            : "origin/" + ReleasingProjectManager
                            .Current
                            .CurrentImportedProjectVO
                            .OnBranch
                            .BranchPath;

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

            return RTViewModel.ReleaseTabGitStatus == ProjectGitStatus.HavingCommit 
                && confirmGerritPush == CyberContactMessage.Yes;
        }

        protected override void ExecuteCommand()
        {
            var pushTaskName = _isPushToGerrit ? "Pushing to Gerrit" : "Pushing to " + _branchPathForPushing;
            var releaseTaskParam = new string[] { ReleasingProjectManager.Current.ProjectPath, _branchPathForPushing};
            var pushResult = PushResult.None;
            var gerritLink = "";
            var errorMes = "";

            BaseAsyncTask pushTask = new GitPushTask(
              param: releaseTaskParam
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

                        RTViewModel.ReleaseTabGitStatus = ProjectGitStatus.None;
                    }
                    else if (pushResult == PushResult.Error)
                    {
                        HoneyboardReleaseService
                            .Current
                            .ServiceManager?
                            .App
                            .ShowWaringBox("Push fail:\n" + errorMes);

                        RTViewModel.ReleaseTabGitStatus = ProjectGitStatus.None;
                    }
                    else if (pushResult == PushResult.UpToDate)
                    {
                        HoneyboardReleaseService
                            .Current
                            .ServiceManager?
                            .App
                            .ShowWaringBox("Everything up-to-date");

                        RTViewModel.ReleaseTabGitStatus = ProjectGitStatus.None;
                    }
                });
        }
    }
}
