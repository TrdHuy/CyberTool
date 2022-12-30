using cyber_installer.implement.modules.server_contact_manager;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using cyber_installer.view.usercontrols.tabs;
using System.Collections.Generic;
using System.Threading;
using static cyber_installer.definitions.CyberInstallerDefinition;

namespace cyber_installer.view_models.tabs.available_tab
{
    internal class AvailableTabViewModel : BaseSoftwareStatusTabViewModel
    {
        public async override void OnTabOpened(BaseSoftwaresStatusTab sender)
        {
            if (App.Current.IsTaskAvailable(ManageableTaskKeyDefinition.UPDATE_SOFTWARE_TASK_TYPE_KEY)
                && App.Current.IsTaskAvailable(ManageableTaskKeyDefinition.DOWNLOAD_AND_INSTALL_SOFTWARE_TASK_TYPE_KEY))
            {
                ItemsSource.Clear();
                IsLoading = true;
                _requestDataTaskCancellationTokenSource = new CancellationTokenSource();
                _requestDataTask = ServerContactManager.Current.RequestMultipleSoftwareInfoFromCyberServer(isForce: true
                    , requestedCallback: (toolsSource) =>
                    {
                        if (toolsSource != null)
                        {
                            GenerateAvailableTool(toolsSource);
                        }
                        IsLoading = false;
                    }
                    , cancellationToken: _requestDataTaskCancellationTokenSource.Token);
                await _requestDataTask;
            }
        }

        public async override void OnScrollDownToBottom(object sender)
        {
            if (_requestDataTask != null && _requestDataTask.IsCompleted
                || _requestDataTask == null)
            {
                IsLoading = true;
                _requestDataTaskCancellationTokenSource = new CancellationTokenSource();
                _requestDataTask = ServerContactManager.Current.RequestMultipleSoftwareInfoFromCyberServer(
                    requestedCallback: (toolsSource) =>
                    {
                        if (toolsSource != null)
                        {
                            GenerateAvailableTool(toolsSource);
                        }
                        IsLoading = false;
                    }
                    , cancellationToken: _requestDataTaskCancellationTokenSource.Token);
                await _requestDataTask;
            }
        }

        public override void OnTabClosed(BaseSoftwaresStatusTab sender)
        {
            if (_requestDataTask != null
                && !_requestDataTask.IsCompleted
                && _requestDataTaskCancellationTokenSource != null)
            {
                _requestDataTaskCancellationTokenSource.Cancel();
            }
        }

        private void GenerateAvailableTool(ICollection<ToolVO> toolsSource)
        {
            foreach (var tool in toolsSource)
            {
                var availableItem = new AvailableItemViewModel(tool);
                ItemsSource.Add(availableItem);
            }
        }

        private ItemStatus GetItemStatus(ToolVO tool)
        {
            // TODO: Check item status here
            return ItemStatus.None;
        }


    }
}
