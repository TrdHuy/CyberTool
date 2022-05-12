using cyber_base.implement.service;
using cyber_base.service;
using log_guard.definitions;
using log_guard.view_models;
using log_guard.views.usercontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace log_guard
{
    public class LogGuardService : AbstractCyberService
    {
        public override string ServiceID { get; protected set; }

        public override long ServicePageLoadingDelayTime { get; protected set; }

        public override string HeaderGeometryData { get; protected set; }

        public override string Header { get; protected set; }

        public override bool IsUnderconstruction => false;

        public override Uri ServiceResourceUri { get; protected set; }

        public LogGuardService()
        {
            ServiceID = LogGuardDefinition.LOG_GUARD_PAGE_URI_ORIGINAL_STRING;
            ServicePageLoadingDelayTime = LogGuardDefinition.LOG_GUARD_PAGE_LOADING_DELAY_TIME;
            HeaderGeometryData = LogGuardDefinition.LOG_GUARD_PAGE_HEADER_GEOMETRY_DATA;
            Header = "Log guard";
            ServiceResourceUri = new Uri("pack://application:,,,/log_guard;component/themes/Themes.xaml",
                     UriKind.Absolute);
        }

        public override void OnServiceCreate(ICyberServiceManager cyberServiceManager)
        {
            base.OnServiceCreate(cyberServiceManager);
        }

        public override void OnPreServiceViewInit(ICyberServiceManager cyberServiceManager)
        {
            base.OnPreServiceViewInit(cyberServiceManager);
        }

        public override void OnServiceViewInstantiated(ICyberServiceManager cyberServiceManager)
        {
            base.OnServiceViewInstantiated(cyberServiceManager);
            int a = 1;
        }

        public override void OnServiceUnloaded(ICyberServiceManager cyberServiceManager)
        {
            base.OnServiceUnloaded(cyberServiceManager);
        }

        protected override object? GenerateServiceView()
        {
            return new LogGuard();
        }
    }
}
