using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.view.usercontrols.tabs.@base
{
    internal interface IAvailableTabContext
    {
        public void OnLoaded(AvailableSoftwaresTab sender);

        public void OnUnloaded(AvailableSoftwaresTab sender);

    }
}
