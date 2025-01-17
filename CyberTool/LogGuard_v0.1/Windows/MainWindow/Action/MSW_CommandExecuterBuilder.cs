﻿using LogGuard_v0._1.Base.UIEventHandler.Action.Builder;
using LogGuard_v0._1.Base.UIEventHandler.Action.Executer;
using LogGuard_v0._1.Base.UIEventHandler.Action.Factory;
using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.Action.Gestures;
using LogGuard_v0._1.Windows.MainWindow.Action.Types;
using LogGuard_v0._1.Windows.MainWindow.Action.Types.LogManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.Action
{
    public class MSW_CommandExecuterBuilder : AbstractCommandExecuterBuilder
    {
        private static Logger logger = new Logger("MSW_CommandExecuterBuilder");


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
                case KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_PLAY_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_PlayButtonAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_CLEAR_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_ClearButtonAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_STOP_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_StopButtonAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_REFRESH_DEVICE_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_RefreshDeviceButtonAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_CTRLA_GESTURE_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_CtrlAGestureAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_DELETE_GESTURE_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_DeleteGestureAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_ZOOM_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_ZoomButtonAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_TAG_DOUBLE_CLICK_GESTURE_FEATURE:
                    viewModelCommandExecuter = new MSW_LWI_LogWatcher_TagDoubleClickAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_MESSAGE_DOUBLE_CLICK_GESTURE_FEATURE:
                    viewModelCommandExecuter = new MSW_LWI_LogWatcher_MessageDoubleClickAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_EDIT_TAG_ITEM_FEATURE:
                    viewModelCommandExecuter = new MSW_LMUC_EditTagItemAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_DELETE_TAG_ITEM_FEATURE:
                    viewModelCommandExecuter = new MSW_LMUC_DeleteTagItemAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_EDIT_MESSAGE_ITEM_FEATURE:
                    viewModelCommandExecuter = new MSW_LMUC_EditMessageItemAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_DELETE_MESSAGE_ITEM_FEATURE:
                    viewModelCommandExecuter = new MSW_LMUC_DeleteMessageItemAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_IMPORT_LOG_FILE_FEATURE:
                    viewModelCommandExecuter = new MSW_LogWatcher_ImportLogFileButtonAction(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                case KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_PARSER_ITEM_SELECTED_GESTURE_FEATURE:
                    viewModelCommandExecuter = new MSW_LPI_LeftMouseClick(keyTag, WindowTag.WINDOW_TAG_MAIN_SCREEN, viewModel, logger);
                    break;
                default:
                    break;
            }
            return viewModelCommandExecuter;
        }
    }
}
