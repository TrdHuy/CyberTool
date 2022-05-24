using cyber_base.implement.service;
using cyber_base.service;
using honeyboard_release_service.definitions;
using honeyboard_release_service.views.usercontrols;
using System;

namespace honeyboard_release_service
{
    public class HoneyboardReleaseService : AbstractCyberService
    {
        public static HoneyboardReleaseService Current { get; private set; }

        public override string ServiceID { get; protected set; }

        public override long ServicePageLoadingDelayTime { get; protected set; }

        public override string HeaderGeometryData { get; protected set; }

        public override string Header { get; protected set; }

        public override bool IsUnderconstruction => false;

        public override Uri ServiceResourceUri { get; protected set; }

        static HoneyboardReleaseService()
        {
            Current = new HoneyboardReleaseService();
        }

        public HoneyboardReleaseService()
        {
            Header = "HBD Release";
            HeaderGeometryData = ServiceDefinition.SERVICE_PAGE_HEADER_GEOMETRY_DATA;
            ServiceID = ServiceDefinition.SERVICE_PAGE_URI_ORIGINAL_STRING;
            ServicePageLoadingDelayTime = ServiceDefinition.SERVICE_PAGE_LOADING_DELAY_TIME;
            ServiceResourceUri = new Uri("pack://application:,,,/honeyboard_release_service;component/themes/Themes.xaml",
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
            return new ServiceView();
        }
    }
}
