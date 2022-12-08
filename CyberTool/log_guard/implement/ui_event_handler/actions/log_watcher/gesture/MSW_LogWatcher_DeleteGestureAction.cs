using cyber_base.utils;
using cyber_base.view_model;
using log_guard._config;
using log_guard.definitions;
using log_guard.implement.flow.source_manager;
using log_guard.implement.flow.view_helper;
using log_guard.view_models.watcher;
using log_guard.views.others.log_watcher;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.ui_event_handler.actions.log_watcher.gesture
{
    internal class MSW_LogWatcher_DeleteGestureAction : LG_ViewModelCommandExecuter
    {
        public MSW_LogWatcher_DeleteGestureAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();

            if (!RUNE.IS_SUPPORT_DELETE_LOG_LINE)
            {
                LogGuardService
                    .Current?
                    .ServiceManager
                    .App
                    .ShowWaringBox("Current version not support this feature!");
                return;
            }

            if (LGPViewModel.UseAutoScroll)
            {
                LogGuardService
                   .Current?
                   .ServiceManager
                   .App
                   .ShowWaringBox("Turn off Auto scroll before deleting line!");
                return;
            }

            var watcher = LogGuardViewHelper
                .Current
                .GetViewByKey(LogGuardViewKeyDefinition.LogWatcher) as LogWatcher;

            if (watcher != null)
            {
                var notifier = watcher.SelectedItems as INotifyCollectionChanged;
                var items = watcher.SelectedItems
                    .OfType<LogWatcherItemViewModel>();
                SourceManager.Current.DeleteSeletedLogLine(items, notifier);
            }
        }


    }
}