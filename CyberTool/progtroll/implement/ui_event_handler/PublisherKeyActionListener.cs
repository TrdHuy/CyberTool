using cyber_base.ui_event_handler.action;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.listener;
using cyber_base.utils;
using cyber_base.view_model;
using progtroll.@base.module;
using progtroll.implement.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler
{
    public class PublisherKeyActionListener : BaseKeyActionListener, IPublisherModule
    {

        public static PublisherKeyActionListener Current
        {
            get
            {
                return PublisherModuleManager.PKAL_Instance;
            }
        }

        public void OnDestroy()
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
                action = SwPublisherCommandExecuterFactory
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
            SwPublisherCommandExecuterFactory
                    .Current
                    .LockBuilder(builderID: windowTag, isLock, status);

            return action;
        }
    }
}
