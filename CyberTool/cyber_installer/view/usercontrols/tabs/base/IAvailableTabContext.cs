using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.view.usercontrols.tabs.@base
{
    internal interface IAvailableTabContext
    {
        public void OnTabOpened(AvailableSoftwaresTab sender);

        public void OnTabClosed(AvailableSoftwaresTab sender);
        
        public void OnScrollDownToBottom(object sender);

        
    }
}
