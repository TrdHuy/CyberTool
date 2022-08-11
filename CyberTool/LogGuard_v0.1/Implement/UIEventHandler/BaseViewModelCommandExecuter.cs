using LogGuard_v0._1.Base.UIEventHandler.Action.Executer;
using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.UIEventHandler
{
    public class BaseViewModelCommandExecuter : AbstractViewModelCommandExecuter
    {
        public BaseViewModelCommandExecuter(string actionID, string builderID, BaseViewModel viewModel, ILogger logger) : base(actionID, builderID, viewModel, logger)
        {
        }
        public BaseViewModelCommandExecuter(string actionName, string actionID, string builderID, BaseViewModel viewModel, ILogger logger) : base(actionName, actionID, builderID, viewModel, logger)
        {
        }

        protected override bool CanExecute(object dataTransfer)
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
    }
}