using cyber_base.utils;
using cyber_base.view_model;
using honeyboard_release_service.implement.view_model;
using honeyboard_release_service.view_models.project_manager.items;
using honeyboard_release_service.view_models.tab_items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.actions.project_manager.listview
{
    internal class PRT_PM_UpdateVersionManagerTab : BaseCommandExecuter
    {
        private VersionManagerTabViewModel versionManagerTabVM; 
        public PRT_PM_UpdateVersionManagerTab(string actionID, string builderID, ILogger? logger) : base(actionID, builderID, logger)
        {
            versionManagerTabVM = ViewModelManager.Current.VMTViewModel;
        }

        protected override void ExecuteCommand()
        {
            if (DataTransfer != null)
            {
                var forcusVersionCommit = DataTransfer[0] as VersionHistoryItemViewModel;
                if (forcusVersionCommit != null)
                {
                    versionManagerTabVM.CurrentFocusVersionCommitVM = forcusVersionCommit;
                }
            }
        }
    }
}
