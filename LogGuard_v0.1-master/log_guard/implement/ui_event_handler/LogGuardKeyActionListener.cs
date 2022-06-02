using cyber_base.ui_event_handler.action;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.listener;
using cyber_base.utils;
using cyber_base.view_model;
using log_guard.@base.module;
using log_guard.implement.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.ui_event_handler
{
    public class LogGuardKeyActionListener : BaseKeyActionListener, ILogGuardModule
    {

        public static LogGuardKeyActionListener Current
        {
            get
            {
                return LogGuardModuleManager.LGKAL_Instance;
            }
        }

        public void OnModuleDestroy()
        {
        }

        public void OnModuleStart()
        {
        }

        protected override IAction GetAction(string keyTag, string builderID, BaseViewModel viewModel = null, ILogger logger = null)
        {
            IAction action;
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
                action = LogGuardCommandExecuterFactory
                    .Current
                    .CreateAction(builderID, keyTag, viewModel, logger);
            }

            return action;
        }

        protected override IAction GetKeyActionAndLockFactory(string windowTag, string keytag, bool isLock = false, BuilderStatus status = BuilderStatus.Default, BaseViewModel viewModel = null, ILogger logger = null)
        {
            var action = GetAction(keytag, windowTag, viewModel, logger);
            LogGuardCommandExecuterFactory
                    .Current
                    .LockBuilder(builderID: windowTag, isLock, status);

            return action;
        }
    }
}
