using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.implement.log_manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.actions.log_monitor.button
{
    internal class PRT_LM_ClearLogContentButtonAction : LM_ViewModelCommandExecuter
    {
        public PRT_LM_ClearLogContentButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
           : base(actionID, builderID, viewModel, logger)
        {
        }

        protected override void ExecuteCommand()
        {
            LogManager.Current.ClearLog();
        }

    }
}
