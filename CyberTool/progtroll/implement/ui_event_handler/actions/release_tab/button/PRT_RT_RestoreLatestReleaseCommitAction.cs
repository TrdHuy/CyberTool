﻿using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view_model;
using progtroll.implement.project_manager;
using progtroll.implement.ui_event_handler.async_tasks.git_tasks;
using progtroll.implement.view_model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.actions.release_tab.button
{
    internal class PRT_RT_RestoreLatestReleaseCommitAction : RT_ViewModelCommandExecuter
    {
        public PRT_RT_RestoreLatestReleaseCommitAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger)
        {
        }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var projectPath = ViewModelManager
           .Current
           .PMViewModel
           .ProjectPath;
            var versionPropertiesFileName = ViewModelManager
                .Current
                .PMViewModel
                .VersionPropertiesFileName;

            if (string.IsNullOrEmpty(versionPropertiesFileName)
                || string.IsNullOrEmpty(projectPath))
            {
                ProgTroll.Current.ServiceManager?.App.ShowWaringBox("Please import project and version properties file first!");
                return;
            }

            string[] pathsParam = new string[] { projectPath, versionPropertiesFileName };

            BaseAsyncTask getLatestReleaseCommitTask = new GetLatestReleaseCommitTask(pathsParam
               , (result) =>
               {
                   if (result.Result == null) return;
                   dynamic res = result.Result;
                   RTViewModel.CommitTitle = res.Subject;
                   RTViewModel.TaskID = res.TaskId;
               });

            CancelableAsyncTask parseVersionPropFromFileTask = new CancelableAsyncTask(
                mainFunc: async (cts, res) =>
                {
                    await Task.Delay(1);

                    var versionPropertiesVO = ReleasingProjectManager
                                                .Current
                                                .VAParsingManager
                                                .GetVersionPropertiesFromVersionFileContent();

                    if (versionPropertiesVO == null) return res;
                    
                    RTViewModel.Major = versionPropertiesVO.Major;
                    RTViewModel.Minor = versionPropertiesVO.Minor;
                    RTViewModel.Patch = versionPropertiesVO.Patch;
                    RTViewModel.Revision = versionPropertiesVO.Revision;
                    
                    return res;
                }
                , cancellationTokenSource: new CancellationTokenSource()
                , estimatedTime: 2000
                , delayTime: 0
                , name: "Parse version properties from file");

            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(getLatestReleaseCommitTask);
            tasks.Add(parseVersionPropFromFileTask);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                , new CancellationTokenSource()
                , Callback
                , name: "Restoring latest release commit"
                , delayTime: 0
                , reportDelay: 100);

            ProgTroll.Current.ServiceManager?.App.OpenMultiTaskBox("Restoring latest release commit", multiTask);
        }

        private async Task<AsyncTaskResult> Callback(List<AsyncTaskResult> results, AsyncTaskResult result)
        {
            return result;
        }
    }
}
