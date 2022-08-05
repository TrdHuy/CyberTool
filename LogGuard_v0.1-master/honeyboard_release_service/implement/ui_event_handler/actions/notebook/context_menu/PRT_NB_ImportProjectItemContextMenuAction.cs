using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.utils;
using honeyboard_release_service.definitions;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.implement.view_model;
using honeyboard_release_service.view_models.calendar_notebook.items;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
                    var importProject = new CancelableAsyncTask(
                    mainFunc: async (ts, res) =>
                        {
                            await Task.Delay(100);
                            releasingProjectManager.SetCurrentImportedProject(selectedProjectItem);
                            pMViewModel.RefreshViewModel();
                            releasingProjectManager.UpdateVersionHistoryTimelineInBackground(updateLevel: Level.Normal);
                            return res;
                        }
                    , estimatedTime: 2000
                    , delayTime: 2000
                    , cancellationTokenSource: new CancellationTokenSource()
                    , name: "Importing project");

                    List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
                    tasks.Add(importProject);

                    MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                       , new CancellationTokenSource()
                       , null
                       , name: "Importing project"
                       , delayTime: 0
                       , reportDelay: 10);
                    HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox(
                        title: "Importing project"
                        , task: multiTask
                        , isCancelable: false);
                }
            }
        }
    }
}

