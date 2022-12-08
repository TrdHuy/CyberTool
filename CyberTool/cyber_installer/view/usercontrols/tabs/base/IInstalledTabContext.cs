using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.view.usercontrols.tabs.@base
{
    internal interface IInstalledTabContext
    {
        public void OnTabOpened(InstalledSofwaresTab sender);

        public void OnTabClosed(InstalledSofwaresTab sender);

        public void OnScrollDownToBottom(object sender);
    }
}
