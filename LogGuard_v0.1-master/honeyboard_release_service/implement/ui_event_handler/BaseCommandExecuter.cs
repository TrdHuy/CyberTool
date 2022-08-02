using cyber_base.ui_event_handler.action.executer;
using cyber_base.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler
{
    internal class BaseCommandExecuter : AbstractCommandExecuter
    {
        public BaseCommandExecuter(string actionID, string builderID, ILogger? logger) : base(actionID, builderID, logger)
        {
        }

        public BaseCommandExecuter(string actionName, string actionID, string builderID, ILogger? logger) : base(actionName, actionID, builderID, logger)
        {
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            return true;
        }

        protected override void ExecuteAlternativeCommand()
        {
        }

        protected override void ExecuteCommand()
        {
        }

        protected override void ExecuteOnCancel()
        {
        }

        protected override void ExecuteOnDestroy()
        {
        }

        protected override void SetCompleteFlagAfterExecuteCommand()
        {
            IsCompleted = true;
        }
    }
}
