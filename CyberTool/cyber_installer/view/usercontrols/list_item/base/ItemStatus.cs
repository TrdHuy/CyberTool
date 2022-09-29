using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.view.usercontrols.list_item.available_item.@base
{
    public enum ItemStatus
    {
        None = 0,
        Downloadable = 1,
        Installable = 2,
        Updateable = 3,
        Downloading = 4,
        Installing = 5,
        Installed = 6,
        Uninstalling = 7
    }
}
