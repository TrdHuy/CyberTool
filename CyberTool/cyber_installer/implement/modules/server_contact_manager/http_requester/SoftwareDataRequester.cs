using cyber_base.implement.utils;
using cyber_installer.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cyber_installer.@base.http_requester;

namespace cyber_installer.implement.modules.server_contact_manager.http_requester
{
    internal class SoftwareDataRequester : BaseHttpRequester<IEnumerable?>
    {

        private const string REQUEST_INFO_API_PATH = "/requestinfo";
        private const string REQUEST_INFO_HEADER_KEY = "h2sw-request-info";

        private const int MAXIMUM_ITEM_PER_REQUEST = 12;
        private const int TIME_OUT_FOR_REQUEST_OF_SEMAPHORE = 100;
        private const string REQUEST_SOFTWARE_INFO_HEADER_ID = "GET_ALL_SOFTWARE_DATA";
        private const string REQUEST_SOFTWARE_INFO_MAXIMUM_AMOUNT_HEADER_ID = "GET_ALL_SOFTWARE_DATA__MAXIMUM_AMOUNT";
        private const string REQUEST_SOFTWARE_INFO_START_INDEX_HEADER_ID = "GET_ALL_SOFTWARE_DATA__START_INDEX";
        private const string RESPONSE_SOFTWARE_INFO_END_OF_DBSET_HEADER_ID = "GET_ALL_SOFTWARE_DATA__IS_END_OF_DBSET";

        private bool _isFullOfDbSet = false;
        private SemaphoreSlim _requestDataSemaphore;
        private int _currentRequestIndex = 0;

        public SoftwareDataRequester()
        {
            _requestDataSemaphore = new SemaphoreSlim(1, 1);
        }

        public bool IsFullOfDbSet { get => _isFullOfDbSet; }

        public void Refresh()
        {
            _isFullOfDbSet = false;
            _currentRequestIndex = 0;
        }

        public override async Task<IEnumerable?> Request(params object[] param)
        {
            try
            {
                var httpClient = param[0] as HttpClient ?? throw new ArgumentNullException();
                var cancellationToken = (CancellationToken)param[1];
                return await RequestServerData(httpClient
                    , cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        private async Task<IEnumerable?> RequestServerData(HttpClient httpClient
            , CancellationToken cancellationToken)
        {
            var isContinue = await _requestDataSemaphore.WaitAsync(TIME_OUT_FOR_REQUEST_OF_SEMAPHORE
                , cancellationToken);
            var toolSource = new List<ToolVO>();
            try
            {
                if (_isFullOfDbSet || !isContinue)
                {
                    return null;
                }

                httpClient.DefaultRequestHeaders.Add(REQUEST_INFO_HEADER_KEY, REQUEST_SOFTWARE_INFO_HEADER_ID);
                httpClient.DefaultRequestHeaders.Add(REQUEST_SOFTWARE_INFO_MAXIMUM_AMOUNT_HEADER_ID, MAXIMUM_ITEM_PER_REQUEST + "");
                httpClient.DefaultRequestHeaders.Add(REQUEST_SOFTWARE_INFO_START_INDEX_HEADER_ID, _currentRequestIndex + "");

                var response = await httpClient.GetAsync(GetRemoteAddress() + REQUEST_INFO_API_PATH, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                try
                {
                    _isFullOfDbSet = response.Headers.GetValues(RESPONSE_SOFTWARE_INFO_END_OF_DBSET_HEADER_ID)
                        .FirstOrDefault() == "1";
                }
                catch
                {
                    _isFullOfDbSet = false;
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                toolSource = JsonHelper
                    .DeserializeObject<List<ToolVO>>(responseContent) ?? toolSource;

                _currentRequestIndex += toolSource.Count;

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
            return toolSource;
        }

    }
}
