﻿using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.utils;
using cyber_base.view_model;
using progtroll.definitions;
using progtroll.implement.ui_event_handler.actions.log_monitor.button;
using progtroll.implement.ui_event_handler.actions.merge_tab.button;
using progtroll.implement.ui_event_handler.actions.notebook.context_menu;
using progtroll.implement.ui_event_handler.actions.project_manager.button;
using progtroll.implement.ui_event_handler.actions.project_manager.gesture;
using progtroll.implement.ui_event_handler.actions.release_tab.button;
using progtroll.implement.ui_event_handler.actions.version_manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.actions
{
    internal class SwPublishActionBuilder : AbstractExecutableCommandBuilder
    {
        private static Logger logger = new Logger("SwPublishActionBuilder", "HRT");

        public override ICommandExecuter? BuildAlternativeCommandExecuterWhenBuilderIsLock(string keyTag, object? dataTransfer, ILogger? logger = null)
        {
            return null;
        }

        public override IViewModelCommandExecuter? BuildAlternativeViewModelCommandExecuterWhenBuilderIsLock(string keyTag, object? dataTransfer, BaseViewModel viewModel, ILogger? logger = null)
        {
            return null;
        }

        public override ICommandExecuter? BuildCommandExecuter(string keyTag, object? dataTransfer, ILogger? logger = null)
        {
            ICommandExecuter? commandExecuter = null;
            switch (keyTag)
            {
                case PublisherKeyFeatureTag.KEY_TAG_PRT_NB_DELETE_PROJECT_ITEM_FEATURE:
                    commandExecuter = new PRT_NB_DeleteProjectItemContextMenuAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_VM_SHOW_COMMIT_DATA_GRID_FEATURE:
                    commandExecuter = new PRT_VM_ShowCommitDataGridAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_NB_RENAME_PROJECT_ITEM_FEATURE:
                    commandExecuter = new PRT_NB_RenameProjectItemContextMenuAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_NB_IMPORT_PROJECT_ITEM_FEATURE:
                    commandExecuter = new PRT_NB_ImportProjectItemContextMenuAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, logger);
                    break;
                default:
                    break;
            }
            return commandExecuter;
        }

        public override IViewModelCommandExecuter? BuildViewModelCommandExecuter(string keyTag, object? dataTransfer, BaseViewModel viewModel, ILogger? logger = null)
        {
            IViewModelCommandExecuter? viewModelCommandExecuter = null;
            switch (keyTag)
            {
                case PublisherKeyFeatureTag.KEY_TAG_PRT_PM_PROJECT_PATH_SELECTED_FEATURE:
                    viewModelCommandExecuter = new PRT_PM_ProjectPathFileSelectedAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_PM_VERSION_FILE_PATH_SELECTED_FEATUER:
                    viewModelCommandExecuter = new PRT_PM_VersionFilePathSelectedAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_PM_SELECTED_BRANCH_CHANGED_FEATURE:
                    viewModelCommandExecuter = new PRT_PM_SelectedBranchChangedAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_QUICK_RELEASE_FEATURE:
                    viewModelCommandExecuter = new PRT_RT_QuickReleaseButtonAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_RESTORE_LATEST_RELEASE_FEATURE:
                    viewModelCommandExecuter = new PRT_RT_RestoreLatestReleaseCommitAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_CREATE_RELEASE_CL_AND_COMMIT_FEATURE:
                    viewModelCommandExecuter = new PRT_RT_CreateReleaseCommitAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_PUSH_RELEASE_COMMIT_FEATURE:
                    viewModelCommandExecuter = new PRT_RT_PushReleaseCommitAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_SAVE_RELEASE_TEMPLATE_FEATURE:
                    viewModelCommandExecuter = new PRT_RT_SaveReleaseTemplateAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_PM_FETCH_PROJECT_FEATURE:
                    viewModelCommandExecuter = new PRT_PM_FetchProjectButtonAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_LM_CLEAR_LOG_CONTENT_FEATURE:
                    viewModelCommandExecuter = new PRT_LM_ClearLogContentButtonAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_LM_CLIPBOARD_LOG_CONTENT_FEATURE:
                    viewModelCommandExecuter = new PRT_LM_CopyLogToClipboardButtonAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_SWITCH_CALENDAR_FEATURE:
                    viewModelCommandExecuter = new PRT_CalendarSwitchButtonAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_SWITCH_LOG_MONITOR_FEATURE:
                    viewModelCommandExecuter = new PRT_LogMonitorSwitchButtonAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_RESTORE_LATEST_MERGE_FEATURE:
                    viewModelCommandExecuter = new PRT_MT_RestoreLatestMergeCommitButtonAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_CREATE_MERGE_CL_AND_COMMIT_FEATURE:
                    viewModelCommandExecuter = new PRT_MT_CreateMergeCommitButtonAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_CHECK_MERGE_CONFLICT_FEATURE:
                    viewModelCommandExecuter = new PRT_MT_CheckMergeConflictButtonAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                case PublisherKeyFeatureTag.KEY_TAG_PRT_PUSH_MERGE_COMMIT_FEATURE:
                    viewModelCommandExecuter = new PRT_MT_PushMergeCommitButtonAction(keyTag, PublisherDefinition.PUBLISHER_PLUGIN_TAG, dataTransfer, viewModel, logger);
                    break;
                default:
                    break;
            }
            return viewModelCommandExecuter;
        }
    }
}
