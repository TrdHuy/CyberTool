using cyber_base.ui_event_handler.action.executer;
using cyber_base.utils;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.implement.ui_event_handler
{
    internal class BaseViewModelCommandExecuter : AbstractViewModelCommandExecuter
    {
        public BaseViewModelCommandExecuter(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger)
        {
        }
        public BaseViewModelCommandExecuter(string actionName, string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionName, actionID, builderID, dataTransfer, viewModel, logger)
        {
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            return true;
        }

        protected override void ExecuteCommand()
        {
        }

        protected override void ExecuteAlternativeCommand()
        {
        }

        protected override void SetCompleteFlagAfterExecuteCommand()
        {
            IsCompleted = true;
        }

        protected override void ExecuteOnDestroy()
        {
        }

        protected override void ExecuteOnCancel()
        {
        }

        protected override Task ExecuteCommandAsync()
        {
            throw new NotImplementedException();
        }
    }
}
