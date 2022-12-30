using cyber_base.definition;
using cyber_base.utils;
using cyber_installer.implement.modules.sw_installing_manager;
using cyber_installer.implement.modules.user_data_manager;
using cyber_installer.implement.modules.utils;
using cyber_installer.implement.modules.view_model_manager;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using cyber_installer.view_models.tabs.installed_tab;
using System;
using System.IO;
using System.Threading.Tasks;
using static cyber_installer.definitions.CyberInstallerDefinition;
using static cyber_installer.implement.app_support_modules.TaskHandleManager;

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
            if (!App.Current.IsTaskAvailable(ManageableTaskKeyDefinition.UNINSTALL_SOFTWARE_TASK_TYPE_KEY))
            {
                App.Current.ShowWaringBox("The uninstall process is current running!");
                return false;
            }
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
            await App.Current.ExecuteManageableTask(ManageableTaskKeyDefinition.UNINSTALL_SOFTWARE_TASK_TYPE_KEY
               , asyncTask: async () =>
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
               , isBybassIfSemaphoreNotAvaild: true);
        }
    }
}
