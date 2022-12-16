using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.view.usercontrols.tabs.@base
{
    internal interface ISoftwareStatusTabContext
    {
        public bool IsLoading { get; set; }

        public void OnTabOpened(BaseSoftwaresStatusTab sender);

        public void OnTabClosed(BaseSoftwaresStatusTab sender);

        public void OnScrollDownToBottom(object sender);


    }
}
