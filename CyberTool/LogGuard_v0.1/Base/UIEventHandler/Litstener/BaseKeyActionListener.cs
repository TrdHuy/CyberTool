using LogGuard_v0._1.Base.UIEventHandler.Action;
using LogGuard_v0._1.Base.UIEventHandler.Action.Builder;
using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.UIEventHandler.Litstener
{
    public abstract class BaseKeyActionListener : IActionListener
    {

        protected ActionExecuteHelper _actionExecuteHelper;

        public BaseKeyActionListener()
        {
            _actionExecuteHelper = ActionExecuteHelper.Current;
        }

        #region Onkey and execute action field
        public IAction OnKey(string windowTag, string keyFeature, object dataTransfer)
        {
            IAction action = GetKeyActionType(windowTag, keyFeature);
            return action;
        }

        public IAction OnKey(string windowTag, string keyFeature, object dataTransfer, BuilderLocker locker)
        {
            IAction action = GetKeyActionAndLockFactory(windowTag, keyFeature, locker.IsLock, locker.Status);
            return action;
        }

        public IAction OnKey(BaseViewModel viewModel, ILogger logger, string windowTag, string keyFeature, object dataTransfer)
        {
            IAction action = GetKeyActionType(windowTag, keyFeature, viewModel, logger);
            return action;
        }

        public IAction OnKey(BaseViewModel viewModel, ILogger logger, string windowTag, string keyFeature, object dataTransfer, BuilderLocker locker)
        {
            IAction action = GetKeyActionAndLockFactory(windowTag, keyFeature, locker.IsLock, locker.Status, viewModel, logger);
            return action;
        }

        #endregion


        private IAction GetKeyActionType(string windowTag
            , string keytag
            , BaseViewModel viewModel = null
            , ILogger logger = null)
        {
            return GetAction(keytag, windowTag, viewModel, logger);
        }

        protected abstract IAction GetKeyActionAndLockFactory(string windowTag
            , string keytag
            , bool isLock = false
            , BuilderStatus status = BuilderStatus.Default
            , BaseViewModel viewModel = null
            , ILogger logger = null);

        protected abstract IAction GetAction(string keyTag
            , string builderID
            , BaseViewModel viewModel = null
            , ILogger logger = null);
        

       

    }
}
