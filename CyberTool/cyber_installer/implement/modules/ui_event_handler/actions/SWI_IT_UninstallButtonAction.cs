using cyber_base.definition;
using cyber_base.utils;
using cyber_installer.@base.model;
using cyber_installer.definitions;
using cyber_installer.implement.modules.sw_installing_manager;
using cyber_installer.implement.modules.user_data_manager;
using cyber_installer.implement.modules.utils;
using cyber_installer.implement.modules.view_model_manager;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using cyber_installer.view_models.tabs.available_tab;
using cyber_installer.view_models.tabs.installed_tab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.ui_event_handler.actions
{
    internal class SWI_IT_UninstallButtonAction : BaseCommandExecuter
    {
        private string _installPath;
        private InstalledItemViewModel _installedItemViewModel;
        private ToolData _toolData;
        public SWI_IT_UninstallButtonAction(string actionID, string builderID, object? dataTransfer, ILogger? logger)
           : base(actionID, builderID, dataTransfer, logger)
        {
            _installedItemViewModel = DataTransfer?[0] as InstalledItemViewModel ?? throw new ArgumentNullException();
            _toolData = _installedItemViewModel.ToolInfo as ToolData ?? throw new ArgumentNullException();
            _installPath = _toolData.InstallPath;
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            var installedSoftwareInfoFilePath = Utils.GetInstalledSoftwareInfoFilePath(_installPath);
            var confirm = App.Current.ShowYesNoQuestionBox($"Do you want to uninstall {_toolData.Name} ?")
                  == CyberContactMessage.Yes;
            var isUninstallable = File.Exists(installedSoftwareInfoFilePath);
            if (!isUninstallable)
            {
                App.Current.ShowWaringBox("Not found installation info! Please try again!");
            }
            return isUninstallable && confirm;
        }

        protected async override Task ExecuteCommandAsync()
        {
            _installedItemViewModel.ItemStatus = ItemStatus.Uninstalling;
            var userData = UserDataManager.Current.CurrentUserData;

            await SwInstallingManager.Current.StartUninstallToolTask(_toolData
                , progressChangedCallback: (s, e2) =>
                {
                    _installedItemViewModel.SwHandlingProgress = e2;
                });
            if (_toolData.ToolStatus == ToolStatus.Removed)
            {
                userData.ToolData.Remove(_toolData);
                await UserDataManager.Current.ExportUserDataToFile();
                ViewModelManager.Current.InstalledTabViewModel.ItemsSource.Remove(_installedItemViewModel);
            }

        }
    }
}
