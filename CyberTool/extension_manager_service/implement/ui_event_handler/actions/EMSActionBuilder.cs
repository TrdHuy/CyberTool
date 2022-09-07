using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.utils;
using cyber_base.view_model;
using extension_manager_service.definitions;
using extension_manager_service.implement.ui_event_handler.actions.plugin_item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.implement.ui_event_handler.actions
{
    internal class EMSActionBuilder : AbstractExecutableCommandBuilder
    {
        private static Logger logger = new Logger("EMSActionBuilder", "EMS");

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
            switch (keyTag)
            {
                case ExtensionManagerKeyFeatureTag.KEY_TAG_EMS_INSTALL_PLUGIN_FEATURE:
                    viewModelCommandExecuter = new EMS_PI_InstallPluginButtonAction(keyTag, ExtensionManagerDefinition.SERVICE_TAG, viewModel, logger);
                    break;
                case ExtensionManagerKeyFeatureTag.KEY_TAG_EMS_UNINSTALL_PLUGIN_FEATURE:
                    viewModelCommandExecuter = new EMS_PI_UninstallPluginButtonAction(keyTag, ExtensionManagerDefinition.SERVICE_TAG, viewModel, logger);
                    break;
                default:
                    break;
            }
            return viewModelCommandExecuter;
        }
    }
}

