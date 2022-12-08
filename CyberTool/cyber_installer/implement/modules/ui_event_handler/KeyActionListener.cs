using cyber_base.ui_event_handler.action;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.listener;
using cyber_base.utils;
using cyber_base.view_model;
using cyber_installer.@base;
using cyber_installer.implement.modules;
using cyber_installer.implement.modules.ui_event_handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.ui_event_handler
{
    internal class KeyActionListener : BaseKeyActionListener, ICyberInstallerModule
    {

        public static KeyActionListener Current
        {
            get
            {
                return ModuleManager.KAL_Instance;
            }
        }

        private KeyActionListener()
        {

        }

        public void OnModuleCreate()
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
            , object? dataTransfer
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
                action = CommandExecuterFactory
                    .Current
                    .CreateAction(builderID, keyTag, dataTransfer, viewModel, logger);
            }

            return action;
        }

        protected override IAction? GetKeyActionAndLockFactory(string windowTag
            , string keytag
            , object? dataTransfer
            , bool isLock = false
            , BuilderStatus status = BuilderStatus.Default
            , BaseViewModel? viewModel = null
            , ILogger? logger = null)
        {
            var action = GetAction(keytag, windowTag, dataTransfer, viewModel, logger);
            CommandExecuterFactory
                    .Current
                    .LockBuilder(builderID: windowTag, isLock, status);

            return action;
        }

        public void OnMainWindowShowed()
        {
        }
    }
}
