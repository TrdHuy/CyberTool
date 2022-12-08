using cyber_base.utils;
using cyber_installer.implement.modules.sw_installing_manager;
using cyber_installer.implement.modules.user_data_manager;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using cyber_installer.view_models.tabs.available_tab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.ui_event_handler.actions
{
    internal class SWI_AT_DownloadAndInstallButtonAction : BaseCommandExecuter
    {

        private string _installPath = "";
        private AvailableItemViewModel _availableItemViewModel;

        public SWI_AT_DownloadAndInstallButtonAction(string actionID, string builderID, object? dataTransfer, ILogger? logger)
            : base(actionID, builderID, dataTransfer, logger)
        {
            _availableItemViewModel = DataTransfer?[0] as AvailableItemViewModel
                ?? throw new ArgumentNullException();
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            _installPath = App.Current.ShowDestinationFolderWindow(_availableItemViewModel.ToolInfo);
            return !String.IsNullOrEmpty(_installPath)
                && _availableItemViewModel != null;
        }

        protected async override void ExecuteCommand()
        {
            _availableItemViewModel.ItemStatus = ItemStatus.Downloading;
            var toolData = await SwInstallingManager.Current.StartDownloadingLatestVersionToolTask(_availableItemViewModel.ToolInfo);

            if (toolData != null)
            {
                var userData = UserDataManager.Current.CurrentUserData;
                userData.ToolData.Add(toolData);
                _availableItemViewModel.ItemStatus = ItemStatus.Installing;
                toolData = await SwInstallingManager.Current.StartToolInstallingTask(toolData, _installPath);
                await UserDataManager.Current.ExportUserDataToFile();
                _availableItemViewModel.ItemStatus = ItemStatus.Installed;

            }

        }
    }

}
