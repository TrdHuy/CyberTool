using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.utils;
using cyber_base.view_model;
using log_guard.definitions;
using log_guard.implement.ui_event_handler.actions.log_manager.button;
using log_guard.implement.ui_event_handler.actions.log_watcher.button;
using log_guard.implement.ui_event_handler.actions.log_watcher.gesture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.ui_event_handler.actions
{
    internal class LogGuardActionBuilder : AbstractExecutableCommandBuilder
    {
        private static Logger logger = new Logger("LogGuardActionBuilder", "log_guard");

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
            return null;
        }

        public override IViewModelCommandExecuter? BuildViewModelCommandExecuter(string keyTag, object? dataTransfer, BaseViewModel viewModel, ILogger? logger = null)
        {
            IViewModelCommandExecuter? viewModelCommandExecuter = null;
            switch (keyTag)
            {
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_PLAY_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_PlayButtonAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_CLEAR_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_ClearButtonAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_STOP_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_StopButtonAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_REFRESH_DEVICE_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_RefreshDeviceButtonAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_CTRLA_GESTURE_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_CtrlAGestureAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_DELETE_GESTURE_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_DeleteGestureAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_ZOOM_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_ZoomButtonAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_TAG_DOUBLE_CLICK_GESTURE_FEATURE:
                    viewModelCommandExecuter = new MSW_LWI_LogWatcher_TagDoubleClickAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_MESSAGE_DOUBLE_CLICK_GESTURE_FEATURE:
                    viewModelCommandExecuter = new MSW_LWI_LogWatcher_MessageDoubleClickAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_EDIT_TAG_ITEM_FEATURE:
                    viewModelCommandExecuter = new MSW_LMUC_EditTagItemAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_DELETE_TAG_ITEM_FEATURE:
                    viewModelCommandExecuter = new MSW_LMUC_DeleteTagItemAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_EDIT_MESSAGE_ITEM_FEATURE:
                    viewModelCommandExecuter = new MSW_LMUC_EditMessageItemAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_DELETE_MESSAGE_ITEM_FEATURE:
                    viewModelCommandExecuter = new MSW_LMUC_DeleteMessageItemAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_IMPORT_LOG_FILE_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_ImportLogFileButtonAction(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                case LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_PARSER_ITEM_SELECTED_GESTURE_FEATURE:
                    viewModelCommandExecuter = new MSW_LPI_LeftMouseClick(keyTag, LogGuardDefinition.LOG_GUARD_SERVICE_TAG, dataTransfer, viewModel, logger);
                    break;
                default:
                    break;
            }
            return viewModelCommandExecuter;
        }
    }
}
