using LogGuard_v0._1._Config;
using LogGuard_v0._1.Base.Command;
using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.LogGuard.Control;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.Action.Gestures
{
    public class MSW_LogWatcher_DeleteGestureAction : BaseViewModelCommandExecuter
    {
        protected LogGuardPageViewModel LGPViewModel
        {
            get
            {
                return ViewModel as LogGuardPageViewModel;
            }
        }

        public MSW_LogWatcher_DeleteGestureAction(string actionID
            , string builderID
            , BaseViewModel viewModel
            , ILogger logger)
            : base(actionID, builderID, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();

            if (!RUNE.IS_SUPPORT_DELETE_LOG_LINE)
            {
                App.Current.ShowWaringBox("Current version not support this feature!");
                return;
            }

            if (LGPViewModel.UseAutoScroll)
            {
                App.Current.ShowWaringBox("Turn off Auto scroll before deleting line!");
                return;
            }

            var watcher = DataTransfer[0] as LogWatcher;
            if (watcher != null)
            {
                var notifier = watcher.SelectedItems as INotifyCollectionChanged;
                var items = watcher.SelectedItems
                    .OfType<LogWatcherItemViewModel>();
                SourceManagerImpl.Current.DeleteSeletedLogLine(items, notifier);
            }
        }


    }
}