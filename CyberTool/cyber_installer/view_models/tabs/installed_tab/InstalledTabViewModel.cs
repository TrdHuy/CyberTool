using cyber_base.implement.utils;
using cyber_base.view_model;
using cyber_installer.implement.modules.server_contact_manager;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using cyber_installer.view.usercontrols.tabs;
using cyber_installer.view.usercontrols.tabs.@base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.view_models.tabs.installed_tab
{
    internal class InstalledTabViewModel : BaseViewModel, IInstalledTabContext
    {
        [Bindable(true)]
        public RangeObservableCollection<InstalledItemViewModel> ItemsSource { get; set; } = new RangeObservableCollection<InstalledItemViewModel>();

        public void OnTabOpened(InstalledSofwaresTab sender)
        {

        }

        public void OnTabClosed(InstalledSofwaresTab sender)
        {

        }

        public void OnScrollDownToBottom(object sender)
        {
            //do something
        }
    }
}
