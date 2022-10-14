using cyber_base.utils;
using cyber_base.view_model;
using log_guard.definitions;
using log_guard.implement.flow.state_controller;

namespace log_guard.implement.ui_event_handler.actions.log_watcher.button
{
    internal class MSW_LogWatcher_PlayButtonAction : LG_ViewModelCommandExecuter
    {
        public MSW_LogWatcher_PlayButtonAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();

            if (StateController.Current?.CurrentState == LogGuardState.NONE
                || StateController.Current?.CurrentState == LogGuardState.STOP)
            {
                if (StateController.Current.Start())
                {
                    //LGPViewModel.CurrentLogGuardState = LogGuardState.RUNNING;
                }
            }
            else if (StateController.Current?.CurrentState == LogGuardState.RUNNING)
            {
                //LGPViewModel.CurrentLogGuardState = LogGuardState.PAUSING;
                StateController.Current.Pause();
            }
            else if (StateController.Current?.CurrentState == LogGuardState.PAUSING)
            {
                //LGPViewModel.CurrentLogGuardState = LogGuardState.RUNNING;

                StateController.Current?.Resume();
            }
           
        }


    }
}
