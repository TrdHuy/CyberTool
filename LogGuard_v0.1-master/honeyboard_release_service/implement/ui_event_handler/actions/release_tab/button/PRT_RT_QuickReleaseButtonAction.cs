using cyber_base.utils;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.actions.release_tab.button
{
    internal class PRT_RT_QuickReleaseButtonAction : RT_ViewModelCommandExecuter
    {
        public PRT_RT_QuickReleaseButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger)
        {

        }
    }
}
