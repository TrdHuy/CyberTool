using cyber_base.implement.utils;
using cyber_base.view_model;
using extension_manager_service.view_models.tabs.plugin_browser.items;
using extension_manager_service.views.elements.plugin_browser.items.@base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.view_models.tabs.plugin_browser
{
    internal class PluginBrowserTabViewModel : BaseViewModel
    {
        private RangeObservableCollection<IPluginItemViewHolderContext> _pluginItemSource;

        [Bindable(true)]
        public RangeObservableCollection<IPluginItemViewHolderContext> PluginItemSource
        {
            get => _pluginItemSource;
            set
            {
                _pluginItemSource = value;
                InvalidateOwn();
            }
        }

        public PluginBrowserTabViewModel(BaseViewModel parents) : base(parents)
        {
            _pluginItemSource = new RangeObservableCollection<IPluginItemViewHolderContext>();
            GenerateTestData();
        }


        private void GenerateTestData()
        {
            var rand = new Random();
            for(int i = 0; i< 100; i++)
            {
                var item = new PluginItemViewModel()
                {
                    PluginName = "Saber sw Publisher",
                    PluginAuthor = "huy.td1",
                    PluginDescription = "Provide a process to manage your dev work",
                    Downloads = 600,
                    Rates = rand.NextDouble() * 5,
                    IsInstalled = true,
                    IsAuthenticated = true,
                    IconSource = new Uri("http://107.127.131.89:8088/cyber_tool/services/extension_manager/publisher_icon.png"),
                    ViewMode = PluginItemViewMode.Full
                };

                _pluginItemSource.Add(item);
            }
        }

    }
}
