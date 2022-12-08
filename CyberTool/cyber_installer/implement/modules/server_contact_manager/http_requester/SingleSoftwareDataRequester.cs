using cyber_base.implement.utils;
using cyber_installer.@base.http_requester;
using cyber_installer.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.server_contact_manager.http_requester
{
    internal class SingleSoftwareDataRequester : BaseHttpRequester<ToolVO?>
    {
        private const string REQUEST_INFO_API_PATH = "/requestinfo";
        private const string REQUEST_INFO_HEADER_KEY = "h2sw-request-info";

        private const string REQUEST_SOFTWARE_INFO_HEADER_ID = "GET_SOFTWARE_DATA";
        private const string REQUEST_SOFTWARE_KEY_HEADER_ID = "GET_SOFTWARE_DATA__SOFTWARE_KEY";

        private const int TIME_OUT_FOR_REQUEST_OF_SEMAPHORE = 100;
        private SemaphoreSlim _requestDataSemaphore;
        private string _swKey;

        public SingleSoftwareDataRequester()
        {
            _requestDataSemaphore = new SemaphoreSlim(1, 1);
        }

        public override async Task<ToolVO?> Request(params object[] param)
        {
            try
            {
                var httpClient = param[0] as HttpClient ?? throw new ArgumentNullException();
                var cancellationToken = (CancellationToken)param[1];
                _swKey = param[2]?.ToString() ?? "";
                return await RequestServerData(httpClient
                    , cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        private async Task<ToolVO?> RequestServerData(HttpClient httpClient
            , CancellationToken cancellationToken)
        {
            var isContinue = await _requestDataSemaphore.WaitAsync(TIME_OUT_FOR_REQUEST_OF_SEMAPHORE
                , cancellationToken);
            ToolVO? result = null;
            try
            {
                if (!isContinue)
                {
                    return null;
                }

                httpClient.DefaultRequestHeaders.Add(REQUEST_INFO_HEADER_KEY, REQUEST_SOFTWARE_INFO_HEADER_ID);
                httpClient.DefaultRequestHeaders.Add(REQUEST_SOFTWARE_KEY_HEADER_ID, _swKey);

                var response = await httpClient.GetAsync(GetRemoteAddress() + REQUEST_INFO_API_PATH, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                result = JsonHelper
                    .DeserializeObject<ToolVO>(responseContent);

                response.Dispose();
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
            finally
            {
                _requestDataSemaphore.Release();
            }
            return result;
        }

    }
}
