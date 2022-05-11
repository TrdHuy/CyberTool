using cyber_tool.@base.page.model;
using System;
using System.Windows.Controls;

namespace cyber_tool.@base.page.page_controller
{
    public abstract class BasePageController
    {
        public abstract event CurrentPageSourceChangedHandler CurrentPageSourceChanged;
        public ServiceVO CurrentServicePageOV { get; set; }
        public ServiceVO PreviousServicePageOV { get; set; }
        public abstract void UpdateCurrentServiceVOByID(string id);

    }

    public delegate void CurrentPageSourceChangedHandler(object sender);
}
