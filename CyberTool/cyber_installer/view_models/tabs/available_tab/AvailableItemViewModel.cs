using cyber_base.view_model;
using cyber_installer.implement.modules.user_data_manager;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.view_models.tabs.available_tab
{
    internal class AvailableItemViewModel : ItemViewModel
    {
        private const int IMPORT_USER_DATA_TIME_OUT = 1000;
        public AvailableItemViewModel(ToolVO toolVO) : base(toolVO)
        {
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
                    .Where(td => td.ToolKey == _toolVO.StringId)
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
                                if (_toolVO.ToolVersions.Count > 0)
                                {
                                    if (System.Version.Parse(toolData.CurrentInstalledVersion)
                                        == System.Version.Parse(_toolVO.ToolVersions[0].Version))
                                    {
                                        ItemStatus = ItemStatus.UpToDate;
                                    }
                                    else if (System.Version.Parse(toolData.CurrentInstalledVersion)
                                        < System.Version.Parse(_toolVO.ToolVersions[0].Version))
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


