﻿using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.io_tasks;
using honeyboard_release_service.implement.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.actions.release_tab.button
{
    internal class PRT_RT_RestoreLatestReleaseCommitAction : RT_ViewModelCommandExecuter
    {
        public PRT_RT_RestoreLatestReleaseCommitAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger)
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
                .VersionPropertiesPath;

            if (string.IsNullOrEmpty(versionPropertiesFileName)
                || string.IsNullOrEmpty(projectPath))
            {
                HoneyboardReleaseService.Current.ServiceManager?.App.ShowWaringBox("Please import project and version properties file first!");
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
            BaseAsyncTask parseVersionPropFromFile = new ParseVersionPropertiesFromFile(pathsParam
               , (result) =>
               {
                   var dict = result.Result as Dictionary<string, string>;
                   if (dict == null) return;

                   if (dict.ContainsKey("major"))
                   {
                       RTViewModel.Major = dict["major"];
                   }

                   if (dict.ContainsKey("minor"))
                   {
                       RTViewModel.Minor = dict["minor"];
                   }

                   if (dict.ContainsKey("patch"))
                   {
                       RTViewModel.Patch = dict["patch"];
                   }

                   if (dict.ContainsKey("revision"))
                   {
                       RTViewModel.Revision = dict["revision"];
                   }
               });


            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(getLatestReleaseCommitTask);
            tasks.Add(parseVersionPropFromFile);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                , new CancellationTokenSource()
                , Callback
                , name: "Restoring latest release commit"
                , delayTime: 0
                , reportDelay: 100);

            HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox("Restoring latest release commit", multiTask);
        }

        private async Task<AsyncTaskResult> Callback(List<AsyncTaskResult> results, AsyncTaskResult result)
        {
            return result;
        }
    }
}
