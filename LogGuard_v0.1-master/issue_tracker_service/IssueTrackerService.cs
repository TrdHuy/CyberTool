using cyber_base.implement.service;
using cyber_base.service;
using issue_tracker_service.definitions;
using issue_tracker_service.views.usercontrols;
using System;

namespace issue_tracker_service
{
    public class IssueTrackerService : AbstractCyberService
    {
        public static IssueTrackerService Current { get; private set; }

        public override string ServiceID { get; protected set; }

        public override long ServicePageLoadingDelayTime { get; protected set; }

        public override string HeaderGeometryData { get; protected set; }

        public override string Header { get; protected set; }

        public override bool IsUnderconstruction => true;

        public override Uri ServiceResourceUri { get; protected set; }

        static IssueTrackerService()
        {
            Current = new IssueTrackerService();
        }

        public IssueTrackerService()
        {
            Header = "Service";
            HeaderGeometryData = IssueTrackerServiceDefinition.SERVICE_PAGE_HEADER_GEOMETRY_DATA;
            ServiceID = IssueTrackerServiceDefinition.SERVICE_PAGE_URI_ORIGINAL_STRING;
            ServicePageLoadingDelayTime = IssueTrackerServiceDefinition.SERVICE_PAGE_LOADING_DELAY_TIME;
            ServiceResourceUri = new Uri("pack://application:,,,/issue_tracker_service;component/themes/Themes.xaml",
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
            return new IssueTrackerServiceView();
        }
    }
}
