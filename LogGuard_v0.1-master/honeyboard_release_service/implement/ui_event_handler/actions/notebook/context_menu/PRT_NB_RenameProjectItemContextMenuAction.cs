using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.view_models.calendar_notebook.items;

namespace honeyboard_release_service.implement.ui_event_handler.actions.notebook.context_menu
{
    internal class PRT_NB_RenameProjectItemContextMenuAction : BaseViewModelCommandExecuter
    {

        public PRT_NB_RenameProjectItemContextMenuAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger) 
            : base(actionID, builderID, viewModel, logger)
        {
           
        }

        protected override void ExecuteCommand()
        {
            if (DataTransfer != null)
            {
                var selectedCNProjectItemVM = DataTransfer[0] as CalendarNotebookProjectItemViewModel;

                if (selectedCNProjectItemVM != null)
                {
                    var oldText = selectedCNProjectItemVM.ProjectName;
                    
                    var newText = HoneyboardReleaseService
                        .Current
                        .ServiceManager?
                        .App
                        .OpenEditTextDialogWindow(oldText);

                    if (newText != null)
                    {
                        selectedCNProjectItemVM.ProjectName = newText;
                    }
                }
            }
        }
    }
}
