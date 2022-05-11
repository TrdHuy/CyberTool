using cyber_tool._definitions;
using cyber_tool.windows.cyber_iface.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cyber_tool.services;
using System.Collections.ObjectModel;
using cyber_tool.@base.page.model;
using cyber_tool.@base.page.page_controller;
using cyber_tool.windows.cyber_iface.views.usercontrols;

namespace cyber_tool.windows.cyber_iface.pages.page_controller
{
    public class CIFaceW_PageController : BasePageController
    {
        private CyberServiceManager _CyberServiceManager = CyberServiceManager.Current;

        public Dictionary<string, ServiceVO> ServicePageVOsMap { get; private set; }

        public override event CurrentPageSourceChangedHandler CurrentPageSourceChanged;

        public static CIFaceW_PageController Current
        {
            get
            {
                return CIFaceW_WindowModuleController.CIFW_PC_Instance;
            }
        }

        private CIFaceW_PageController()
        {
            ServicePageVOsMap = new Dictionary<string, ServiceVO>();

            // load services page
            foreach (var service in _CyberServiceManager.CyberServicesCollection)
            {
                var pageVO = new ServiceVO(
                    service,
                    null,
                    service.Header,
                    service.HeaderGeometryData,
                    service.ServicePageLoadingDelayTime);
                pageVO.IsPageUnderconstruction = service.IsUnderconstruction;
                ServicePageVOsMap.Add(service.ServiceID.ToString(), pageVO);
            }

            PreviousServicePageOV = null;
            if (ServicePageVOsMap.Count > 0)
            {
                CurrentServicePageOV = ServicePageVOsMap.Values.ElementAt(0);
            }
            
        }

        public override void UpdateCurrentServiceVOByID(string id)
        {
            PreviousServicePageOV = CurrentServicePageOV;
            CurrentServicePageOV = ServicePageVOsMap[id] ??= CurrentServicePageOV;

            CurrentPageSourceChanged?.Invoke(this);
        }

    }
}