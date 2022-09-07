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
    internal class EMS_PI_UninstallPluginButtonAction : BaseViewModelCommandExecuter
    {
        private PluginItemViewModel PIViewModel { get; set; }

        public EMS_PI_UninstallPluginButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, viewModel, logger)
        {
            PIViewModel = viewModel as PluginItemViewModel ?? throw new ArgumentNullException("view model must be type of PluginItemViewModel");
        }

        protected override bool CanExecute(object? dataTransfer)
        {

            var confirm = ExtensionManagerService
                .Current
                .ServiceManager?.App.ShowYesNoQuestionBox("Do you want to download and uninstall this plugin!");

            return confirm == cyber_base.definition.CyberContactMessage.Yes;
        }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var uninstallPluginTask = new CommonEMSParamAsyncTask(
                param: PIViewModel
                , name: "Uninstalling " + PIViewModel.PluginName
                , mainAct: (param, result, cancellationToken) =>
                {
                    var installedSuccessfully = CyberPluginManager.Current.UninstallDownloadedPlugin(PIViewModel.PluginKey);
                    if (installedSuccessfully)
                    {
                        PIViewModel.IsInstalled = false;
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
                              .ServiceManager?.App.ShowWaringBox("Failed to uninstall " + PIViewModel.PluginName + " !");
                    }
                    else if (result.MesResult == MessageAsyncTaskResult.OK)
                    {
                        ExtensionManagerService
                              .Current
                              .ServiceManager?.App.ShowWaringBox("Uninstalled " + PIViewModel.PluginName + " successfully!");
                    }

                }
                , delayTime: 3000
                , estimateTime: 3000);
            List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
            tasks.Add(uninstallPluginTask);

            MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                , new CancellationTokenSource()
                , null
                , name: "Uninstall " + PIViewModel.PluginName
                , delayTime: 3000
                , reportDelay: 100);
            ExtensionManagerService.Current.ServiceManager?.App.OpenMultiTaskBox(
                title: "Uninstall " + PIViewModel.PluginName
                , task: multiTask
                , isCancelable: false
                , isUseMultiTaskReport: false);
        }
    }
}
