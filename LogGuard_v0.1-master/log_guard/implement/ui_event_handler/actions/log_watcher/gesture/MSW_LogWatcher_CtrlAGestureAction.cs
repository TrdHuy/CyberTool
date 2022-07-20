using cyber_base.utils;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.ui_event_handler.actions.log_watcher.gesture
{
    internal class MSW_LogWatcher_CtrlAGestureAction : LG_ViewModelCommandExecuter
    {
        public MSW_LogWatcher_CtrlAGestureAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger) 
            : base(actionID, builderID, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
        }
    }
}