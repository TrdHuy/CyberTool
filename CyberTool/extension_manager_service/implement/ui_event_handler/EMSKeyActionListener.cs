using cyber_base.ui_event_handler.action;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.listener;
using cyber_base.utils;
using cyber_base.view_model;
using extension_manager_service.@base;
using extension_manager_service.implement.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.implement.ui_event_handler
{
    internal class EMSKeyActionListener : BaseKeyActionListener, IExtensionManagerModule
    {

        public static EMSKeyActionListener Current
        {
            get
            {
                return ModuleManager.EKAL_Instance;
            }
        }
        
        private EMSKeyActionListener()
        {

        }

        public void OnModuleDestroy()
        {
        }

        public void OnModuleStart()
        {
        }

        public void OnViewInstantiated()
        {
        }

        protected override IAction? GetAction(string keyTag
            , string builderID
            , BaseViewModel? viewModel = null
            , ILogger? logger = null)
        {
            IAction? action;
            try
            {
                action = _actionExecuteHelper.GetActionInCache(builderID, keyTag);
            }
            catch
            {
                action = null;
            }

            if (action == null)
            {
                action = EMSCommandExecuterFactory
                    .Current
                    .CreateAction(builderID, keyTag, viewModel, logger);
            }

            return action;
        }

        protected override IAction? GetKeyActionAndLockFactory(string windowTag
            , string keytag
            , bool isLock = false
            , BuilderStatus status = BuilderStatus.Default
            , BaseViewModel? viewModel = null
            , ILogger? logger = null)
        {
            var action = GetAction(keytag, windowTag, viewModel, logger);
            EMSCommandExecuterFactory
                    .Current
                    .LockBuilder(builderID: windowTag, isLock, status);

            return action;
        }
    }
}
