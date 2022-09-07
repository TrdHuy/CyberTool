using cyber_base.implement.command;
using cyber_base.implement.utils;
using cyber_base.view_model;
using extension_manager_service.@base.view_model;
using extension_manager_service.implement.server_contact_manager;
using extension_manager_service.view_models.tabs.plugin_browser.items;
using extension_manager_service.views.elements.plugin_browser.items.@base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace extension_manager_service.view_models.tabs.plugin_browser
{
    internal class PluginBrowserTabViewModel : BaseViewModel, IEMSScrollViewModel
    {
        private RangeObservableCollection<IPluginItemViewHolderContext>? _pluginItemSource;
        private PluginItemViewMode _currentListViewMode;
        private object? _currentSelectedPlugin;
        private bool _isLoadingPluginsInfoFromServer;

        [Bindable(true)]
        public bool IsLoadingPluginsInfoFromServer
        {
            get => _isLoadingPluginsInfoFromServer;
            set
            {
                _isLoadingPluginsInfoFromServer = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public RangeObservableCollection<IPluginItemViewHolderContext>? PluginItemSource
        {
            get => _pluginItemSource;
            set
            {
                _pluginItemSource = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public PluginItemViewMode CurrentListViewMode
        {
            get => _currentListViewMode;
            set
            {
                _currentListViewMode = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object? CurrentSelectedPlugin
        {
            get => _currentSelectedPlugin;
            set
            {
                if (_currentSelectedPlugin == null && value != null)
                {
                    CurrentListViewMode = PluginItemViewMode.Half;
                }
                else if (value == null && _currentSelectedPlugin != null)
                {
                    CurrentListViewMode = PluginItemViewMode.Full;
                }
                _currentSelectedPlugin = value;

                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public ICommand CloseDetailPanelButtonCmd { get; set; }

        public PluginBrowserTabViewModel(BaseViewModel parents) : base(parents)
        {
            _pluginItemSource = ServerContactManager.Current.BrowserTabPluginItemSource;
            CloseDetailPanelButtonCmd = new BaseDotNetCommandImpl((s) =>
            {
                CurrentSelectedPlugin = null;
            });

            IsLoadingPluginsInfoFromServer = true;
            ServerContactManager.Current.RequestPluginsInfoFromCyberServer(
                    requestedCallback: () =>
                    {
                        IsLoadingPluginsInfoFromServer = false;
                    });
        }
        
        public void OnScrollDownToBottom(object sender)
        {
            if (!ServerContactManager.Current.IsBrowserTabPluginSourceFullOfDbset())
            {
                IsLoadingPluginsInfoFromServer = true;
                ServerContactManager.Current.RequestPluginsInfoFromCyberServer(
                        requestedCallback: () =>
                        {
                            IsLoadingPluginsInfoFromServer = false;
                        });
            }
        }
    }
}
