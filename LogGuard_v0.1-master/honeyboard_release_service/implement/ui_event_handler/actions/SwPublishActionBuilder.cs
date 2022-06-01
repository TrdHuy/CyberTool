﻿using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.definitions;
using honeyboard_release_service.implement.ui_event_handler.actions.project_manager.gesture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.actions
{
    internal class SwPublishActionBuilder : AbstractExecutableCommandBuilder
    {
        private static Logger logger = new Logger("SwPublishActionBuilder");

        public override ICommandExecuter BuildAlternativeCommandExecuterWhenBuilderIsLock(string keyTag, ILogger logger = null)
        {
            return null;
        }

        public override IViewModelCommandExecuter BuildAlternativeViewModelCommandExecuterWhenBuilderIsLock(string keyTag, BaseViewModel viewModel, ILogger logger = null)
        {
            return null;
        }

        public override ICommandExecuter BuildCommandExecuter(string keyTag, ILogger logger = null)
        {
            return null;
        }

        public override IViewModelCommandExecuter BuildViewModelCommandExecuter(string keyTag, BaseViewModel viewModel, ILogger logger = null)
        {
            IViewModelCommandExecuter viewModelCommandExecuter = null;
            switch (keyTag)
            {
                case PublisherKeyFeatureTag.KEY_TAG_PRT_PM_PROJECT_PATH_SELECTED_FEATURE:
                    viewModelCommandExecuter = new PRT_PM_ProjectPathFileSelectedAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, viewModel, logger);
                    break;
                default:
                    break;
            }
            return viewModelCommandExecuter;
        }
    }
}
