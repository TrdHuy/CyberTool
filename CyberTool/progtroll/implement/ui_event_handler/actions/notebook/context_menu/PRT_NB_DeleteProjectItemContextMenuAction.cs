using cyber_base.definition;
using cyber_base.utils;
using cyber_base.view_model;
using progtroll.implement.project_manager;
using progtroll.implement.view_model;
using progtroll.models.VOs;
using progtroll.view_models.calendar_notebook.items;
using System;
using System.Collections.Generic;
using System.Windows;

namespace progtroll.implement.ui_event_handler.actions.notebook.context_menu
{
    internal class PRT_NB_DeleteProjectItemContextMenuAction : BaseCommandExecuter
    {
        private ReleasingProjectManager releasingProjectManager;
        private ViewModelManager viewModelManager;

        public PRT_NB_DeleteProjectItemContextMenuAction(string actionID, string builderID, object? dataTransfer, ILogger? logger)
            : base(actionID, builderID, dataTransfer, logger)
        {
            releasingProjectManager = ReleasingProjectManager.Current;
            viewModelManager = ViewModelManager.Current;
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            var confirm = ProgTroll
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
                    releasingProjectManager.SetCurrentImportedProject(null);
                    releasingProjectManager.VersionHistoryItemContexts.Clear();
                    viewModelManager.PMViewModel.TaskIdCommitListSource = new List<String>();
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

