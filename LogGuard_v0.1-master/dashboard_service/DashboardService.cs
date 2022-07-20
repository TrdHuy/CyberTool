using cyber_base.implement.service;
using cyber_base.service;
using dashboard_service.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dashboard_service
{
    public class DashboardService : AbstractCyberService
    {
        public static DashboardService Current { get; private set; }

        public override string ServiceID { get; protected set; }

        public override long ServicePageLoadingDelayTime { get; protected set; }

        public override string HeaderGeometryData { get; protected set; }

        public override string Header { get; protected set; }

        public override bool IsUnderconstruction => true;

        public override Uri ServiceResourceUri { get; protected set; }

        static DashboardService()
        {
            Current = new DashboardService();
        }

        public DashboardService()
        {
            Header = "Dashboard";
            HeaderGeometryData = "M46.14,38.13A24.66,24.66,0,0,0,50,24.86a25,25,0,0,0-50,0A24.66,24.66,0,0,0,3.86,38.13Zm2.24-15.52a2.77,2.77,0,1,1-2.77-2.77A2.78,2.78,0,0,1,48.38,22.61Zm-5.86-11.9a2.78,2.78,0,1,1-2.77,2.77A2.77,2.77,0,0,1,42.52,10.71ZM35.43,3.83A2.77,2.77,0,1,1,32.65,6.6,2.77,2.77,0,0,1,35.43,3.83ZM25.11.74a2.78,2.78,0,1,1-2.78,2.77A2.77,2.77,0,0,1,25.11.74ZM25,17.42h.15l2.6-5A3.6,3.6,0,0,1,32.59,11h0A3.61,3.61,0,0,1,34.1,15.8l-2.58,4.9A8.11,8.11,0,1,1,25,17.42ZM14.85,3.83A2.77,2.77,0,1,1,12.08,6.6,2.78,2.78,0,0,1,14.85,3.83ZM7.62,10.71a2.78,2.78,0,1,1-2.77,2.77A2.77,2.77,0,0,1,7.62,10.71ZM4.39,19.84a2.78,2.78,0,1,1-2.77,2.77A2.77,2.77,0,0,1,4.39,19.84Z";
            ServiceID = DashboardDefinition.DASHBOARD_PAGE_URI_ORIGINAL_STRING;
            ServicePageLoadingDelayTime = DashboardDefinition.DASHBOARD_PAGE_LOADING_DELAY_TIME;
            HeaderGeometryData = DashboardDefinition.DASHBOARD_PAGE_HEADER_GEOMETRY_DATA;
            ServiceResourceUri = new Uri("pack://application:,,,/dashboard_service;component/themes/Themes.xaml",
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
            return null;
        }
    }
}
