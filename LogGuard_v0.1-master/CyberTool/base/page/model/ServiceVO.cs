using cyber_base.service;
using System;

namespace cyber_tool.@base.page.model
{
    public class ServiceVO
    {
        public ICyberService CyberService { get; }
        public object ServiceContent { get; }
        public bool IsPageUnderconstruction { get; set; }
        public string Header { get; set; }
        public string IconHeaderGeometryData { get; set; }
        public long LoadingDelayTime { get; set; }

        public ServiceVO(ICyberService service
            , object serviceContent
            , string header
            , string iconHeaderGeometryData
            , long delayTime = 2000)
        {
            ServiceContent = serviceContent;
            LoadingDelayTime = delayTime;
            CyberService = service;
            Header = header;
            IconHeaderGeometryData = iconHeaderGeometryData;
        }
    }
}
