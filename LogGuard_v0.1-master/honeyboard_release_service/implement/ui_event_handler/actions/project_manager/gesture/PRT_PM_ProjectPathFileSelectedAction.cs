using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.io_tasks;
using honeyboard_release_service.models.VOs;
using honeyboard_release_service.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace honeyboard_release_service.implement.ui_event_handler.actions.project_manager.gesture
{
    internal class PRT_PM_ProjectPathFileSelectedAction : PM_ViewModelCommandExecuter
    {
        public PRT_PM_ProjectPathFileSelectedAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger)
        {
        }

        protected override void ExecuteCommand()
        {
            var branchCache = new CyberTreeViewObservableCollection<ICyberTreeViewItemContext>();

            var param = new object[]
            {
                ReleasingProjectManager
                    .Current
                    .VAParsingManager
                    .GetVersionPropertiesFileName() ?? Array.Empty<string>(),
                ReleasingProjectManager
                    .Current
                    .VAParsingManager
                    .GetVersionPropertiesParserMainSyntax() ?? Array.Empty<string>(),
                PMViewModel.ProjectPath
            };
            BaseAsyncTask findVersionPathTask = new FindVersionPropertiesFileTask(param
                , (result) =>
                {
                    if (result.Result != null)
                    {
                        if (result.MesResult == MessageAsyncTaskResult.Done
                        || result.MesResult == MessageAsyncTaskResult.Finished)
                        {
                            dynamic res = result.Result;
                            var filePath = res.VersionFilePath;
                            var fileSyntax = res.VersionSyntax;
                            var versionFileContent = res.VersionFileContent;
                            ReleasingProjectManager
                                .Current
                                .SetVersionAttrFilePathAndSyntaxOfCurrentImportProject(filePath, fileSyntax, versionFileContent);
                        }
                    }
                });
            BaseAsyncTask listAllBranch = new GetAllProjectBranchsTask(PMViewModel.ProjectPath
                , prepareGetAllProjectBranchs: () =>
                {
                    ReleasingProjectManager
                            .Current
                            .CurrentImportedProjectVO?
                            .Branchs.Clear();
                }
                , callback: (result) =>
                {
                    dynamic? newRes = result.Result;
                    if (newRes != null)
                    {
                        ReleasingProjectManager
                            .Current
                            .SetCurrentProjectBranchContextSource(newRes.ContextSource);
                        var branchs = newRes.Branchs;
                    }
                }
                , readBranchCallback: (sender, task, branch, isOnBranch) =>
                {
                    if (branch != null)
                    {
                        ReleasingProjectManager
                            .Current.AddBranchToCurrentProject(branch.Branch);
                    }

                    if (isOnBranch && branch != null)
                    {
                        PMViewModel.ForceSetSelectedBranch(branch);
                    }
                });

            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(findVersionPathTask);
            tasks.Add(listAllBranch);

            MultiAsyncTask multiTask = new MultiAsyncTask(mainFunc: tasks
                , cancellationTokenSource: new CancellationTokenSource()
                , name: "Importing project"
                , delayTime: 0
                , reportDelay: 100);
            var message = HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox("Importing project", multiTask);

            if (message != CyberContactMessage.Cancel
                && PMViewModel.VersionPropertiesFileName != "")
            {
                ReleasingProjectManager
                    .Current
                    .UpdateVersionHistoryTimelineInBackground();
            }
            else if (message == CyberContactMessage.Cancel)
            {
            }
        }

    }
}
