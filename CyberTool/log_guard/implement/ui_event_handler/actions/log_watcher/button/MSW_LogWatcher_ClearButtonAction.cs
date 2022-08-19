using cyber_base.utils;
using cyber_base.view_model;
using log_guard.implement.flow.source_manager;
using log_guard.view_models;

namespace log_guard.implement.ui_event_handler.actions.log_watcher.button
{
    internal class MSW_LogWatcher_ClearButtonAction : LG_ViewModelCommandExecuter
    {
        public MSW_LogWatcher_ClearButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger) : base(actionID, builderID, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            SourceManager.Current.ClearSource();
        }
    }
}