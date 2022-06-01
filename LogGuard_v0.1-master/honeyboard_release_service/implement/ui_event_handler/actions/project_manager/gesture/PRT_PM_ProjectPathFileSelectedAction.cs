using cyber_base.utils;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.actions.project_manager.gesture
{
    internal class PRT_PM_ProjectPathFileSelectedAction : PM_ViewModelCommandExecuter
    {
        public PRT_PM_ProjectPathFileSelectedAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger)
        {
        }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();

        }
    }
}
