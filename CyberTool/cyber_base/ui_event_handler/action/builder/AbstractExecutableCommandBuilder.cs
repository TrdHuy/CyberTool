using cyber_base.ui_event_handler.action.executer;
using cyber_base.utils;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.action.builder
{
    public abstract class AbstractExecutableCommandBuilder : AbstractActionBuilder, ICommandExecuterBuilder
    {
        public override IAction? BuildMainAction(string keyTag, object? dataTransfer)
        {
            return BuildCommandExecuter(keyTag, dataTransfer);
        }

        public override IAction? BuildAlternativeActionWhenFactoryIsLock(string keyTag, object? dataTransfer)
        {
            return BuildAlternativeCommandExecuterWhenBuilderIsLock(keyTag, dataTransfer);
        }

        public abstract ICommandExecuter? BuildAlternativeCommandExecuterWhenBuilderIsLock(string keyTag, object? dataTransfer, ILogger? logger = null);
        public abstract IViewModelCommandExecuter? BuildAlternativeViewModelCommandExecuterWhenBuilderIsLock(string keyTag, object? dataTransfer, BaseViewModel viewModel, ILogger? logger = null);

        public abstract ICommandExecuter? BuildCommandExecuter(string keyTag, object? dataTransfer, ILogger? logger = null);
        public abstract IViewModelCommandExecuter? BuildViewModelCommandExecuter(string keyTag, object? dataTransfer, BaseViewModel viewModel, ILogger? logger = null);


    }
}
