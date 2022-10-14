using cyber_base.utils;
using cyber_base.view_model;
using log_guard.view_models.log_manager;

namespace log_guard.implement.ui_event_handler.actions.log_manager.button
{
    internal class MSW_LMUC_DeleteMessageItemAction : LM_ViewModelCommandExecuter
    {
        public MSW_LMUC_DeleteMessageItemAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var item = DataTransfer[0] as TrippleToggleItemViewModel;
            if (item != null)
            {
                LMUCViewModel.MessageManagerContent.Messagetems.Remove(item);
            }
        }
    }
}