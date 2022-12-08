using cyber_base.utils;
using cyber_base.view_model;
using progtroll.view_models.project_manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.actions.project_manager
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

        public PM_ViewModelCommandExecuter(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger)
        {
        }
    }
}

