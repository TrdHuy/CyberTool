using cyber_base.utils;
using cyber_base.view_model;
using log_guard.view_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.ui_event_handler.actions.log_watcher
{
    internal class LG_ViewModelCommandExecuter : BaseViewModelCommandExecuter
    {
        protected LogGuardViewModel LGPViewModel
        {
            get
            {
                return ViewModel as LogGuardViewModel;
            }
        }

        public LG_ViewModelCommandExecuter(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger) { }
    }
}
