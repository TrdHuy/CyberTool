using cyber_base.async_task;
using cyber_installer.@base.async_task;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.ui_event_handler.async_task
{
    internal class DownloadSoftwareTask : AbsParamAsyncTask
    {
        private string _sourceToDownload;
        private string _filePathDestination;
        private Uri _sourceUri;
        private Dictionary<string, string> _requestHeaderContentMap;

#pragma warning disable CS8618
        public DownloadSoftwareTask(object param
#pragma warning restore CS8618 
        , Action<AsyncTaskResult>? callback = null
        , string name = "Downloading") : base(param, name, callback)
        {
            switch (param)
            {
                case object[] data:
                    if (data.Length == 3)
                    {
                        _sourceToDownload = data[0].ToString()
                            ?? throw new ArgumentNullException("Source to download not found in params!");
                        _filePathDestination = data[1].ToString()
                            ?? throw new ArgumentNullException("File path destination not found in params!");
                        _requestHeaderContentMap = data[2] as Dictionary<string, string>
                            ?? throw new ArgumentNullException("Request header content map not found in params!");
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
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }

}
