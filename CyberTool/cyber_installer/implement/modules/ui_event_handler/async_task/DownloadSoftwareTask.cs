using cyber_base.async_task;
using cyber_base.implement.utils;
using cyber_installer.@base.async_task;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.ui_event_handler.async_task
{
    internal class DownloadSoftwareTask : AbsParamAsyncTask
    {
        private Logger _logger = new Logger("DownloadSoftwareTask", "cyber_installer");

        private string _sourceToDownload;
        private string _iconSource;
        private string _filePathDestination;
        private Uri _sourceUri;
        private Uri _iconSourceUri;
        private Dictionary<string, string> _requestHeaderContentMap;
        private Action<string>? _iconSourceDownloadedCallback;

#pragma warning disable CS8618
        public DownloadSoftwareTask(object param
#pragma warning restore CS8618 
        , Action<AsyncTaskResult>? callback = null
        , string name = "Downloading") : base(param, name, callback)
        {
            switch (param)
            {
                case object[] data:
                    if (data.Length == 5)
                    {
                        _sourceToDownload = data[0].ToString()
                            ?? throw new ArgumentNullException("Source to download not found in params!");
                        _filePathDestination = data[1].ToString()
                            ?? throw new ArgumentNullException("File path destination not found in params!");
                        _requestHeaderContentMap = data[2] as Dictionary<string, string>
                            ?? throw new ArgumentNullException("Request header content map not found in params!");
                        _iconSource = data[3].ToString()
                            ?? throw new ArgumentNullException("Icon source not found in params!");
                        _iconSourceDownloadedCallback = data[4] as Action<string>;
                    }
                    break;
                default:
                    throw new InvalidDataException("Param must be an array with 3 elements");
            }
            _isEnableReport = false;
        }

        protected override async Task DoAsyncMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            if (_sourceUri != null && _filePathDestination != null && _requestHeaderContentMap != null)
            {
#pragma warning disable SYSLIB0014
                using (WebClient myWebClient = new WebClient())
#pragma warning restore SYSLIB0014
                {
                    foreach (var kv in _requestHeaderContentMap)
                    {
                        myWebClient.Headers.Add(kv.Key, kv.Value);
                    }

                    myWebClient.DownloadProgressChanged += (s, e) =>
                    {
                        var rate = e.BytesReceived / e.TotalBytesToReceive * 100;
                        CurrentProgress = rate;
                    };
                    await myWebClient.DownloadFileTaskAsync(_sourceUri, _filePathDestination);
                }


                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(_iconSourceUri);
                    var responseContent = await response.Content.ReadAsByteArrayAsync();
                    var iconPath = Path.GetDirectoryName(_filePathDestination) + "\\" + "iconres";
                    await File.WriteAllBytesAsync(iconPath, responseContent);
                    if (_iconSourceDownloadedCallback != null)
                    {
                        _iconSourceDownloadedCallback.Invoke(iconPath);
                    }

                }


            }

        }
        protected override bool IsTaskPossible(object param)
        {
            if (_sourceToDownload == null)
            {
                return false;
            }

            try
            {
                _sourceUri = new Uri(_sourceToDownload);
                _iconSourceUri = new Uri(_iconSource);
            }
            catch (Exception ex)
            {
                _logger.E("Fail to parse URI " + ex.Message);
                return false;
            }
            return true;
        }
    }

}
