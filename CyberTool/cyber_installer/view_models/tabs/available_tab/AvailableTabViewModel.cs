using cyber_base.implement.utils;
using cyber_base.view_model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using cyber_installer.view.usercontrols.tabs;
using cyber_installer.view.usercontrols.tabs.@base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.view_models.tabs.available_tab
{
    internal class AvailableTabViewModel : BaseViewModel, IAvailableTabContext
    {
        [Bindable(true)]
        public RangeObservableCollection<AvailableItemViewModel> ItemsSource { get; set; } = new RangeObservableCollection<AvailableItemViewModel>();


        public void OnLoaded(AvailableSoftwaresTab sender)
        {
            // TODO: Load data from server here
            ItemsSource.Clear();

            var testItem = new AvailableItemViewModel()
            {
                SoftwareName = "Cyber tool",
                Version = "3.0.0.0",
                ItemStatus = ItemStatus.Downloadable,
                IconSource = new Uri("https://cdn.icon-icons.com/icons2/3191/PNG/512/cyclone_weather_world_time_icon_194253.png")
            };
            ItemsSource.Add(testItem);
        }

        public void OnUnloaded(AvailableSoftwaresTab sender)
        {
        }
    }
}
