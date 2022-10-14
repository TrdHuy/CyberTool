using cyber_base.utils;
using cyber_base.view_model;
using log_guard.view_models.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.ui_event_handler.actions.log_watcher.gesture
{
    internal class MSW_LPI_LeftMouseClick : LG_ViewModelCommandExecuter
    {
        public MSW_LPI_LeftMouseClick(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var vm = DataTransfer[0] as LogParserItemViewModel;
            if (vm != null)
            {
               
            }
        }
    }
}