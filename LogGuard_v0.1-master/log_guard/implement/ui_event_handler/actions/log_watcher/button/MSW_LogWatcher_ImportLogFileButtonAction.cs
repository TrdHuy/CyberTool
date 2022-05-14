using cyber_base.utils;
using cyber_base.view_model;
using log_guard.definitions;
using log_guard.implement.flow.state_controller;
using log_guard.view_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.ui_event_handler.actions.log_watcher.button
{
    internal class MSW_LogWatcher_ImportLogFileButtonAction : LG_ViewModelCommandExecuter
    {
        public MSW_LogWatcher_ImportLogFileButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            LGPViewModel.SelectParserOption(LogParserOption.DUMPSTATE_PARSER);

            var path = LogGuardService
                .Current?
                .ServiceManager
                .App
                .OpenFileChooserDialogWindow();
            if (!string.IsNullOrEmpty(path))
            {
                StateController.Current?.StartImportLogFile(path);
            }
        }


    }
}
