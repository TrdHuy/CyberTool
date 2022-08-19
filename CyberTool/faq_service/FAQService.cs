using cyber_base.implement.service;
using cyber_base.service;
using faq_service.definitions;
using faq_service.views.usercontrols;
using System;

namespace faq_service
{
    public class FAQService : AbstractCyberService
    {
        public static FAQService Current { get; private set; }

        public override string ServiceID { get; protected set; }

        public override long ServicePageLoadingDelayTime { get; protected set; }

        public override string HeaderGeometryData { get; protected set; }

        public override string Header { get; protected set; }

        public override bool IsUnderconstruction => true;

        public override Uri ServiceResourceUri { get; protected set; }

        static FAQService()
        {
            Current = new FAQService();
        }

        public FAQService()
        {
            Header = "FAQ";
            HeaderGeometryData = FAQServiceDefinition.FAQ_SERVICE_PAGE_HEADER_GEOMETRY_DATA;
            ServiceID = FAQServiceDefinition.FAQ_SERVICE_PAGE_URI_ORIGINAL_STRING;
            ServicePageLoadingDelayTime = FAQServiceDefinition.FAQ_SERVICE_PAGE_LOADING_DELAY_TIME;
            ServiceResourceUri = new Uri("pack://application:,,,/faq_service;component/themes/Themes.xaml",
                    UriKind.Absolute);
        }

        public override void OnServiceCreate(ICyberServiceManager cyberServiceManager)
        {
            base.OnServiceCreate(cyberServiceManager);
        }

        public override void OnPreServiceViewInit(ICyberServiceManager cyberServiceManager)
        {
        }

        public override void OnServiceViewInstantiated(ICyberServiceManager cyberServiceManager)
        {
        }

        public override void OnServiceViewLoaded(ICyberServiceManager cyberServiceManager)
        {
            base.OnServiceViewLoaded(cyberServiceManager);
        }

        public override void OnServiceUnloaded(ICyberServiceManager cyberServiceManager)
        {
            base.OnServiceUnloaded(cyberServiceManager);
        }

        protected override object? GenerateServiceView()
        {
            return new FAQServiceView();
        }
    }
}
