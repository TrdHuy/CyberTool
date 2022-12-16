using cyber_installer.implement.modules.server_contact_manager;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using cyber_installer.view.usercontrols.tabs.@base;
using cyber_installer.view.usercontrols.tabs;
using cyber_installer.view_models.tabs.available_tab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cyber_base.view_model;
using cyber_base.implement.utils;

namespace cyber_installer.view_models.tabs
{
    internal abstract class BaseSoftwareStatusTabViewModel : BaseViewModel, ISoftwareStatusTabContext
    {
        private bool _isLoading;

        protected Task? _requestDataTask;
        protected CancellationTokenSource? _requestDataTaskCancellationTokenSource;

        [Bindable(true)]
        public RangeObservableCollection<ItemViewModel> ItemsSource { get; set; } = new RangeObservableCollection<ItemViewModel>();

        [Bindable(true)]
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                InvalidateOwn();
            }
        }

        public abstract void OnTabOpened(BaseSoftwaresStatusTab sender);

        public abstract void OnScrollDownToBottom(object sender);

        public abstract void OnTabClosed(BaseSoftwaresStatusTab sender);

    }
}