using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.ui_event_handler.listener;
using cyber_base.view_model;
using extension_manager_service.definitions;
using extension_manager_service.implement.ui_event_handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.view_models.commands
{
    internal class BaseEMSCommandViewModel : CommandViewModel
    {
        private Logger _logger;

        protected override Logger logger => _logger;

        protected override IActionListener _keyActionListener => EMSKeyActionListener.Current;

        public BaseEMSCommandViewModel(BaseViewModel parentsModel, string commandVMTag = "BaseEMSCommandViewModel") : base(parentsModel)
        {
            _logger = new Logger(commandVMTag, "EMS");
        }

        public ICommandExecuter? GetCommandExecuter(string keyTag
            , object? paramaters
            , bool isViewModelOnKey = true
            , string builderTag = ExtensionManagerDefinition.SERVICE_TAG)
        {
            return base.OnKey(keyTag, paramaters, isViewModelOnKey, builderTag);
        }

        protected override ICommandExecuter? OnKey(string keyTag
            , object? paramaters
            , bool isViewModelOnKey = true
            , string builderTag = ExtensionManagerDefinition.SERVICE_TAG)
        {
            return base.OnKey(keyTag, paramaters, isViewModelOnKey, builderTag);
        }

        protected override ICommandExecuter? OnKey(string keyTag
            , object? paramaters
            , BuilderLocker locker
            , bool isViewModelOnKey = true
            , string builderTag = ExtensionManagerDefinition.SERVICE_TAG)
        {
            return base.OnKey(keyTag, paramaters, locker, isViewModelOnKey, builderTag);
        }
    }
}
