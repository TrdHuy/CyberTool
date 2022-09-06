using cyber_base.implement.utils;
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
    internal class PluginItemViewModel : BaseViewModel, IPluginItemViewHolderContext
    {
        private string _pluginName = "";
        private string _pluginAuthor = "";
        private string _pluginDescription = "";
        private string _version = "";
        private string _datePublished = "";
        private string _projectURL = "";
        private Uri? _iconSource;
        private bool _isAuthenticated = false;
        private bool _isInstalled = false;
        private int _downLoads = 0;
        private double _rates = 0;
        private string[] _tags = new string[0];
        private FirstLastObservableCollection<IVersionHistoryItemViewHolderContext> _versionHistorySource 
            = new FirstLastObservableCollection<IVersionHistoryItemViewHolderContext>();


        [Bindable(true)]
        public string PluginName
        {
            get => _pluginName;
            set
            {
                _pluginName = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string PluginAuthor
        {
            get => _pluginAuthor;
            set
            {
                _pluginAuthor = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string PluginDescription
        {
            get => _pluginDescription;
            set
            {
                _pluginDescription = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set
            {
                _isAuthenticated = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public Uri IconSource
        {
            get => _iconSource ?? new Uri("");
            set
            {
                _iconSource = value;
                InvalidateOwn();
            }
        }

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
        public int Downloads
        {
            get => _downLoads;
            set
            {
                _downLoads = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public double Rates
        {
            get => _rates;
            set
            {
                _rates = value;
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
        public string DatePublished
        {
            get => _datePublished;
            set
            {
                _datePublished = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string ProjectURL
        {
            get => _projectURL;
            set
            {
                _projectURL = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string[] Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                InvalidateOwn();
            }
        }

        public FirstLastObservableCollection<IVersionHistoryItemViewHolderContext> VersionHistorySource
        {
            get => _versionHistorySource;
            set
            {
                _versionHistorySource = value;
                InvalidateOwn();
            }
        }
    }
}
