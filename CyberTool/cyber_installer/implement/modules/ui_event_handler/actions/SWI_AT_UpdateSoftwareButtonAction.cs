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
    class SWI_AT_UpdateSoftwareButtonAction : BaseCommandExecuter
    {
        private AvailableItemViewModel _availableItemViewModel;
        private ToolVO _serverToolInfo;
        private ToolData? _installationToolData;

        public SWI_AT_UpdateSoftwareButtonAction(string actionID, string builderID, object? dataTransfer, ILogger? logger)
            : base(actionID, builderID, dataTransfer, logger)
        {
            _availableItemViewModel = DataTransfer?[0] as AvailableItemViewModel
                ?? throw new ArgumentNullException();
            _serverToolInfo = _availableItemViewModel.ToolInfo as ToolVO
                ?? throw new ArgumentNullException();
            _installationToolData = _availableItemViewModel.GetToolDataCache();
        }


        protected override bool CanExecute(object? dataTransfer)
        {
            var confirm = App.Current.ShowYesNoQuestionBox($"Do you want to update new version '{_serverToolInfo.LatestVersion}'?");
            if (confirm == CyberContactMessage.No)
            {
                return false;
            }
            return _installationToolData != null
                && _availableItemViewModel != null;
        }

        protected async override Task ExecuteCommandAsync()
        {
            if (_installationToolData == null) return;

            _availableItemViewModel.ItemStatus = ItemStatus.Downloading;
            _availableItemViewModel.SwHandlingProgress = 0;

            var isDownloadedSuccess = await SwInstallingManager.Current.StartDownloadingLatestUpdateVersionForTool(_serverToolInfo
                , _installationToolData
                , downloadProgressChangedCallback: (s, e) =>
                {
                    _availableItemViewModel.SwHandlingProgress = e;
                });

            if (isDownloadedSuccess)
            {
                _availableItemViewModel.ItemStatus = ItemStatus.Installing;
                _availableItemViewModel.SwHandlingProgress = 0;
                await SwInstallingManager.Current.StartInstallUpdateVersionOfTool(_installationToolData
                   , progressChangedCallback: (e) =>
                   {
                       _availableItemViewModel.SwHandlingProgress = e;
                   });

                if (_installationToolData.ToolStatus == ToolStatus.Installed
                    && _installationToolData.ToolVersionSource.Last().VersionStatus == ToolVersionStatus.VersionInstalled)
                {
                    App.Current.ShowSuccessBox($"Updated {_installationToolData.Name} to " +
                        $"{_installationToolData.CurrentInstalledVersion} successfully");
                    _availableItemViewModel.ItemStatus = ItemStatus.UpToDate;
                }
                else
                {
                    App.Current.ShowWaringBox($"Failed to install version {_installationToolData.CurrentInstalledVersion}!");
                    _availableItemViewModel.ItemStatus = ItemStatus.InstallFailed;
                }
            }
            else
            {
                App.Current.ShowWaringBox($"Failed to download version {_serverToolInfo.ToolVersions.Last().Version.Trim()} " +
                    $"of {_installationToolData.Name}!");
                _availableItemViewModel.ItemStatus = ItemStatus.Updateable;
            }


        }
    }

}
