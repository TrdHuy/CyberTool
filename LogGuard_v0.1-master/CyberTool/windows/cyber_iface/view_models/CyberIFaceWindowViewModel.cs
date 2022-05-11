using cyber_base.view_model;
using cyber_tool.services;
using cyber_tool.windows.cyber_iface.pages.page_controller;
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
        private CIFaceW_PageController _PageHost = CIFaceW_PageController.Current;
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
                    _PageHost.UpdateCurrentServiceVOByID(value.PageVO.CyberService.ServiceID);
                }
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object ServiceContent
        {
            get
            {
                if (_PageHost.CurrentServicePageOV.IsPageUnderconstruction)
                {
                    return new Underconstruction();
                }
                else
                {
                    _PageHost?.PreviousServicePageOV?.CyberService?.OnServiceUnloaded(_ServiceManager);

                    _PageHost?.CurrentServicePageOV?.CyberService?.OnPreServiceViewInit(_ServiceManager);

                    var content = _PageHost?.CurrentServicePageOV?.CyberService?.GetServiceView();

                    if (content != null)
                        _PageHost?.CurrentServicePageOV?.CyberService?.OnServiceViewInstantiated(_ServiceManager);

                    return content;
                }
            }
        }


        public CyberIFaceWindowViewModel()
        {
            InitPageHeaderItemSource();

            _PageHost.CurrentPageSourceChanged -= OnPageSourceChanged;
            _PageHost.CurrentPageSourceChanged += OnPageSourceChanged;

        }

        private void InitPageHeaderItemSource()
        {
            foreach (var vo in _PageHost.ServicePageVOsMap.Values)
            {
                if (vo != null)
                {
                    var vm = new CyberIFacePageHeaderItemViewModel(vo);
                    PageHeaderItems.Add(vm);
                    if (_PageHost.CurrentServicePageOV == vo)
                    {
                        _selectedHeaderItem = vm;
                    }
                }
            }
        }

        private void OnPageSourceChanged(object sender)
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
