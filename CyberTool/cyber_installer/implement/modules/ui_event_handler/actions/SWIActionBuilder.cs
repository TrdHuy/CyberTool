using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.utils;
using cyber_base.view_model;
using cyber_installer.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.ui_event_handler.actions
{
    internal class SWIActionBuilder : AbstractExecutableCommandBuilder
    {
        private Logger logger = new Logger("SWIActionBuilder", CyberInstallerDefinition.CYBER_INSTALLER_INDENTIFER);

        public override ICommandExecuter? BuildAlternativeCommandExecuterWhenBuilderIsLock(string keyTag, ILogger? logger = null)
        {
            return null;
        }

        public override IViewModelCommandExecuter? BuildAlternativeViewModelCommandExecuterWhenBuilderIsLock(string keyTag, BaseViewModel viewModel, ILogger? logger = null)
        {
            return null;
        }

        public override ICommandExecuter? BuildCommandExecuter(string keyTag, ILogger? logger = null)
        {
            ICommandExecuter? commandExecuter = null;
            switch (keyTag)
            {
                default:
                    break;
            }
            return commandExecuter;
        }

        public override IViewModelCommandExecuter? BuildViewModelCommandExecuter(string keyTag, BaseViewModel viewModel, ILogger? logger = null)
        {
            IViewModelCommandExecuter? viewModelCommandExecuter = null;
            return viewModelCommandExecuter;
        }
    }
}

              