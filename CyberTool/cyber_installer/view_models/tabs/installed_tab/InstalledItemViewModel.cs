using cyber_base.view_model;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.view_models.tabs.installed_tab
{
    internal class InstalledItemViewModel : ItemViewModel
    {
        public InstalledItemViewModel(ToolVO toolVO) : base(toolVO)
        {
        }

        protected override void InstantiateItemStatus()
        {
        }
    }
}
