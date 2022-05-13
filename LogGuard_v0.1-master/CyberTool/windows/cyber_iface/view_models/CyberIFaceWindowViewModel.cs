using cyber_base.view_model;
using cyber_tool.services;
using cyber_tool.windows.cyber_iface.view_models.page_header;
using cyber_tool.windows.cyber_iface.views.usercontrols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_tool.windows.cyber_iface.view_models
{
    public class CyberIFaceWindowViewModel : BaseViewModel
    {
        private CyberServiceController _ServiceController = CyberServiceController.Current;
        private CyberServiceManager _ServiceManager = CyberServiceManager.Current;
        private CyberIFacePageHeaderItemViewModel _selectedHeaderItem = null;


        [Bindable(true)]
        public ObservableCollection<CyberIFacePageHeaderItemViewModel> PageHeaderItems { get; set; } = new ObservableCollection<CyberIFacePageHeaderItemViewModel>();

        [Bindable(true)]
        public CyberIFacePageHeaderItemViewModel SelectedHeaderItem
        {
            get
            {
                return _selectedHeaderItem;
            }
            set
            {
                if (IsShouldChangePage(_selectedHeaderItem, value))
                {
                    _selectedHeaderItem = value;
                    _ServiceController.UpdateCurrentServiceByID(value.Service.ServiceID);
                }
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object ServiceContent
        {
            get
            {
                return _ServiceController.CurrentServiceView;
            }
        }


        public CyberIFaceWindowViewModel()
        {
            InitServiceHeaderItemSource();
            _ServiceController.ServiceChanged -= OnServiceChanged;
            _ServiceController.ServiceChanged += OnServiceChanged;
        }

        

        private void InitServiceHeaderItemSource()
        {
            foreach (var vo in _ServiceManager.CyberServiceMaper.Values)
            {
                if (vo != null)
                {
                    var vm = new CyberIFacePageHeaderItemViewModel(vo);
                    PageHeaderItems.Add(vm);
                    if (_ServiceController.CurrentService == vo)
                    {
                        _selectedHeaderItem = vm;
                    }
                }
            }
        }

        private void OnServiceChanged(object sender, CyberServiceController.ServiceEventArgs args)
        {
            Invalidate("ServiceContent");
        }

        private bool IsShouldChangePage(CyberIFacePageHeaderItemViewModel oldValue
            , CyberIFacePageHeaderItemViewModel newValue)
        {
            return true;
        }
    }
}
