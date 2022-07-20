using cyber_base.utils;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.action.executer
{
    public abstract class AbstractViewModelCommandExecuter : AbstractCommandExecuter, IViewModelCommandExecuter
    {
        public virtual BaseViewModel ViewModel { get; protected set; }

        public AbstractViewModelCommandExecuter(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, logger)
        {
            this.ViewModel = viewModel;
        }

        public AbstractViewModelCommandExecuter(string actionName, string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
            : base(actionName, actionID, builderID, logger)
        {
            this.ViewModel = viewModel;
        }

        /// <summary>
        /// Set completed flag for some command, because when some ExecuteVM() was call
        /// it may be async method, so should let inherited child overide the flag
        /// by their own.
        /// 
        /// And the flag will be true as default.
        /// </summary>
        protected override void SetCompleteFlagAfterExecuteCommand()
        {
            IsCompleted = true;
        }

        /// <summary>
        /// Check posibility of command with transfered data
        /// default = true
        /// </summary>
        /// <param name="dataTransfer">data passed into executer</param>
        /// <returns>true if meet condition and execute the command</returns>
        protected override bool CanExecute(object? dataTransfer)
        {
            return true;
        }


    }
}
