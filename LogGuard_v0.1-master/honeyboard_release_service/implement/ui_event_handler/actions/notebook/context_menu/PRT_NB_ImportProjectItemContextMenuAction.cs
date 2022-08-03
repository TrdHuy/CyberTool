using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.utils;
using honeyboard_release_service.definitions;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using honeyboard_release_service.implement.view_model;
using honeyboard_release_service.view_models.calendar_notebook.items;
using System.Collections.Generic;
using System.Threading;

namespace honeyboard_release_service.implement.ui_event_handler.actions.notebook.context_menu
{
    internal class PRT_NB_ImportProjectItemContextMenuAction : BaseCommandExecuter
    {
        private ReleasingProjectManager releasingProjectManager;
        private ViewModelManager viewModelManager;
        private HoneyboardReleaseService honeyboardReleaseService;

        public PRT_NB_ImportProjectItemContextMenuAction(string actionID, string builderID, ILogger? logger)
            : base(actionID, builderID, logger)
        {
            releasingProjectManager = ReleasingProjectManager.Current;
            viewModelManager = ViewModelManager.Current;
            honeyboardReleaseService = HoneyboardReleaseService.Current;
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            var confirm = honeyboardReleaseService
                .ServiceManager?
                .App
                .ShowYesNoQuestionBox("Do you want to import this project?");

            return confirm == CyberContactMessage.Yes;
        }

        protected override void ExecuteCommand()
        {
            if (DataTransfer != null)
            {
                var selectedCNProjectItemVM = DataTransfer[0] as CalendarNotebookProjectItemViewModel;
                var selectedProjectItem = selectedCNProjectItemVM?.SelectedProjectItem;

                var currentImportProject = releasingProjectManager.CurrentImportedProjectVO;
                var pMViewModel = viewModelManager.PMViewModel;

                if (selectedProjectItem == currentImportProject)
                {
                    honeyboardReleaseService
                        .ServiceManager?
                        .App
                        .ShowWaringBox("You've already imported this project!");

                    return;
                }

                if (selectedProjectItem != null)
                {
                    releasingProjectManager.SetCurrentImportedProject(selectedProjectItem);

                    pMViewModel.RefreshViewModel();

                    BaseAsyncTask listAllBranch = new GetAllProjectBranchsTask(pMViewModel.ProjectPath
                        , prepareGetAllProjectBranchs: () =>
                        {
                            releasingProjectManager
                                .CurrentImportedProjectVO?
                                .Branchs.Clear();
                        }
                        , callback: (result) =>
                        {
                            dynamic? newRes = result.Result;
                            if (newRes != null)
                            {
                                releasingProjectManager
                                    .SetCurrentProjectBranchContextSource(newRes.ContextSource);

                                var branchs = newRes.Branchs;
                            }
                        }
                        , readBranchCallback: (sender, task, branch, isOnBranch) =>
                        {
                            if (branch != null)
                            {
                                releasingProjectManager
                                    .AddBranchToCurrentProject(branch.Branch);
                            }

                            if (isOnBranch && branch != null)
                            {
                                pMViewModel.ForceSetSelectedBranch(branch);
                            }
                        });

                    List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
                    tasks.Add(listAllBranch);

                    MultiAsyncTask multiTask = new MultiAsyncTask(mainFunc: tasks
                        , cancellationTokenSource: new CancellationTokenSource()
                        , name: "Importing project"
                        , delayTime: 0
                        , reportDelay: 100);

                    var message = honeyboardReleaseService
                                    .ServiceManager?
                                    .App
                                    .OpenMultiTaskBox("Importing project", multiTask);

                    if (message != CyberContactMessage.Cancel
                        && pMViewModel.VersionPropertiesPath != "")
                    {
                        releasingProjectManager
                            .UpdateVersionHistoryTimelineInBackground(updateLevel: Level.Normal);
                    }
                    else if (message == CyberContactMessage.Cancel)
                    {
                    }
                }
            }
        }
    }
}
