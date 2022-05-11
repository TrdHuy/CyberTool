using cyber_base.view_model;
using cyber_tool.@base.page.model;
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
        public ServiceVO PageVO { get; private set; }

        public CyberIFacePageHeaderItemViewModel(ServiceVO vo)
        {
            PageVO = vo;
        }

        [Bindable(true)]
        public string Header
        {
            get
            {
                return PageVO.Header;
            }
        }

        [Bindable(true)]
        public string HeaderGeometryData
        {
            get
            {
                return PageVO.IconHeaderGeometryData;
            }
        }
    }
}
