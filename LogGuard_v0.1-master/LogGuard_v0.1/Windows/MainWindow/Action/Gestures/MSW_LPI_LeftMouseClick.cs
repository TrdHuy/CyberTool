using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.Action.Gestures
{
    public class MSW_LPI_LeftMouseClick : BaseViewModelCommandExecuter
    {
        protected LogGuardPageViewModel LGPViewModel
        {
            get
            {
                return ViewModel as LogGuardPageViewModel;
            }
        }

        public MSW_LPI_LeftMouseClick(string actionID
            , string builderID
            , BaseViewModel viewModel
            , ILogger logger)
            : base(actionID, builderID, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var vm = DataTransfer[0] as LogParserItemViewModel;
            if (vm != null)
            {
               
            }
        }
    }
}