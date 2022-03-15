using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.AndroidLog;
using LogGuard_v0._1.Implement.Device;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.Models;
using LogGuard_v0._1.Windows.MainWindow.ViewModels;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogGuard_v0._1.Implement.LogGuardFlow;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Implement.LogGuardFlow.StateController;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages;

namespace LogGuard_v0._1.Windows.MainWindow.Action.Types
{
    public class MSW_LogWatcher_PlayButtonAction : BaseViewModelCommandExecuter
    {
        protected LogGuardPageViewModel LGPViewModel
        {
            get
            {
                return ViewModel as LogGuardPageViewModel;
            }
        }

        public MSW_LogWatcher_PlayButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger) : base(actionID, builderID, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();

            if (HighCpu_StateController.Current.CurrentState == LogGuardState.NONE
                || HighCpu_StateController.Current.CurrentState == LogGuardState.STOP)
            {
                LGPViewModel.CurrentLogGuardState = LogGuardState.RUNNING;
                HighCpu_StateController.Current.Start();
            }
            else if (HighCpu_StateController.Current.CurrentState == LogGuardState.RUNNING)
            {
                LGPViewModel.CurrentLogGuardState = LogGuardState.PAUSING;
                HighCpu_StateController.Current.Pause();
            }
            else if (HighCpu_StateController.Current.CurrentState == LogGuardState.PAUSING)
            {
                LGPViewModel.CurrentLogGuardState = LogGuardState.RUNNING;

                HighCpu_StateController.Current.Resume();
            }


            //if (LowCpu_StateController.Current.CurrentState == LogGuardState.NONE
            //    || LowCpu_StateController.Current.CurrentState == LogGuardState.STOP)
            //{
            //    MSWViewModel.CurrentLogGuardState = LogGuardState.RUNNING;
            //    LowCpu_StateController.Current.Start();
            //}
            //else if (LowCpu_StateController.Current.CurrentState == LogGuardState.RUNNING)
            //{
            //    MSWViewModel.CurrentLogGuardState = LogGuardState.PAUSING;
            //    LowCpu_StateController.Current.Pause();
            //}
            //else if (LowCpu_StateController.Current.CurrentState == LogGuardState.PAUSING)
            //{
            //    MSWViewModel.CurrentLogGuardState = LogGuardState.RUNNING;

            //    LowCpu_StateController.Current.Resume();
            //}

        }


    }
}
