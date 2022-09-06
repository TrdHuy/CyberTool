using cyber_base.implement.utils;
using extension_manager_service.@base;
using extension_manager_service.implement.module;
using extension_manager_service.implement.server_contact_manager.source_manager;
using extension_manager_service.views.elements.plugin_browser.items.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.implement.server_contact_manager
{
    internal class ServerContactManager : BaseExtensionManagerModule
    {
        private const string END_POINT = "http://107.127.131.89:8080";
        private const string REQUEST_INFO_API_PATH = "/requestinfo";
        private const string REQUEST_INFO_HEADER_KEY = "h2sw-request-info";

        private BrowserTabSourceManager? _browserTabSourceManager;

        public static ServerContactManager Current
        {
            get => ModuleManager.SCM_Instance;
        }

        public RangeObservableCollection<IPluginItemViewHolderContext>? BrowserTabPluginItemSource
        {
            get => _browserTabSourceManager?.PluginItemSource;
        }

        public override void OnModuleStart()
        {
            _browserTabSourceManager = new BrowserTabSourceManager();
        }

        public bool IsBrowserTabPluginSourceFullOfDbset()
        {
            return _browserTabSourceManager?.IsFullOfDbSet ?? false;
        }

        public async void RequestPluginsInfoFromCyberServer(Action? requestedCallback)
        {
            if (_browserTabSourceManager != null)
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        await _browserTabSourceManager.RequestServerData(client
                       , REQUEST_INFO_HEADER_KEY
                       , END_POINT + REQUEST_INFO_API_PATH);
                    }
                    catch (HttpRequestException ex)
                    {
                        if (ex.Message.Contains("No connection could be made because the target machine actively refused it"))
                        {
                            ExtensionManagerService
                                .Current
                                .ServiceManager?
                                .App.ShowWaringBox("Server not found!");
                        }
                    }
                    catch
                    {

                    }

                }

                requestedCallback?.Invoke();
            }
        }
    }
}
