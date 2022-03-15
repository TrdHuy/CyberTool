﻿using LogGuard_v0._1.Base.UIEventHandler.Action.Builder;
using LogGuard_v0._1.Base.UIEventHandler.Action.Executer;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels
{
    public class MSW_LogWatcherControlButtonCommandVM : MSW_ButtonCommandViewModel
    {
        public CommandExecuterModel PlayButtonCommand { get; set; }

        public MSW_LogWatcherControlButtonCommandVM(BaseViewModel parentsModel) : base(parentsModel)
        {
            PlayButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_PLAY_FEATURE
                    , paramaters);
            });
        }
    }
}