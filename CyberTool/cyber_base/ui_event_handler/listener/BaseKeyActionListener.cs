using cyber_base.ui_event_handler.action;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.utils;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.listener
{
    public abstract class BaseKeyActionListener : IActionListener
    {

        protected ActionExecuteHelper _actionExecuteHelper;

        public BaseKeyActionListener()
        {
            _actionExecuteHelper = ActionExecuteHelper.Current;
        }

        #region Onkey and execute action field
        public IAction? OnKey(string builderTag, string keyFeature, object dataTransfer)
        {
            IAction? action = GetKeyActionType(builderTag, keyFeature, dataTransfer);
            return action;
        }

        public IAction? OnKey(string builderTag, string keyFeature, object dataTransfer, BuilderLocker locker)
        {
            IAction? action = GetKeyActionAndLockFactory(builderTag, keyFeature, dataTransfer, locker.IsLock, locker.Status);
            return action;
        }

        public IAction? OnKey(BaseViewModel viewModel, ILogger logger, string builderTag, string keyFeature, object dataTransfer)
        {
            IAction? action = GetKeyActionType(builderTag, keyFeature, dataTransfer, viewModel, logger);
            return action;
        }

        public IAction? OnKey(BaseViewModel viewModel, ILogger logger, string builderTag, string keyFeature, object dataTransfer, BuilderLocker locker)
        {
            IAction? action = GetKeyActionAndLockFactory(builderTag, keyFeature, dataTransfer, locker.IsLock, locker.Status, viewModel, logger);
            return action;
        }

        #endregion


        private IAction? GetKeyActionType(string builderTag
            , string keytag
            , object? dataTransfer
            , BaseViewModel? viewModel = null
            , ILogger? logger = null)
        {
            return GetAction(keytag, builderTag, dataTransfer, viewModel, logger);
        }

        protected abstract IAction? GetKeyActionAndLockFactory(string builderTag
            , string keytag
            , object? dataTransfer
            , bool isLock = false
            , BuilderStatus status = BuilderStatus.Default
            , BaseViewModel? viewModel = null
            , ILogger? logger = null);

        protected abstract IAction? GetAction(string keyTag
            , string builderID
            , object? dataTransfer
            , BaseViewModel? viewModel = null
            , ILogger? logger = null);
    }
}
