﻿using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action;
using cyber_base.ui_event_handler.action.factory;
using cyber_base.utils;
using cyber_base.view_model;
using log_guard.@base.module;
using log_guard.definitions;
using log_guard.implement.module;
using log_guard.implement.ui_event_handler.actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.ui_event_handler
{
    public class LogGuardCommandExecuterFactory : BaseCommandExecuterFactory, ILogGuardModule
    {
        private Logger logger = new Logger("LogGuardCommandExecuterFactory", "log_guard");

        public static LogGuardCommandExecuterFactory Current
        {
            get
            {
                return LogGuardModuleManager.LGCEF_Instance;
            }
        }

        public override ILogger Logger => logger;

        public LogGuardCommandExecuterFactory()
        {
            RegisterBuilder(LogGuardDefinition.LOG_GUARD_SERVICE_TAG, new LogGuardActionBuilder());
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
    }
}