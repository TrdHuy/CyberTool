using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.Action.Types.LogManager
{
    public class MSW_LMUC_DeleteMessageItemAction : BaseViewModelCommandExecuter
    {
        protected LogManagerUCViewModel LMUCViewModel
        {
            get
            {
                return ViewModel as LogManagerUCViewModel;
            }
        }

        public MSW_LMUC_DeleteMessageItemAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger) : base(actionID, builderID, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var item = DataTransfer[0] as TrippleToggleItemViewModel;
            if (item != null)
            {
                LMUCViewModel.MessageManagerContent.Messagetems.Remove(item);
            }
        }
    }
}