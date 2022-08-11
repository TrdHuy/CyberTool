using LogGuard_v0._1.Base.LogGuardFlow;
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

namespace LogGuard_v0._1.Windows.MainWindow.Action.Types
{
    public class MSW_LogWatcher_ImportLogFileButtonAction : BaseViewModelCommandExecuter
    {
        protected LogGuardPageViewModel LGPViewModel
        {
            get
            {
                return ViewModel as LogGuardPageViewModel;
            }
        }

        public MSW_LogWatcher_ImportLogFileButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            LGPViewModel.SelectParserOption(Implement.AndroidLog.LogParser.LogParserOption.DUMPSTATE_PARSER);

            var path = App.Current.OpenFileChooserDialogWindow();
            if (!string.IsNullOrEmpty(path))
            {
                StateControllerImpl.Current.StartImportLogFile(path);
            }
        }


    }
}
