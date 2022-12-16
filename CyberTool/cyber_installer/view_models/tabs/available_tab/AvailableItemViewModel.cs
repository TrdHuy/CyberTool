using cyber_base.implement.command;
using cyber_base.ui_event_handler.action.executer;
using cyber_installer.definitions;
using cyber_installer.implement.modules.ui_event_handler;
using cyber_installer.implement.modules.user_data_manager;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace cyber_installer.view_models.tabs.available_tab
{
    internal class AvailableItemViewModel : ItemViewModel
    {
        private const int IMPORT_USER_DATA_TIME_OUT = CyberInstallerDefinition.IMPORT_USER_DATA_TIME_OUT;
        private ICommand _downloadAndInstallCommand;
        public ICommand DownloadAndInstallCommand { get => _downloadAndInstallCommand; }

        public AvailableItemViewModel(ToolVO toolVO) : base(toolVO)
        {

            _downloadAndInstallCommand = new CommandExecuterImpl((paramaters) =>
            {
                var data = paramaters ?? this;
                return KeyActionListener.Current.OnKey(CyberInstallerDefinition.CYBER_INSTALLER_INDENTIFER
                    , CyberInstallerKeyFeatureTag.KEY_TAG_SWI_AT_DOWNLOAD_AND_INSTALL_FEATURE
                    , data) as ICommandExecuter;
            });

        }

        protected override async void InstantiateItemStatus()
        {
            IsLoadingItemStatus = true;
            if (await UserDataManager
                .Current
                .WaitForImportUserDataTask(IMPORT_USER_DATA_TIME_OUT))
            {
                await Task.Delay(2000);

                var userData = UserDataManager.Current.CurrentUserData;
                var toolData = userData
                    .ToolData
                    .Where(td => td.StringId == _toolVO.StringId)
                    .FirstOrDefault();

                // Kiểm tra dữ liệu tool trên server đã có trong
                // dữ liệu ở local hay chưa
                if (toolData != null)
                {
                    if (toolData.ToolStatus == ToolStatus.Downloaded)
                    {
                        ItemStatus = ItemStatus.Installable;
                    }

                    switch (toolData.ToolStatus)
                    {
                        case ToolStatus.Downloaded:
                            {
                                ItemStatus = ItemStatus.Installable;
                                break;
                            }
                        case ToolStatus.InstallFailed:
                            {
                                ItemStatus = ItemStatus.InstallFailed;
                                break;
                            }
                        case ToolStatus.Installed:
                            {
                                var versionSource = (_toolVO as ToolVO)?.ToolVersions;
                                if (versionSource != null && versionSource.Count > 0)
                                {
                                    if (System.Version.Parse(toolData.CurrentInstalledVersion)
                                        == System.Version.Parse(versionSource.Last().Version))
                                    {
                                        ItemStatus = ItemStatus.UpToDate;
                                    }
                                    else if (System.Version.Parse(toolData.CurrentInstalledVersion)
                                        < System.Version.Parse(versionSource.Last().Version))
                                    {
                                        ItemStatus = ItemStatus.Updateable;
                                    }
                                }
                                break;
                            }
                    }
                }
                else
                {
                    ItemStatus = ItemStatus.Downloadable;
                }
            }
            IsLoadingItemStatus = false;
        }
    }
}


