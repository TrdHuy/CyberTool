using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.StateController;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LogGuard_v0._1.Windows.MainWindow.Action.Types
{
    public class MSW_LogWatcher_ZoomButtonAction : BaseViewModelCommandExecuter
    {
        protected LogGuardPageViewModel LGPViewModel
        {
            get
            {
                return ViewModel as LogGuardPageViewModel;
            }
        }

        public MSW_LogWatcher_ZoomButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger) : base(actionID, builderID, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var cc = DataTransfer[0] as ContentControl;
            var opener = DataTransfer[1] as UIElement;
            if (cc != null)
            {
                var shouldRunLogCapture = false;
                if (StateControllerImpl.Current.IsRunning)
                {
                    StateControllerImpl.Current.Pause();
                    shouldRunLogCapture = true;
                }

                App.Current.ShowPopupCControl(cc
                    , opener: opener
                    , ownerWindow: OwnerWindow.MainScreen
                    , width: 900
                    , height: 700
                    , dataContext: LGPViewModel
                    , windowShowedCallback: (sender) =>
                    {
                        if (shouldRunLogCapture)
                        {
                            StateControllerImpl.Current.Resume();
                        }
                    }
                    , "Log Watcher");
            }
        }
    }
}