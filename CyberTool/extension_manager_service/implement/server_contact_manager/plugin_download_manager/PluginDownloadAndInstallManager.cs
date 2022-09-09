using extension_manager_service.implement.ui_event_handler.async_tasks.http_request;
using extension_manager_service.models.user_data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace extension_manager_service.implement.server_contact_manager.plugin_download_manager
{
    internal class PluginDownloadAndInstallManager
    {
        private const int TIME_OUT_FOR_REQUEST_OF_SEMAPHORE = 100;

        private SemaphoreSlim _downloadPluginSemaphore;
        private SemaphoreSlim _installPluginSemaphore;
        private const string REQUEST_CHECK_PLUGIN_DOWNLOADABLE_HEADER_ID = "CHECK_DOWNLOADABLE_PLUGIN";
        private const string REQUEST_KEY_TO_CHECK_DOWNLOADABLE_HEADER_ID = "CHECK_DOWNLOADABLE_PLUGIN__PLUGIN_KEY";
        private const string REQUEST_VERSION_TO_CHECK_DOWNLOADABLE_HEADER_ID = "CHECK_DOWNLOADABLE_PLUGIN__PLUGIN_VERSION";
        private const string RESPONSE_IS_PLUGIN_DOWNLOADABLE_HEADER_ID = "CHECK_DOWNLOADABLE_PLUGIN__IS_DOWNLOADABLE";
        private const string RESPONSE_PLUGIN_FILE_NAME_HEADER_ID = "CHECK_DOWNLOADABLE_PLUGIN__PLUGIN_FILE_NAME";
        private const string RESPONSE_PLUGIN_EXECUTE_PATH_HEADER_ID = "CHECK_DOWNLOADABLE_PLUGIN__PLUGIN_EXECUTE_PATH";
        private const string RESPONSE_PLUGIN_MAIN_CLASS_NAME_HEADER_ID = "CHECK_DOWNLOADABLE_PLUGIN__PLUGIN_MAIN_CLASS_NAME";

        private const string REQUEST_DOWNLOAD_PLUGIN_HEADER_ID = "DOWNLOAD_PLUGIN";
        private const string REQUEST_DOWNLOAD_PLUGIN_KEY_HEADER_ID = "DOWNLOAD_PLUGIN__PLUGIN_KEY";
        private const string REQUEST_DOWNLOAD_PLUGIN_VERSION_HEADER_ID = "DOWNLOAD_PLUGIN__PLUGIN_VERSION";

        public PluginDownloadAndInstallManager()
        {
            _downloadPluginSemaphore = new SemaphoreSlim(1, 1);
            _installPluginSemaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<PluginVersionUD?> StartDownloadPlugin(HttpClient httpClient
            , string downloadPluginHeaderKey
            , string pluginDownloadAPIUri
            , string pluginKey
            , string pluginVersion
            , Action<double>? downloadProgressChangedCallback = null)
        {
            PluginVersionUD? result = null;
            var isContinue = await _downloadPluginSemaphore.WaitAsync(TIME_OUT_FOR_REQUEST_OF_SEMAPHORE);
            try
            {
                httpClient.DefaultRequestHeaders.Add(downloadPluginHeaderKey, REQUEST_CHECK_PLUGIN_DOWNLOADABLE_HEADER_ID);
                httpClient.DefaultRequestHeaders.Add(REQUEST_KEY_TO_CHECK_DOWNLOADABLE_HEADER_ID, pluginKey);
                httpClient.DefaultRequestHeaders.Add(REQUEST_VERSION_TO_CHECK_DOWNLOADABLE_HEADER_ID, pluginVersion);

                var response = await httpClient.GetAsync(pluginDownloadAPIUri);
                var isDownloadable = false;
                var fileName = "";
                var executePath = "";
                var mainClassName = "";
                try
                {
                    isDownloadable = response.Headers.GetValues(RESPONSE_IS_PLUGIN_DOWNLOADABLE_HEADER_ID)
                        .FirstOrDefault() == "1";
                    fileName = response.Headers.GetValues(RESPONSE_PLUGIN_FILE_NAME_HEADER_ID)
                        .FirstOrDefault();
                    executePath = response.Headers.GetValues(RESPONSE_PLUGIN_EXECUTE_PATH_HEADER_ID)
                        .FirstOrDefault();
                    mainClassName = response.Headers.GetValues(RESPONSE_PLUGIN_MAIN_CLASS_NAME_HEADER_ID)
                        .FirstOrDefault();
                }
                catch
                {
                    isDownloadable = false;
                }

                if (!isDownloadable 
                    || string.IsNullOrEmpty(fileName) 
                    || string.IsNullOrEmpty(executePath)
                    || string.IsNullOrEmpty(mainClassName))
                {
                    return null;
                }
                else
                {
                    var downloadFileFolder = (ExtensionManagerService
                            .Current
                            .ServiceManager?
                            .GetPluginsBaseFolderLocation() ?? "plugins")
                            + "\\" + pluginKey + "\\" + pluginVersion;
                    var versionExecutePath = executePath;
                    if (!Directory.Exists(downloadFileFolder))
                    {
                        Directory.CreateDirectory(downloadFileFolder);
                    }
                    var downloadFilePath = downloadFileFolder + "\\" + fileName;
                    var requestHeaderContentMap = new Dictionary<string, string>();
                    requestHeaderContentMap.Add(downloadPluginHeaderKey, REQUEST_DOWNLOAD_PLUGIN_HEADER_ID);
                    requestHeaderContentMap.Add(REQUEST_DOWNLOAD_PLUGIN_KEY_HEADER_ID, pluginKey);
                    requestHeaderContentMap.Add(REQUEST_DOWNLOAD_PLUGIN_VERSION_HEADER_ID, pluginVersion);
                    var param = new object[] { pluginDownloadAPIUri
                        , downloadFilePath
                        , requestHeaderContentMap};
                    var downloadTask = new DownloadPluginTask(param);
                    downloadTask.ProgressChanged += (s, e) =>
                    {
                        downloadProgressChangedCallback?.Invoke(e);
                    };
                    await downloadTask.Execute();

                    if (downloadTask.IsCompleted)
                    {
                        result = new PluginVersionUD();
                        result.Version = pluginVersion;
                        result.DownloadFilePath = downloadFilePath;
                        result.ExecutePath = versionExecutePath;
                        result.MainClassName = mainClassName;
                    }
                }

            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
            finally
            {
                _downloadPluginSemaphore.Release();
            }

            return result;
        }

        public async Task<bool> StartInstallPlugin(PluginVersionUD versionData)
        {
            var isContinue = await _installPluginSemaphore.WaitAsync(TIME_OUT_FOR_REQUEST_OF_SEMAPHORE);

            return true;
        }

    }
}
