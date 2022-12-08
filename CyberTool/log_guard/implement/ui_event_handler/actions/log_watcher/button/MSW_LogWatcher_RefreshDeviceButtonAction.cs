using cyber_base.utils;
using cyber_base.view_model;
using log_guard.implement.device;

namespace log_guard.implement.ui_event_handler.actions.log_watcher.button
{
    internal class MSW_LogWatcher_RefreshDeviceButtonAction : LG_ViewModelCommandExecuter
    {

        public MSW_LogWatcher_RefreshDeviceButtonAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            DeviceManager.Current.UpdateListDevices();
        }
    }
}
