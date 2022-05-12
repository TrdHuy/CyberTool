using cyber_base.service;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_tool.windows.cyber_iface.view_models.page_header
{
    public class CyberIFacePageHeaderItemViewModel : BaseViewModel
    {
        public ICyberService Service { get; private set; }

        public CyberIFacePageHeaderItemViewModel(ICyberService service)
        {
            Service = service;
        }

        [Bindable(true)]
        public string Header
        {
            get
            {
                return Service.Header;
            }
        }

        [Bindable(true)]
        public string HeaderGeometryData
        {
            get
            {
                return Service.HeaderGeometryData;
            }
        }
    }
}
