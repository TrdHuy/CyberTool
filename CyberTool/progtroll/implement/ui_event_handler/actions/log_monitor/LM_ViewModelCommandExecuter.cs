﻿using cyber_base.utils;
using cyber_base.view_model;
using progtroll.view_models.log_monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.actions.log_monitor
{
    internal class LM_ViewModelCommandExecuter : BaseViewModelCommandExecuter
    {
        protected LogMonitorViewModel LMViewModel
        {
            get
            {
                return (LogMonitorViewModel)ViewModel;
            }
        }

        public LM_ViewModelCommandExecuter(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger)
        {
        }
    }
}