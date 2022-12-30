using cyber_base.implement.utils;
using cyber_base.view_model;
using cyber_installer.definitions;
using cyber_installer.implement.modules.server_contact_manager;
using cyber_installer.implement.modules.user_data_manager;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using cyber_installer.view.usercontrols.tabs;
using cyber_installer.view.usercontrols.tabs.@base;
using cyber_installer.view_models.tabs.available_tab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static cyber_installer.definitions.CyberInstallerDefinition;

namespace cyber_installer.view_models.tabs.installed_tab
{
    internal class InstalledTabViewModel : BaseSoftwareStatusTabViewModel
    {
        private const int IMPORT_USER_DATA_TIME_OUT = CyberInstallerDefinition.IMPORT_USER_DATA_TIME_OUT;

        public async override void OnTabOpened(BaseSoftwaresStatusTab sender)
        {
            if (App.Current.IsTaskAvailable(ManageableTaskKeyDefinition.UNINSTALL_SOFTWARE_TASK_TYPE_KEY))
            {
                IsLoading = true;
                _requestDataTaskCancellationTokenSource = new CancellationTokenSource();
                if (await UserDataManager
                    .Current
                    .WaitForImportUserDataTask(IMPORT_USER_DATA_TIME_OUT))
                {
                    ItemsSource.Clear();

                    _requestDataTask = UserDataManager
                        .Current
                        .GetAllInstalledToolFromUserData(
                            toolDataExtractingEvent: (data) =>
                            {
                                var itemVM = new InstalledItemViewModel(data);
                                ItemsSource.Add(itemVM);
                            }
                            , _requestDataTaskCancellationTokenSource.Token
                            );
                    await _requestDataTask;
                }
                IsLoading = false;
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

        public override void OnScrollDownToBottom(object sender)
        {
        }
    }
}
