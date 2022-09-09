using cyber_base.utils;
using cyber_base.view_model;
using progtroll.view_models.calendar_notebook.items;

namespace progtroll.implement.ui_event_handler.actions.notebook.context_menu
{
    internal class PRT_NB_RenameProjectItemContextMenuAction : BaseCommandExecuter
    {

        public PRT_NB_RenameProjectItemContextMenuAction(string actionID, string builderID, ILogger? logger)
            : base(actionID, builderID, logger)
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

                    var newText = ProgTroll
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
