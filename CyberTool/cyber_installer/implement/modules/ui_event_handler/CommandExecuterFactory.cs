using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action;
using cyber_base.ui_event_handler.action.factory;
using cyber_base.utils;
using cyber_base.view_model;
using cyber_installer.@base;
using cyber_installer.definitions;

namespace cyber_installer.implement.modules.ui_event_handler
{
    internal class CommandExecuterFactory : BaseCommandExecuterFactory, ICyberInstallerModule
    {
        private Logger logger = new Logger("CommandExecuterFactory", CyberInstallerDefinition.CYBER_INSTALLER_INDENTIFER);

        public static CommandExecuterFactory Current
        {
            get
            {
                return ModuleManager.CEF_Instance;
            }
        }

        public override ILogger Logger => logger;

        private CommandExecuterFactory()
        {
            RegisterBuilder(CyberInstallerDefinition.CYBER_INSTALLER_INDENTIFER, new SWIActionBuilder());
        }

        public void OnModuleStart()
        {
        }

        public override IAction? CreateAction(string builderID, string keyID, object? dataTransfer, BaseViewModel? viewModel = null, ILogger? logger = null)
        {
            IAction? action = base.CreateAction(builderID, keyID, dataTransfer, viewModel, logger);

            return action;
        }


        public void OnModuleDestroy()
        {
        }

        public void OnModuleCreate()
        {
        }
    }
}