using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.io_tasks;
using honeyboard_release_service.implement.user_data_manager;
using honeyboard_release_service.implement.view_model;
using honeyboard_release_service.view_models.calendar_notebook.items;
using System.Collections.Generic;
using System.Threading;

namespace honeyboard_release_service.implement.ui_event_handler.actions.notebook.context_menu
{
    internal class PRT_NB_ImportProjectItemContextMenuAction : BaseViewModelCommandExecuter
    {
        private ReleasingProjectManager releasingProjectManager;
        private ViewModelManager viewModelManager;

        public PRT_NB_ImportProjectItemContextMenuAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, viewModel, logger)
        {
            releasingProjectManager = ReleasingProjectManager.Current;
            viewModelManager = ViewModelManager.Current;
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            var confirm = HoneyboardReleaseService
                .Current
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

                var currentImportProject = releasingProjectManager.CurrentProjectVO;
                var pMViewModel = viewModelManager.PMViewModel;

                if (selectedProjectItem == currentImportProject)
                {
                    HoneyboardReleaseService.Current
                        .ServiceManager?
                        .App
                        .ShowWaringBox("You've already imported this project!");

                    return;
                }

                if (selectedProjectItem != null)
                {
                    pMViewModel.ProjectPath = selectedProjectItem.Path;
                    pMViewModel.VersionPropertiesPath = selectedProjectItem.VersionFilePath;

                    BaseAsyncTask listAllBranch = new GetAllProjectBranchsTask(pMViewModel.ProjectPath
                        ,prepareGetAllProjectBranchs: () =>
                        {
                            ReleasingProjectManager
                                    .Current
                                    .CurrentProjectVO?
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
                                    .Current.AddProjectBranch(branch.Branch);
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

                    var message = HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox("Importing project", multiTask);

                    if (message != CyberContactMessage.Cancel
                        && pMViewModel.VersionPropertiesPath != "")
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
    }
}
