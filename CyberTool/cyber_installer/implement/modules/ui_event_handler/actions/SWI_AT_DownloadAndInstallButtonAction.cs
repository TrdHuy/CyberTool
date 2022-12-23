using cyber_base.definition;
using cyber_base.utils;
using cyber_installer.implement.modules.sw_installing_manager;
using cyber_installer.implement.modules.user_data_manager;
using cyber_installer.implement.modules.utils;
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
        private ToolVO _toolInfo;
        private bool _isCreateDesktopShortcut;

        public SWI_AT_DownloadAndInstallButtonAction(string actionID, string builderID, object? dataTransfer, ILogger? logger)
            : base(actionID, builderID, dataTransfer, logger)
        {
            _availableItemViewModel = DataTransfer?[0] as AvailableItemViewModel
                ?? throw new ArgumentNullException();
            _toolInfo = _availableItemViewModel.ToolInfo as ToolVO
                ?? throw new ArgumentNullException();
        }


        protected override bool CanExecute(object? dataTransfer)
        {
            var confirm = App.Current.ShowYesNoQuestionBox($"Do you want to download & install '{_toolInfo.Name}'?");
            if (confirm == CyberContactMessage.No)
            {
                return false;
            }
            var destinationFolderWindow = App.Current.ShowDestinationFolderWindow(_toolInfo);
            _installPath = destinationFolderWindow.GetDestinationFolderPath();
            _isCreateDesktopShortcut = destinationFolderWindow.IsCreateDesktopShortcut();

            return !String.IsNullOrEmpty(_installPath)
                && _availableItemViewModel != null;
        }

        protected async override Task ExecuteCommandAsync()
        {
            _availableItemViewModel.ItemStatus = ItemStatus.Downloading;
            _availableItemViewModel.SwHandlingProgress = 0;

            var toolData = await SwInstallingManager.Current.StartDownloadingLatestVersionToolTask(_toolInfo
                , downloadProgressChangedCallback: (s, e) =>
                {
                    _availableItemViewModel.SwHandlingProgress = e;
                });

            if (toolData != null)
            {
                var userData = UserDataManager.Current.CurrentUserData;
                userData.ToolData.Add(toolData);
                _availableItemViewModel.ItemStatus = ItemStatus.Installing;
                _availableItemViewModel.SwHandlingProgress = 0;

                toolData = await SwInstallingManager.Current.StartToolInstallingTask(toolData
                    , _installPath
                    , installProgressChangedCallback: (progress) =>
                    {
                        _availableItemViewModel.SwHandlingProgress = progress;
                    });
                if (toolData?.ToolStatus == ToolStatus.Installed)
                {
                    if (_isCreateDesktopShortcut)
                    {
                        toolData.ShortcutPath = Utils.CreateDesktopShortCutToFile(toolData.ExecutePath);
                    }
                    _availableItemViewModel.ItemStatus = ItemStatus.UpToDate;
                }
                await UserDataManager.Current.ExportUserDataToFile();
            }

        }
    }

}
