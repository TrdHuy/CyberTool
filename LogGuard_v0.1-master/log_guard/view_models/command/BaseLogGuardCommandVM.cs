using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.ui_event_handler.listener;
using cyber_base.view_model;
using log_guard.definitions;
using log_guard.implement.ui_event_handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.command
{
    internal class BaseLogGuardCommandVM : CommandViewModel
    {
        private static Logger _logger;

        protected override Logger logger => _logger;

        protected override IActionListener _keyActionListener => LogGuardKeyActionListener.Current;

        public BaseLogGuardCommandVM(BaseViewModel parentsModel, string commandVMTag = "BaseLogGuardCommandVM") : base(parentsModel)
        {
            _logger = new Logger(commandVMTag, "log_guard");
        }

        protected override ICommandExecuter OnKey(string keyTag
            , object paramaters
            , bool isViewModelOnKey = true
            , string builderTag = LogGuardDefinition.LOG_GUARD_SERVICE_TAG)
        {
            return base.OnKey(keyTag, paramaters, isViewModelOnKey, builderTag);
        }

        protected override ICommandExecuter OnKey(string keyTag
            , object paramaters
            , BuilderLocker locker
            , bool isViewModelOnKey = true
            , string builderTag = LogGuardDefinition.LOG_GUARD_SERVICE_TAG)
        {
            return base.OnKey(keyTag, paramaters, locker, isViewModelOnKey, builderTag);
        }
    }
}
