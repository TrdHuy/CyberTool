using cyber_base.definition;
using cyber_base.utils;
using cyber_base.view_model;
using log_guard.definitions;
using log_guard.implement.flow.state_controller;
using log_guard.implement.flow.view_helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace log_guard.implement.ui_event_handler.actions.log_watcher.button
{
    internal class MSW_LogWatcher_ZoomButtonAction : LG_ViewModelCommandExecuter
    {
        public MSW_LogWatcher_ZoomButtonAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var cc = LogGuardViewHelper.Current.GetViewByKey(LogGuardViewKeyDefinition.LogWatcherContentControl) as ContentControl;
            var opener = LogGuardViewHelper.Current.GetViewByKey(LogGuardViewKeyDefinition.LogWatcherZoomButton) as UIElement;
            if (cc != null)
            {
                var shouldRunLogCapture = false;
                if (StateController.Current.IsRunning)
                {
                    StateController.Current.Pause();
                    shouldRunLogCapture = true;
                }

                LogGuardService
                    .Current?
                    .ServiceManager
                    .App
                    .ShowPopupCControl(cc
                    , opener: opener
                    , ownerWindow: CyberOwner.ServiceManager
                    , width: 900
                    , height: 700
                    , dataContext: LGPViewModel
                    , windowShowedCallback: (sender) =>
                    {
                        if (shouldRunLogCapture)
                        {
                            StateController.Current.Resume();
                        }
                    }
                    , "Log Watcher");
            }
        }
    }
}