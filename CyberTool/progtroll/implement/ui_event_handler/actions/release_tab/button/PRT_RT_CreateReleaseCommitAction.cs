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
using progtroll.view_models.project_manager;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.actions.release_tab.button
{
    internal class PRT_RT_CreateReleaseCommitAction : RT_ViewModelCommandExecuter
    {
        private ProjectManagerViewModel PMViewModel;
        private string _branchPath = "";
        private readonly string EMPTY_STRING = "";

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
                ProgTroll.Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("Please import project first!");
                return false;
            }

            if (string.IsNullOrEmpty(RTViewModel.TaskID))
            {
                ProgTroll.Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("You must enter task id");
                return false;
            }

            if (string.IsNullOrEmpty(RTViewModel.CommitTitle))
            {
                ProgTroll.Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("You must enter commit title");
                return false;
            }

            if (ReleasingProjectManager
                    .Current
                    .CurrentImportedProjectVO.OnBranch == null)
            {
                ProgTroll.Current
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
                ProgTroll.Current
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
                ProgTroll.Current
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
                ProgTroll.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("New version must be greater than "
                   + ReleasingProjectManager
                        .Current.LatestCommitVO.Properties.ToString());
                return false;
            }

            var versionPropertiesVO = ReleasingProjectManager
                                        .Current
                                        .VAParsingManager
                                        .GetVersionPropertiesFromVersionFileContent();

            if ((versionPropertiesVO.Major == EMPTY_STRING && RTViewModel.Major != EMPTY_STRING)
                || (versionPropertiesVO.Major != EMPTY_STRING && RTViewModel.Major == EMPTY_STRING))
            {
                ProgTroll.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Please re-check major of project!");

                return false;
            }

            if ((versionPropertiesVO.Minor == EMPTY_STRING && RTViewModel.Minor != EMPTY_STRING)
                || (versionPropertiesVO.Minor != EMPTY_STRING && RTViewModel.Minor == EMPTY_STRING))
            {
                ProgTroll.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Please re-check minor of project!");

                return false;
            }

            if ((versionPropertiesVO.Patch == EMPTY_STRING && RTViewModel.Patch != EMPTY_STRING)
                || (versionPropertiesVO.Patch != EMPTY_STRING && RTViewModel.Patch == EMPTY_STRING))
            {
                ProgTroll.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Please re-check path of project!");

                return false;
            }

            if ((versionPropertiesVO.Revision == EMPTY_STRING && RTViewModel.Revision != EMPTY_STRING) 
                || (versionPropertiesVO.Revision != EMPTY_STRING && RTViewModel.Revision == EMPTY_STRING))
            {
                ProgTroll.Current
                   .ServiceManager?
                   .App
                   .ShowWaringBox("Please re-check revision of project!");

                return false;
            }

            _branchPath = ReleasingProjectManager.Current.CurrentImportedProjectVO.OnBranch.IsRemote
                                ? ReleasingProjectManager.Current.CurrentImportedProjectVO.OnBranch.BranchPath
                                : "origin/" + ReleasingProjectManager.Current.CurrentImportedProjectVO.OnBranch.BranchPath;

            var confirm = ProgTroll
                .Current
                .ServiceManager?
                .App
                .ShowYesNoQuestionBox("Do you want to release for '" + _branchPath + "'?");

            return confirm == CyberContactMessage.Yes;
        }

        protected override void ExecuteCommand()
        {
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

            CancelableAsyncTask modifyVersionTask = new CancelableAsyncTask(
                mainFunc: async (cts, res) =>
                {
                    var _propertiesMap = new Dictionary<string, string>();

                    if (RTViewModel.ModifiedVersionPropVO == null) return res;

                    _propertiesMap.Add("major", RTViewModel.ModifiedVersionPropVO.Major);
                    _propertiesMap.Add("minor", RTViewModel.ModifiedVersionPropVO.Minor);
                    _propertiesMap.Add("patch", RTViewModel.ModifiedVersionPropVO.Patch);
                    _propertiesMap.Add("revision", RTViewModel.ModifiedVersionPropVO.Revision);

                    var content = ReleasingProjectManager
                                .Current
                                .VAParsingManager
                                .ModifyVersionAttributeOfOriginText(_propertiesMap);

                    await File.WriteAllTextAsync(PMViewModel.ProjectPath + "\\" + PMViewModel.VersionPropertiesFileName, content);

                    return res;
                }
                , cancellationTokenSource: new CancellationTokenSource()
                , estimatedTime: 2000
                , delayTime: 0
                , name: "Modifying version properties file");

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

            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(fetchTask);
            tasks.Add(checkoutTask);
            tasks.Add(resetTask);
            tasks.Add(cleanTask);
            tasks.Add(modifyVersionTask);
            tasks.Add(addModifiedFileTask);
            tasks.Add(commitTask);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                , releaseTaskCancelTokenSource
                , null
                , name: "Releasing"
                , delayTime: 0
                , reportDelay: 100);

            ProgTroll.Current.ServiceManager?.App.OpenMultiTaskBox(
                "Releasing"
                , multiTask
                , isCancelable: false
                , multiTaskDoneCallback: (param) =>
                {
                    var waitingBox = param as IStandBox;

                    if (commitTask.IsCompleted)
                    {
                        waitingBox?.UpdateMessageAndTitle("Commit sucess", "Finished");
                        RTViewModel.ReleaseTabGitStatus = ProjectGitStatus.HavingCommit;
                        ProgTroll
                            .Current
                            .ServiceManager?
                            .App
                            .ShowWaringBox("Commit successfully!");
                    }
                    else if (commitTask.IsCanceled || commitTask.IsFaulted)
                    {
                        waitingBox?.UpdateMessageAndTitle("Cancel commit process", "Finished");
                        RTViewModel.ReleaseTabGitStatus = ProjectGitStatus.None;
                    }
                });
        }
    }
}
