using cyber_installer.@base.http_requester;
using cyber_installer.implement.modules.ui_event_handler.async_task;
using cyber_installer.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.sw_installing_manager.http_requester
{
    internal class SwDownloadRequester : BaseHttpRequester
    {
        public const string DOWNLOAD_TOOL_API_PATH = "/downloadtool";
        public const string REQUEST_DOWNLOAD_TOOL_HEADER_KEY = "h2sw-download-tool";
        public const string REQUEST_CHECK_TOOL_DOWNLOADABLE_HEADER_ID = "CHECK_DOWNLOADABLE_TOOL";
        public const string REQUEST_KEY_TO_CHECK_DOWNLOADABLE_HEADER_ID = "CHECK_DOWNLOADABLE_TOOL__TOOL_KEY";
        public const string REQUEST_VERSION_TO_CHECK_DOWNLOADABLE_HEADER_ID = "CHECK_DOWNLOADABLE_TOOL__TOOL_VERSION";
        public const string RESPONSE_IS_TOOL_DOWNLOADABLE_HEADER_ID = "CHECK_DOWNLOADABLE_TOOL__IS_DOWNLOADABLE";
        public const string RESPONSE_TOOL_FILE_NAME_HEADER_ID = "CHECK_DOWNLOADABLE_TOOL__TOOL_FILE_NAME";
        public const string RESPONSE_TOOL_EXECUTE_PATH_HEADER_ID = "CHECK_DOWNLOADABLE_TOOL__TOOL_EXECUTE_PATH";

        public const string REQUEST_DOWNLOAD_TOOL_HEADER_ID = "DOWNLOAD_TOOL";
        public const string REQUEST_DOWNLOAD_TOOL_KEY_HEADER_ID = "DOWNLOAD_TOOL__TOOL_KEY";
        public const string REQUEST_DOWNLOAD_TOOL_VERSION_HEADER_ID = "DOWNLOAD_TOOL__TOOL_VERSION";


        private const int TIME_OUT_FOR_REQUEST_OF_SEMAPHORE = 100;
        private SemaphoreSlim _requestDownloadToolSemaphore;

        public SwDownloadRequester()
        {
            _requestDownloadToolSemaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<ToolData?> RequestDownloadSoftwareWithLatestVersion(HttpClient httpClient
            , ToolVO requestingTool)
        {
            var isContinue = await _requestDownloadToolSemaphore.WaitAsync(TIME_OUT_FOR_REQUEST_OF_SEMAPHORE);
            var downLoadResult = new ToolData()
            {
                ToolName = requestingTool.Name
            };
            try
            {
                if (!isContinue)
                {
                    return null;
                }

                var requestToolKey = requestingTool.StringId;
                var requestToolVersion = requestingTool.ToolVersions.Last().Version.Trim();

                httpClient.DefaultRequestHeaders.Add(REQUEST_DOWNLOAD_TOOL_HEADER_KEY, REQUEST_CHECK_TOOL_DOWNLOADABLE_HEADER_ID);
                httpClient.DefaultRequestHeaders.Add(REQUEST_KEY_TO_CHECK_DOWNLOADABLE_HEADER_ID, requestToolKey);
                httpClient.DefaultRequestHeaders.Add(REQUEST_VERSION_TO_CHECK_DOWNLOADABLE_HEADER_ID, requestToolVersion);

                var response = await httpClient.GetAsync(REMOTE_ADDRESS + DOWNLOAD_TOOL_API_PATH);
                var responseContent = await response.Content.ReadAsStringAsync();

                var isDownloadable = false;
                var fileName = "";
                var executePath = "";
                try
                {
                    isDownloadable = response.Headers.GetValues(RESPONSE_IS_TOOL_DOWNLOADABLE_HEADER_ID)
                        .FirstOrDefault() == "1";
                    fileName = response.Headers.GetValues(RESPONSE_TOOL_FILE_NAME_HEADER_ID)
                        .FirstOrDefault();
                    executePath = response.Headers.GetValues(RESPONSE_TOOL_EXECUTE_PATH_HEADER_ID)
                        .FirstOrDefault() ?? "";
                }
                catch
                {
                    isDownloadable = false;
                }

                if (!isDownloadable)
                {
                    downLoadResult = null;
                }
                else
                {
                    var downloadFileFolder = GetToolDownloadFileFolder(requestToolKey, requestToolVersion);
                    var versionExecutePath = downloadFileFolder + "\\" + executePath;
                    if (!Directory.Exists(downloadFileFolder))
                    {
                        Directory.CreateDirectory(downloadFileFolder);
                    }
                    var downloadFilePath = downloadFileFolder + "\\" + fileName;
                    var requestHeaderContentMap = new Dictionary<string, string>();
                    requestHeaderContentMap.Add(REQUEST_DOWNLOAD_TOOL_HEADER_KEY
                        , REQUEST_DOWNLOAD_TOOL_HEADER_ID);
                    requestHeaderContentMap.Add(REQUEST_DOWNLOAD_TOOL_KEY_HEADER_ID
                        , requestToolKey);
                    requestHeaderContentMap.Add(REQUEST_DOWNLOAD_TOOL_VERSION_HEADER_ID
                        , requestToolVersion);
                    var param = new object[] { REMOTE_ADDRESS + DOWNLOAD_TOOL_API_PATH
                                    , downloadFilePath
                                    , requestHeaderContentMap};
                    var downloadTask = new DownloadSoftwareTask(param);
                    downloadTask.ProgressChanged += (s, e2) =>
                    {
                    };
                    await downloadTask.Execute();

                    if (downloadTask.IsCompleted)
                    {
                        downLoadResult.ToolKey = requestToolKey;
                        var toolVersionData = new ToolVersionData()
                        {
                            Version = requestToolVersion,
                            DownloadFilePath = downloadFilePath,
                            ExecutePath = executePath,
                            VersionStatus = ToolVersionStatus.VersionDownloadedButWithoutInstalled,
                        };
                        downLoadResult.ToolVersionSource.Add(toolVersionData);
                        downLoadResult.ToolStatus = ToolStatus.Downloaded;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
            finally
            {
                _requestDownloadToolSemaphore.Release();
            }
            return downLoadResult;
        }

    }
}
