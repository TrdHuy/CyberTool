using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.utils;
using cyber_base.view_model;
using cyber_installer.definitions;
using cyber_installer.implement.modules.ui_event_handler.actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.ui_event_handler
{
    internal class SWIActionBuilder : AbstractExecutableCommandBuilder
    {
        private Logger logger = new Logger("SWIActionBuilder", CyberInstallerDefinition.CYBER_INSTALLER_INDENTIFER);

        public override ICommandExecuter? BuildAlternativeCommandExecuterWhenBuilderIsLock(string keyTag, object? dataTransfer, ILogger? logger = null)
        {
            return null;
        }

        public override IViewModelCommandExecuter? BuildAlternativeViewModelCommandExecuterWhenBuilderIsLock(string keyTag, object? dataTransfer, BaseViewModel viewModel, ILogger? logger = null)
        {
            return null;
        }

        public override ICommandExecuter? BuildCommandExecuter(string keyTag, object? dataTransfer, ILogger? logger = null)
        {
            ICommandExecuter? commandExecuter = null;
            switch (keyTag)
            {
                case CyberInstallerKeyFeatureTag.KEY_TAG_SWI_AT_DOWNLOAD_AND_INSTALL_FEATURE:
                    commandExecuter = new SWI_AT_DownloadAndInstallButtonAction(keyTag
                        , CyberInstallerDefinition.CYBER_INSTALLER_INDENTIFER
                        , dataTransfer
                        , logger);
                    break;
                default:
                    break;
            }
            return commandExecuter;
        }

        public override IViewModelCommandExecuter? BuildViewModelCommandExecuter(string keyTag, object? dataTransfer, BaseViewModel viewModel, ILogger? logger = null)
        {
            IViewModelCommandExecuter? viewModelCommandExecuter = null;
            return viewModelCommandExecuter;
        }
    }
}

