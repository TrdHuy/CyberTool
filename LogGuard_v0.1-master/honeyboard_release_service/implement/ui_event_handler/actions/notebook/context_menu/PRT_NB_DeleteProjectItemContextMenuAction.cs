using cyber_base.definition;
using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.implement.view_model;
using honeyboard_release_service.models.VOs;
using honeyboard_release_service.view_models.calendar_notebook.items;
using System.Windows;

namespace honeyboard_release_service.implement.ui_event_handler.actions.notebook.context_menu
{
    internal class PRT_NB_DeleteProjectItemContextMenuAction : BaseViewModelCommandExecuter
    {
        private ReleasingProjectManager releasingProjectManager;
        private ViewModelManager viewModelManager;

        public PRT_NB_DeleteProjectItemContextMenuAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
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
                .ShowYesNoQuestionBox("Do you want to delete this project?");
            return confirm == CyberContactMessage.Yes;
        }

        protected override void ExecuteCommand()
        {
            if(DataTransfer != null)
            {
                var selectedCNProjectItemVM = DataTransfer[0] as CalendarNotebookProjectItemViewModel;
                var selectedProjectItem = selectedCNProjectItemVM?.SelectedProjectItem;
                var notebookItemContexts = viewModelManager.CNViewModel.NotebookItemContexts;
                var notebookItemContextsMap = viewModelManager.CNViewModel.NotebookItemContextsMap;
                var importedProjectMap = releasingProjectManager.ImportedProjects;

                if (selectedProjectItem == releasingProjectManager.CurrentImportedProjectVO)
                {
                    viewModelManager.PMViewModel.ProjectPath = "";
                    releasingProjectManager.VersionHistoryItemContexts.Clear();
                    viewModelManager.PMViewModel.RefreshViewModel();
                }
                if (selectedProjectItem != null)
                {
                    var context = notebookItemContextsMap[selectedProjectItem.Path];
                    importedProjectMap.Remove(selectedProjectItem.Path);
                    notebookItemContexts.Remove(context);
                    notebookItemContextsMap.Remove(selectedProjectItem.Path);
                }
            }

        }
    }
}

