using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view_model;
using extension_manager_service.implement.plugin_manager;
using extension_manager_service.implement.ui_event_handler.async_tasks;
using extension_manager_service.view_models.tabs.plugin_browser.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace extension_manager_service.implement.ui_event_handler.actions.plugin_item
{
    internal class EMS_PI_InstallPluginButtonAction : BaseViewModelCommandExecuter
    {
        private PluginItemViewModel PIViewModel { get; set; }

        public EMS_PI_InstallPluginButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, viewModel, logger)
        {
            PIViewModel = viewModel as PluginItemViewModel ?? throw new ArgumentNullException("view model must be type of PluginItemViewModel");
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            if (string.IsNullOrEmpty(PIViewModel.Version))
            {
                ExtensionManagerService
                    .Current
                    .ServiceManager?.App.ShowWaringBox("Please wait to sync data!");
                return false;
            }

            if (CyberPluginManager.Current.CheckPlugiIsInstalled(PIViewModel.PluginKey))
            {
                ExtensionManagerService
                     .Current
                     .ServiceManager?.App.ShowWaringBox("This plugin is already installed");
                return false;
            }

            var confirm = ExtensionManagerService
                .Current
                .ServiceManager?.App.ShowYesNoQuestionBox("Do you want to download and install this plugin!");

            return confirm == cyber_base.definition.CyberContactMessage.Yes;
        }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var isDownloadable = false;
            var downloadPluginTask = new CommonEMSParamAsyncTask(
                    param: PIViewModel
                    , name: "Downloading " + PIViewModel.PluginName
                    , mainFunc: async (param, result, cancellationToken) =>
                    {
                        isDownloadable = await CyberPluginManager.Current.DownloadPluginFromCyberServer(
                            PIViewModel.PluginKey
                            , PIViewModel.Version);
                        if (!isDownloadable)
                        {
                            ExtensionManagerService
                               .Current
                               .ServiceManager?.App.ShowWaringBox("Download failed!");
                        }
                    }
                    , delayTime: 3000
                    , estimateTime: 3000
                );


            var installPluginTask = new CommonEMSParamAsyncTask(
                param: PIViewModel
                , name: "Installing " + PIViewModel.PluginName
                , mainAct: (param, result, cancellationToken) =>
                {
                    var installedSuccessfully = CyberPluginManager.Current.InstallDownloadedPlugin(
                        PIViewModel.PluginKey
                        , PIViewModel.Version);
                    if (installedSuccessfully)
                    {
                        PIViewModel.IsInstalled = true;
                        result.MesResult = MessageAsyncTaskResult.OK;
                    }
                    else
                    {
                        result.MesResult = MessageAsyncTaskResult.Faulted;
                    }
                }
                , callBack: (param, result) =>
                {
                    if (result.MesResult == MessageAsyncTaskResult.Faulted)
                    {
                        ExtensionManagerService
                              .Current
                              .ServiceManager?.App.ShowWaringBox("Failed to install " + PIViewModel.PluginName + " !");
                    }
                    else if (result.MesResult == MessageAsyncTaskResult.OK)
                    {
                        ExtensionManagerService
                              .Current
                              .ServiceManager?.App.ShowWaringBox("Installed " + PIViewModel.PluginName + " successfully!");
                    }
                }
                , delayTime: 3000
                , estimateTime: 3000);
            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(downloadPluginTask);
            tasks.Add(installPluginTask);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                , new CancellationTokenSource()
                , null
                , name: "Download & install " + PIViewModel.PluginName
                , delayTime: 6000
                , reportDelay: 100);
            ExtensionManagerService.Current.ServiceManager?.App.OpenMultiTaskBox(
                title: "Download & install " + PIViewModel.PluginName
                , task: multiTask
                , isCancelable: false);

        }
    }
}
