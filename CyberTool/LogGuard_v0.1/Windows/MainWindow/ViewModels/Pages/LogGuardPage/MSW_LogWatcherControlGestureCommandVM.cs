﻿using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage
{
    public class MSW_LogWatcherControlGestureCommandVM : MSW_ButtonCommandViewModel
    {
        public CommandExecuterModel LogTagDoubleClickCommand { get; set; }
        public CommandExecuterModel LogMessageDoubleClickCommand { get; set; }
        public CommandExecuterModel ParserFormatSelectedCommand { get; set; }

        public MSW_LogWatcherControlGestureCommandVM(BaseViewModel parentsModel) : base(parentsModel)
        {
            LogTagDoubleClickCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_TAG_DOUBLE_CLICK_GESTURE_FEATURE
                    , paramaters);
            });

            LogMessageDoubleClickCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_MESSAGE_DOUBLE_CLICK_GESTURE_FEATURE
                    , paramaters);
            });

            ParserFormatSelectedCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_PARSER_ITEM_SELECTED_GESTURE_FEATURE
                    , paramaters);
            });
        }
    }
}
