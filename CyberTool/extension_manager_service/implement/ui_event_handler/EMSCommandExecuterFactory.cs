using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action;
using cyber_base.ui_event_handler.action.factory;
using cyber_base.utils;
using cyber_base.view_model;
using extension_manager_service.@base;
using extension_manager_service.definitions;
using extension_manager_service.implement.module;
using extension_manager_service.implement.ui_event_handler.actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.implement.ui_event_handler
{
    internal class EMSCommandExecuterFactory : BaseCommandExecuterFactory, IExtensionManagerModule
    {
        private static Logger logger = new Logger("EMSCommandExecuterFactory", "EMS");

        public static EMSCommandExecuterFactory Current
        {
            get
            {
                return ModuleManager.ECEF_Instance;
            }
        }

        public override ILogger Logger => logger;

        private EMSCommandExecuterFactory()
        {
            RegisterBuilder(ExtensionManagerDefinition.SERVICE_TAG, new EMSActionBuilder());
        }

        public void OnModuleStart()
        {
        }

        public override IAction? CreateAction(string builderID, string keyID, object? dataTransfer, BaseViewModel? viewModel = null, ILogger? logger = null)
        {
            IAction? action = base.CreateAction(builderID, keyID, dataTransfer, viewModel, logger);
            return action;
        }

        public void OnViewInstantiated()
        {
        }

        public void OnModuleDestroy()
        {
        }
    }
}