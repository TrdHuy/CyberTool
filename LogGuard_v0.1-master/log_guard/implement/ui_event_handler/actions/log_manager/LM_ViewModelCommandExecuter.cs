using cyber_base.utils;
using cyber_base.view_model;
using log_guard.view_models.log_manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.ui_event_handler.actions.log_manager
{
    internal class LM_ViewModelCommandExecuter : BaseViewModelCommandExecuter
    {
        protected LogManagerUCViewModel LMUCViewModel
        {
            get
            {
                return ViewModel as LogManagerUCViewModel;
            }
        }

        public LM_ViewModelCommandExecuter(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger)
        {
        }
    }
}
