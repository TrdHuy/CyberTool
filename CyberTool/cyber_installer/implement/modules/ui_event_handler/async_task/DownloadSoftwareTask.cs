using cyber_base.async_task;
using cyber_base.implement.utils;
using cyber_installer.@base.async_task;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.ui_event_handler.async_task
{
    internal class DownloadSoftwareTask : AbsParamAsyncTask
    {
        private Logger _logger = new Logger("DownloadSoftwareTask", "cyber_installer");

        private string _sourceToDownload;
        private string? _iconSource;
        private string _filePathDestination;
        private Uri _sourceUri;
        private Uri? _iconSourceUri = null;
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
                        _iconSource = data[3]?.ToString();
                        _iconSourceDownloadedCallback = data[4] as Action<string>;
                    }
                    break;
                default:
                    throw new InvalidDataException("Param must be an array with 3 elements");
            }
            _isEnableAutomaticallyReport = false;
        }

        protected override async Task DoAsyncMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            if (_sourceUri != null && _filePathDestination != null && _requestHeaderContentMap != null)
            {
                // Download software from server and write it in a specific folder
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                using (WebClient client = new WebClient())
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                {
                    CurrentProgress = 0;

                    //Setup request header
                    foreach (var kv in _requestHeaderContentMap)
                    {
                        client.Headers.Add(kv.Key, kv.Value);
                    }

                    using (var stream = await client.OpenReadTaskAsync(_sourceUri))
                    {
                        var totalBytes = Int32.Parse(client.ResponseHeaders?[HttpResponseHeader.ContentLength] ?? "0");
                        var buffer = new byte[totalBytes];
                        int read = 0;
                        int total = 0;
                        int received = 0;
                        float percentage = 0;
                        var receivedBytes = 0;
                        using (var fileStream = File.Create(_filePathDestination))
                        {
                            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, read);
                                receivedBytes += read;
                                received = unchecked((int)receivedBytes);
                                total = unchecked((int)totalBytes);
                                percentage = ((float)received) / total;
                                CurrentProgress = percentage * 80;
                                await Task.Delay(100);
                            }
                        }

                        stream.Close();
                    }
                }

                // Download icon of software from server and write it in a specific folder
                if(_iconSourceUri != null)
                {
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
                    CurrentProgress = 100;
                    await Task.Delay(300);
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
                if (!string.IsNullOrEmpty(_iconSource))
                {
                    _iconSourceUri = new Uri(_iconSource);
                }
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
