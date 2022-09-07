using cyber_base.view_model;
using extension_manager_service.views.elements.plugin_browser.items.@base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.view_models.tabs.plugin_browser.items
{
    internal class VersionHistoryItemViewModel : BaseViewModel, IVersionHistoryItemViewHolderContext
    {
        private bool _isFirst;
        private bool _isLast;
        private bool _isInstalled;
        private string _version = "";
        private string _description = "";

        [Bindable(true)]
        public bool IsInstalled
        {
            get => _isInstalled;
            set
            {
                _isInstalled = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Description
        {
            get => _description; set
            {
                _description = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsFirst
        {
            get => _isFirst;
            set
            {
                _isFirst = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsLast
        {
            get => _isLast;
            set
            {
                _isLast = value;
                InvalidateOwn();
            }
        }
    }
}
