using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view_model;
using progtroll.implement.project_manager;
using progtroll.implement.ui_event_handler.async_tasks.git_tasks;
using progtroll.implement.ui_event_handler.async_tasks.io_tasks;
using progtroll.implement.view_model;
using progtroll.view_models.project_manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.actions.release_tab.button
{
    internal class PRT_RT_CreateReleaseCommitAction : RT_ViewModelCommandExecuter
    {
        private ProjectManagerViewModel PMViewModel;
        private string _branchPath = "";
        private string _branchPathForPushing = "";

        public PRT_RT_CreateReleaseCommitAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, viewModel, logger)
        {
            PMViewModel = ViewModelManager.Current.PMViewModel;
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

            if (string.IsNullOrEmpty(RTViewModel.TaskID))
            {
                HoneyboardReleaseService.Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("You must enter task id");
                return false;
            }

            if (string.IsNullOrEmpty(RTViewModel.CommitTitle))
            {
                HoneyboardReleaseService.Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("You must enter commit title");
                return false;
            }

            if (string.IsNullOrEmpty(RTViewModel.Major))
            {
                HoneyboardReleaseService.Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("Major property can not be empty!");
                return false;
            }

            if (ReleasingProjectManager
                    .Current
                    .CurrentImportedProjectVO.OnBranch == null)
            {
                HoneyboardReleaseService.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Please select a branch you planning to release new version");
                return false;
            }

            if (!ReleasingProjectManager
                    .Current
                    .CurrentImportedProjectVO.OnBranch.IsRemote
                && !ReleasingProjectManager
                    .Current
                    .CurrentImportedProjectVO
                    .Branchs
                    .ContainsKey("origin/"
                        + ReleasingProjectManager
                            .Current
                            .CurrentImportedProjectVO
                            .OnBranch
                            .BranchPath))
            {
                HoneyboardReleaseService.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Please select an origin branch");
                return false;
            }

            if (ReleasingProjectManager
                    .Current
                    .LatestCommitVO?.Properties == null
                || (ReleasingProjectManager.Current.LatestCommitVO?.Properties != null
                    && (ReleasingProjectManager
                        .Current
                        .LatestCommitVO?
                        .Properties.IsEmpty() ?? false)))
            {
                HoneyboardReleaseService.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Please wait few minutes for loading latest version");
                return false;
            }

            if (ReleasingProjectManager
                    .Current
                    .LatestCommitVO?.Properties != null
                && RTViewModel.ModifiedVersionPropVO
                    <= ReleasingProjectManager
                        .Current.LatestCommitVO.Properties)
            {
                HoneyboardReleaseService.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("New version must be greater than "
                   + ReleasingProjectManager
                        .Current.LatestCommitVO.Properties.ToString());
                return false;
            }

            _branchPath = ReleasingProjectManager
                            .Current
                            .CurrentImportedProjectVO
                            .OnBranch
                            .IsRemote
                ? ReleasingProjectManager
                            .Current.CurrentImportedProjectVO.OnBranch.BranchPath
                : "origin/" + ReleasingProjectManager
                            .Current.CurrentImportedProjectVO.OnBranch.BranchPath;
            _branchPathForPushing = "HEAD:refs/for/" + _branchPath.Substring(7);
            return base.CanExecute(dataTransfer);
        }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var releaseTaskCancelTokenSource = new CancellationTokenSource();

            BaseAsyncTask fetchTask = new CommonGitTask(folderPath: PMViewModel.ProjectPath
                , gitCmd: "git fetch"
                , callback: (result) =>
                {
                    // Append log for user here
                }
                , name: "Fetching"
                , estimatedTime: 4000);

            BaseAsyncTask checkoutTask = new CommonGitTask(folderPath: PMViewModel.ProjectPath
                , gitCmd: "git checkout " + _branchPath
                , callback: (result) =>
                {
                    // Append log for user here
                }
                , name: "Checking out branch: " + _branchPath
                , estimatedTime: 2000);
            BaseAsyncTask resetTask = new CommonGitTask(folderPath: PMViewModel.ProjectPath
               , gitCmd: "git reset --hard " + _branchPath
               , callback: (result) =>
               {
                   // Append log for user here
               }
               , name: "Reseting"
               , estimatedTime: 2000);
            BaseAsyncTask cleanTask = new CommonGitTask(folderPath: PMViewModel.ProjectPath
              , gitCmd: "git clean -f -fd"
              , callback: (result) =>
              {
                  // Append log for user here
              }
              , name: "Cleaning"
              , estimatedTime: 2000);

            BaseAsyncTask modifyVersionTask = new ModifyVersionPropertiesFileTask(
                param: new object[] { PMViewModel.ProjectPath
                    , PMViewModel.VersionPropertiesFileName
                    , RTViewModel.ModifiedVersionPropVO }
                , completedCallback: (result) =>
                {
                    if (result.MesResult == MessageAsyncTaskResult.Aborted
                    || result.MesResult == MessageAsyncTaskResult.Faulted)
                    {
                        releaseTaskCancelTokenSource.Cancel();
                    }
                    // Append log for user here
                });

            BaseAsyncTask addModifiedFileTask = new CommonGitTask(folderPath: PMViewModel.ProjectPath
              , gitCmd: "git add " + PMViewModel.VersionPropertiesFileName
              , callback: (result) =>
              {
                  // Append log for user here
              }
              , name: "Adding \"" + PMViewModel.VersionPropertiesFileName + "\" to commit"
              , estimatedTime: 2000);
            BaseAsyncTask commitTask = new CommonGitTask(folderPath: PMViewModel.ProjectPath
              , gitCmd: "git commit -m \"[" + RTViewModel.TaskID + "]" + RTViewModel.CommitTitle + "\""
              , callback: (result) =>
              {
                  // Append log for user here
              }
              , name: "Commiting \"" + RTViewModel.TaskID + "\""
              , estimatedTime: 5000);
            BaseAsyncTask pushTask = new CommonGitTask(folderPath: PMViewModel.ProjectPath
              , gitCmd: "git push --no-thin origin " + _branchPathForPushing
              , callback: (result) =>
              {
                  // Append log for user here
              }
              , name: "Pushing to Gerrit"
              , estimatedTime: 3000);

            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(fetchTask);
            tasks.Add(checkoutTask);
            tasks.Add(resetTask);
            tasks.Add(cleanTask);
            tasks.Add(modifyVersionTask);
            tasks.Add(addModifiedFileTask);
            tasks.Add(commitTask);
            tasks.Add(pushTask);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                , releaseTaskCancelTokenSource
                , null
                , name: "Releasing"
                , delayTime: 0
                , reportDelay: 100);

            HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox("Releasing", multiTask);
        }
    }
}
