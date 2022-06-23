using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.ui_event_handler.listener;
using cyber_base.view_model;
using honeyboard_release_service.definitions;
using honeyboard_release_service.implement.ui_event_handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models.command
{
    internal class BaseSwPublisherCommandVM : CommandViewModel
    {
        private Logger _logger;

        protected override Logger logger => _logger;

        protected override IActionListener _keyActionListener => PublisherKeyActionListener.Current;

        public BaseSwPublisherCommandVM(BaseViewModel parentsModel, string commandVMTag = "BaseSwPublisherCommandVM") : base(parentsModel)
        {
            _logger = new Logger(commandVMTag);
        }

        protected override ICommandExecuter? OnKey(string keyTag
            , object? paramaters
            , bool isViewModelOnKey = true
            , string builderTag = PublisherDefinition.PUBLISHER_PLUGIN_TAG)
        {
            return base.OnKey(keyTag, paramaters, isViewModelOnKey, builderTag);
        }

        protected override ICommandExecuter? OnKey(string keyTag
            , object? paramaters
            , BuilderLocker locker
            , bool isViewModelOnKey = true
            , string builderTag = PublisherDefinition.PUBLISHER_PLUGIN_TAG)
        {
            return base.OnKey(keyTag, paramaters, locker, isViewModelOnKey, builderTag);
        }
    }
}
