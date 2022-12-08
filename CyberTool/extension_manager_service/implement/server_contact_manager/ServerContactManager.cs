using cyber_base.implement.utils;
using extension_manager_service.@base;
using extension_manager_service.implement.log_manager;
using extension_manager_service.implement.module;
using extension_manager_service.implement.server_contact_manager.plugin_download_manager;
using extension_manager_service.implement.server_contact_manager.source_manager;
using extension_manager_service.models.user_data;
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
        private const string END_POINT = "http://107.98.32.108:8080";
        private const string REQUEST_INFO_API_PATH = "/requestinfo";
        private const string DOWNLOAD_PLUGIN_API_PATH = "/downloadplugin";
        private const string REQUEST_INFO_HEADER_KEY = "h2sw-request-info";
        private const string REQUEST_DOWNLOAD_PLUGIN_HEADER_KEY = "h2sw-download-plugin";

        private BrowserTabSourceManager? _browserTabSourceManager;
        private PluginDownloadManager? _pluginDownloadManager;

        public static ServerContactManager Current
        {
            get => ModuleManager.SCM_Instance;
        }

        public RangeObservableCollection<IPluginItemViewHolderContext>? BrowserTabPluginItemSource
        {
            get => _browserTabSourceManager?.PluginItemSource;
        }

        private ServerContactManager()
        {

        }

        public override void OnModuleStart()
        {
            _browserTabSourceManager = new BrowserTabSourceManager();
            _pluginDownloadManager = new PluginDownloadManager();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginKey"></param>
        /// <param name="pluginVersion"></param>
        // Phương thức này chỉ nên sử dụng cho CyberPluginManager
        // Tại CyberPluginManager, hệ thống sẽ kiểm tra plugin cần download đã được
        // cài đặt hay chưa, và có được phép tải xuống hay không

        public async Task DownloadPluginFromCyberServer(string pluginKey
            , string pluginVersion
            , Action<PluginVersionUD?>? requestedCallback = null
            , Action<double>? downloadProgressChangedCallback = null)
        {
            if (_pluginDownloadManager != null)
            {
                PluginVersionUD? pluginVersionUD = null;
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        pluginVersionUD = await _pluginDownloadManager.StartDownloadPlugin(client
                       , REQUEST_DOWNLOAD_PLUGIN_HEADER_KEY
                       , END_POINT + DOWNLOAD_PLUGIN_API_PATH
                       , pluginKey
                       , pluginVersion
                       , downloadProgressChangedCallback);
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
                        EMSLogManager.E(ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        EMSLogManager.E(ex.ToString());
                    }

                }

                requestedCallback?.Invoke(pluginVersionUD);
            }
        }
    }
}
