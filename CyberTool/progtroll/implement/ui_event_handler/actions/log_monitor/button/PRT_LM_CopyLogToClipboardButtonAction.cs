using cyber_base.utils;
using cyber_base.view_model;
using progtroll.implement.log_manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace progtroll.implement.ui_event_handler.actions.log_monitor.button
{
    internal class PRT_LM_CopyLogToClipboardButtonAction : LM_ViewModelCommandExecuter
    {
        public PRT_LM_CopyLogToClipboardButtonAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger)
        {
        }

        protected override void ExecuteCommand()
        {
            Clipboard.SetText(LogManager.Current.LogContent);
        }

    }
}