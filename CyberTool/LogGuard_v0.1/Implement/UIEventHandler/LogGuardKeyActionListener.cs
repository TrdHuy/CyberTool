using LogGuard_v0._1.Base.UIEventHandler.Action;
using LogGuard_v0._1.Base.UIEventHandler.Action.Builder;
using LogGuard_v0._1.Base.UIEventHandler.Litstener;
using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.UIEventHandler
{
    public class LogGuardKeyActionListener : BaseKeyActionListener
    {
        private static LogGuardKeyActionListener _instance;
        private LogGuardCommandExecuterFactory _commandExecuterFactory;

        private LogGuardKeyActionListener()
        {
            _commandExecuterFactory = LogGuardCommandExecuterFactory.Current;
        }

        public static LogGuardKeyActionListener Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogGuardKeyActionListener();
                }
                return _instance;
            }
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
                action = _commandExecuterFactory.CreateAction(builderID, keyTag, viewModel, logger);
            }

            return action;
        }

        protected override IAction GetKeyActionAndLockFactory(string windowTag, string keytag, bool isLock = false, BuilderStatus status = BuilderStatus.Default, BaseViewModel viewModel = null, ILogger logger = null)
        {
            var action = GetAction(keytag, windowTag, viewModel, logger);
            _commandExecuterFactory.LockBuilder(builderID: windowTag, isLock, status);

            return action;
        }
    }
}
