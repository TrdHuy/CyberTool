using cyber_base.implement.utils;
using extension_manager_service.models;
using extension_manager_service.view_models.tabs.plugin_browser.items;
using extension_manager_service.views.elements.plugin_browser.items.@base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace extension_manager_service.implement.server_contact_manager.source_manager
{
    internal class BrowserTabSourceManager
    {
        private const int MAXIMUM_ITEM_PER_REQUEST = 12;
        private const int TIME_OUT_FOR_REQUEST_OF_SEMAPHORE = 100;
        private const string GET_PLUGIN_INFO_HEADER_ID = "GET_ALL_PLUGIN_DATA";
        private const string REQUEST_PLUGIN_INFO_MAXIMUM_AMOUNT_HEADER_ID = "GET_ALL_PLUGIN_DATA__MAXIMUM_AMOUNT";
        private const string REQUEST_PLUGIN_INFO_START_INDEX_HEADER_ID = "GET_ALL_PLUGIN_DATA__START_INDEX";
        private const string RESPONSE_PLUGIN_INFO_END_OF_DBSET_HEADER_ID = "GET_ALL_PLUGIN_DATA__IS_END_OF_DBSET";

        private RangeObservableCollection<IPluginItemViewHolderContext> _pluginItemSource;
        private List<PluginVO> _rawCache;

        private int _currentSourceCount = 0;
        private bool _isFullOfDbSet = false;
        private SemaphoreSlim _requestDataSemaphore;

        public bool IsFullOfDbSet { get => _isFullOfDbSet; }
        public RangeObservableCollection<IPluginItemViewHolderContext> PluginItemSource
        {
            get => _pluginItemSource;
        }

        public BrowserTabSourceManager()
        {
            _pluginItemSource = new RangeObservableCollection<IPluginItemViewHolderContext>();
            _rawCache = new List<PluginVO>();
            _requestDataSemaphore = new SemaphoreSlim(1, 1);
        }

        public int GetCurrentItemStartIndexForHttpRequest()
        {
            return _currentSourceCount;
        }

        public void ClearData()
        {
            _isFullOfDbSet = false;
            _pluginItemSource.Clear();
            _rawCache.Clear();
            _currentSourceCount = 0;
        }

        public async Task RequestServerData(HttpClient httpClient, string requestInfoHeaderKey, string uri)
        {
            var isContinue = await _requestDataSemaphore.WaitAsync(TIME_OUT_FOR_REQUEST_OF_SEMAPHORE);
            try
            {
                if (_isFullOfDbSet || !isContinue)
                {
                    return;
                }

                httpClient.DefaultRequestHeaders.Add(requestInfoHeaderKey, GET_PLUGIN_INFO_HEADER_ID);
                httpClient.DefaultRequestHeaders.Add(REQUEST_PLUGIN_INFO_MAXIMUM_AMOUNT_HEADER_ID, MAXIMUM_ITEM_PER_REQUEST + "");
                httpClient.DefaultRequestHeaders.Add(REQUEST_PLUGIN_INFO_START_INDEX_HEADER_ID, _currentSourceCount + "");

                var response = await httpClient.GetAsync(uri);

                try
                {
                    _isFullOfDbSet = response.Headers.GetValues(RESPONSE_PLUGIN_INFO_END_OF_DBSET_HEADER_ID)
                        .FirstOrDefault() == "1";
                }
                catch
                {
                    _isFullOfDbSet = false;
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var pluginSource = JsonConvert.DeserializeObject<List<PluginVO>>(responseContent);

                if (pluginSource != null)
                {
                    AddItemFromModel(pluginSource);
                }
                response.Dispose();
            }
            catch { }
            finally
            {
                _requestDataSemaphore.Release();
            }
        }

        private void AddItemFromModel(ICollection<PluginVO> vOSource)
        {
            _currentSourceCount += vOSource.Count;
            foreach (var plugin in vOSource)
            {
                var item = new PluginItemViewModel()
                {
                    PluginName = plugin.Name,
                    PluginAuthor = plugin.Author,
                    PluginDescription = plugin.Description,
                    Downloads = plugin.Downloads,
                    ProjectURL = plugin.ProjectURL,
                    IsAuthenticated = plugin.IsAuthenticated,
                };
                CheckCurrentPluginIsInstalled(item);
                CalculateRateForPlugin(item, plugin.Votes);
                InitIconSource(item, plugin.IconSource);
                InitTagItemForPlugin(item, plugin.Tags);
                InitVersionItemForPluginAsync(item, plugin.PluginVersions);
                _pluginItemSource.Add(item);
                _rawCache.Add(plugin);
            }
        }

        private void InitIconSource(PluginItemViewModel item, string iconSource)
        {
            try
            {
                item.IconSource = new Uri(iconSource);
            }
            catch
            {

            }
        }

        private void CheckCurrentPluginIsInstalled(PluginItemViewModel item)
        {
            ///TODO: Triển khai kiểm tra việc check plugin này đã đc cài đặt hay chưa
        }

        private async void InitVersionItemForPluginAsync(PluginItemViewModel item, ICollection<PluginVersionVO> pluginVersions)
        {
            item.VersionHistorySource = await GetVersionHistorySource(pluginVersions);
        }

        private async Task<FirstLastObservableCollection<IVersionHistoryItemViewHolderContext>> GetVersionHistorySource(ICollection<PluginVersionVO> pluginVersions)
        {
            await Task.Delay(100);
            var source = new FirstLastObservableCollection<IVersionHistoryItemViewHolderContext>();
            foreach (var item in pluginVersions)
            {
                source.Add(new VersionHistoryItemViewModel()
                {
                    Version = item.Version,
                    Description = item.Description,
                });
            }
            return source;
        }

        private async void InitTagItemForPlugin(PluginItemViewModel item, ICollection<TagVO> tags)
        {
            item.Tags = await GetTagItemSource(tags);
        }

        private async Task<string[]> GetTagItemSource(ICollection<TagVO> tags)
        {
            await Task.Delay(100);
            var source = new string[tags.Count];
            var i = 0;
            foreach (var tag in tags)
            {
                source[i++] = tag.Content;
            }
            return source;
        }

        private async void CalculateRateForPlugin(PluginItemViewModel item, ICollection<VoteVO> votes)
        {
            item.Rates = await GetPluginRate(votes);
        }

        private async Task<double> GetPluginRate(ICollection<VoteVO> votes)
        {
            await Task.Delay(3000);
            if (votes.Count > 0)
            {
                var rate = votes.Average(s => s.Stars);
                return rate;
            }
            return 1.5;
        }

    }
}
