using cyber_base.utils;
using cyber_base.view_model;
using progtroll.implement.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.actions
{
    internal class PRT_CalendarSwitchButtonAction : BaseViewModelCommandExecuter
    {
        public PRT_CalendarSwitchButtonAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger)
        {
        }
        protected override void ExecuteCommand()
        {
            var hrsViewModel = ViewModelManager.Current.HRSViewModel;
            hrsViewModel.CalendarNoteBookVisibility = System.Windows.Visibility.Visible;
            hrsViewModel.LogMonitorVisibility = System.Windows.Visibility.Hidden;
        }
    }
}
