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
        public override IAction? BuildMainAction(string keyTag)
        {
            return BuildCommandExecuter(keyTag);
        }

        public override IAction? BuildAlternativeActionWhenFactoryIsLock(string keyTag)
        {
            return BuildAlternativeCommandExecuterWhenBuilderIsLock(keyTag);
        }

        public abstract ICommandExecuter? BuildAlternativeCommandExecuterWhenBuilderIsLock(string keyTag, ILogger? logger = null);
        public abstract IViewModelCommandExecuter? BuildAlternativeViewModelCommandExecuterWhenBuilderIsLock(string keyTag, BaseViewModel viewModel, ILogger? logger = null);

        public abstract ICommandExecuter? BuildCommandExecuter(string keyTag, ILogger? logger = null);
        public abstract IViewModelCommandExecuter? BuildViewModelCommandExecuter(string keyTag, BaseViewModel viewModel, ILogger? logger = null);


    }
}
