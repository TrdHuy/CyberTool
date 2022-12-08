using cyber_base.implement.utils;
using cyber_base.view_model;
using cyber_core.services;
using cyber_core.windows.cyber_iface.view_models.page_header;
using cyber_core.windows.cyber_iface.views.usercontrols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_core.windows.cyber_iface.view_models
{
    public class CyberIFaceWindowViewModel : BaseViewModel
    {
        private SemaphoreSlim _pageHeaderItemsSourceSlim = new SemaphoreSlim(1, 1);
        private CyberServiceController _serviceController = CyberServiceController.Current;
        private CyberServiceManager _serviceManager = CyberServiceManager.Current;
        private CyberIFacePageHeaderItemViewModel? _selectedHeaderItem = null;

        [Bindable(true)]
        public RangeObservableCollection<CyberIFacePageHeaderItemViewModel> PageHeaderItems { get; set; }
            = new RangeObservableCollection<CyberIFacePageHeaderItemViewModel>();

        [Bindable(true)]
        public CyberIFacePageHeaderItemViewModel? SelectedHeaderItem
        {
            get
            {
                return _selectedHeaderItem;
            }
            set
            {

                if (value != null && value.IsService && value.Service != null)
                {
                    if (IsShouldChangePage(_selectedHeaderItem, value))
                    {
                        _selectedHeaderItem = value;
                        _serviceController.UpdateCurrentServiceByID(value.Service.ServiceID);
                    }
                }
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object? ServiceContent
        {
            get
            {
                return _serviceController.CurrentServiceView;
            }
        }


        public CyberIFaceWindowViewModel()
        {
            InitServiceHeaderItemSource();
            _serviceController.ServiceChanged -= OnServiceChanged;
            _serviceController.ServiceChanged += OnServiceChanged;
            _serviceManager.ExtensionServiceMapperCollectionChanged -= HandleExtensionServiceCollectionChanged;
            _serviceManager.ExtensionServiceMapperCollectionChanged += HandleExtensionServiceCollectionChanged;
        }

        private async void HandleExtensionServiceCollectionChanged(object sender, ExtensionServiceMapperCollectionChangedEventArgs args)
        {
            await _pageHeaderItemsSourceSlim.WaitAsync();
            try
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    if (args.NewService != null)
                    {
                        var vm = new CyberIFacePageHeaderItemViewModel(args.NewService);
                        PageHeaderItems.Add(vm);
                    }
                }
                else if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    if (args.OldService != null)
                    {
                        foreach (var vm in PageHeaderItems)
                        {
                            if (vm.Service == args.OldService)
                            {
                                PageHeaderItems.Remove(vm);
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            finally
            {
                _pageHeaderItemsSourceSlim.Release();
            }
        }

        private void InitServiceHeaderItemSource()
        {
            try
            {
                _pageHeaderItemsSourceSlim.Wait();
                foreach (var vo in _serviceManager.CyberServiceMaper.Values)
                {
                    if (vo != null)
                    {
                        var vm = new CyberIFacePageHeaderItemViewModel(vo);
                        PageHeaderItems.Add(vm);
                        if (_serviceController.CurrentService == vo)
                        {
                            _selectedHeaderItem = vm;
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                _pageHeaderItemsSourceSlim.Release();
            }
        }

        private void OnServiceChanged(object sender, CyberServiceController.ServiceEventArgs args)
        {
            Invalidate("ServiceContent");
        }

        private bool IsShouldChangePage(CyberIFacePageHeaderItemViewModel? oldValue
            , CyberIFacePageHeaderItemViewModel? newValue)
        {
            return true;
        }

        public override void OnDestroy()
        {
            _serviceController.ServiceChanged -= OnServiceChanged;
            _serviceManager.ExtensionServiceMapperCollectionChanged -= HandleExtensionServiceCollectionChanged;
            _pageHeaderItemsSourceSlim.Dispose();
            base.OnDestroy();
        }
    }
}
