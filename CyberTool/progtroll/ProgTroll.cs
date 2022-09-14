using cyber_base.extension;
using cyber_base.implement.service;
using cyber_base.service;
using cyber_base.view_model;
using progtroll.definitions;
using progtroll.implement.module;
using progtroll.views.usercontrols;
using System;

namespace progtroll
{
    public class ProgTroll : AbstractCyberService, ICyberExtension
    {
        public static ProgTroll Current { get; private set; }

        public override string ServiceID { get; protected set; }

        public override long ServicePageLoadingDelayTime { get; protected set; }

        public override string HeaderGeometryData { get; protected set; }

        public override string Header { get; protected set; }

        public override bool IsUnderconstruction => false;

        public override Uri ServiceResourceUri { get; protected set; }

        static ProgTroll()
        {
            Current = new ProgTroll();
        }

        public ProgTroll()
        {
            Header = "Progtroll";
            HeaderGeometryData = PublisherDefinition.PUBLISHER_PLUGIN_GEOMETRY_DATA;
            ServiceID = PublisherDefinition.SERVICE_PAGE_URI_ORIGINAL_STRING;
            ServicePageLoadingDelayTime = PublisherDefinition.SERVICE_PAGE_LOADING_DELAY_TIME;
            ServiceResourceUri = new Uri("pack://application:,,,/progtroll;component/themes/Themes.xaml",
                   UriKind.Absolute);
        }

        public override void OnServiceCreate(ICyberServiceManager cyberServiceManager)
        {
            base.OnServiceCreate(cyberServiceManager);
        }

        public override void OnPreServiceViewInit(ICyberServiceManager cyberServiceManager)
        {
            PublisherModuleManager.Init();
        }

        public override void OnServiceViewInstantiated(ICyberServiceManager cyberServiceManager)
        {
            PublisherModuleManager.OnViewInstantiated();
            (ServiceViewContext as BaseViewModel)?.OnViewInstantiated();
        }

        public override void OnServiceViewLoaded(ICyberServiceManager cyberServiceManager)
        {
            base.OnServiceViewLoaded(cyberServiceManager);
        }

        public override void OnServiceUnloaded(ICyberServiceManager cyberServiceManager)
        {
            base.OnServiceUnloaded(cyberServiceManager);
            PublisherModuleManager.Destroy();
        }

        protected override object? GenerateServiceView()
        {
            return new ServiceView();
        }

        public void OnPluginInstalled()
        {
        }

        public void OnPluginUninstalled()
        {
        }

        public void OnPluginStart()
        {
        }
    }
}
