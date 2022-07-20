using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.view_models.project_manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.actions.project_manager
{
    internal class PM_ViewModelCommandExecuter : BaseViewModelCommandExecuter
    {
        protected ProjectManagerViewModel PMViewModel
        {
            get
            {
                return (ProjectManagerViewModel)ViewModel;
            }
        }

        public PM_ViewModelCommandExecuter(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger) : base(actionID, builderID, viewModel, logger)
        {
        }
    }
}

