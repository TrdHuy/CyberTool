﻿using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action;
using cyber_base.ui_event_handler.action.factory;
using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.@base.module;
using honeyboard_release_service.definitions;
using honeyboard_release_service.implement.module;
using honeyboard_release_service.implement.ui_event_handler.actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler
{
    public class SwPublisherCommandExecuterFactory : BaseCommandExecuterFactory, IPublisherModule
    {
        private static Logger logger = new Logger("LogGuardCommandExecuterFactory");

        public static SwPublisherCommandExecuterFactory Current
        {
            get
            {
                return PublisherModuleManager.SPCEF_Instance;
            }
        }

        public override ILogger Logger => logger;

        public SwPublisherCommandExecuterFactory()
        {
            RegisterBuilder(PublisherDefinition.PUBLISHER_PLUGIN_TAG, new SwPublishActionBuilder());
        }

        public void OnModuleStart()
        {
        }

        public override IAction CreateAction(string builderID, string keyID, BaseViewModel? viewModel = null, ILogger? logger = null)
        {
            IAction action = base.CreateAction(builderID, keyID, viewModel, logger);

            return action;
        }

        public void OnViewInstantiated()
        {
        }
    }
}