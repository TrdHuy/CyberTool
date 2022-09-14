using cyber_base.app;
using cyber_base.service;
using cyber_core.@base.module;
using cyber_core.utils;
using dashboard_service;
using extension_manager_service;
using faq_service;
using issue_tracker_service;
using log_guard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using cyber_base.extension;
using System.Collections.Specialized;

namespace cyber_core.services
{
    /// <summary>
    /// Class này dùng để giao tiếp giữa Cyber tool và các project
    /// triển khai Cyber service
    /// </summary>
    public class CyberServiceManager : ICyberServiceManager, ICyberCoreModule
    {
        public ICyberService? LogGuardSvc { get; private set; }
        public ICyberService? DashboardSvc { get; private set; }
        public ICyberService? IssueTrackerSvc { get; private set; }
        public ICyberService? EMSvc { get; private set; }
        public ICyberService? FAQSvc { get; private set; }

        public Dictionary<string, ICyberService> CyberServiceMaper { get; }
        public Dictionary<string, ICyberService> CyberExtensionServiceMapper { get; }
        public ExtensionServiceMapperCollectionChangedEventHandler? ExtensionServiceMapperCollectionChanged;

        public static CyberServiceManager Current
        {
            get
            {
                return CyberToolModuleManager.CSM_Insatace;
            }
        }

        public ICyberApplication App
        {
            get
            {
                return cyber_core.App.Current;
            }
        }


        private CyberServiceManager()
        {
            CyberServiceMaper = new Dictionary<string, ICyberService>();
            CyberExtensionServiceMapper = new Dictionary<string, ICyberService>();
        }

        public void OnModuleInit()
        {
            CyberServiceMaper.Clear();

            LogGuardSvc = LogGuardService.Current;
            DashboardSvc = DashboardService.Current;
            IssueTrackerSvc = IssueTrackerService.Current;
            EMSvc = ExtensionManagerService.Current;
            FAQSvc = FAQService.Current;

            CyberServiceMaper.Add(DashboardSvc.ServiceID, DashboardSvc);
            CyberServiceMaper.Add(LogGuardSvc.ServiceID, LogGuardSvc);
            CyberServiceMaper.Add(IssueTrackerSvc.ServiceID, IssueTrackerSvc);
            CyberServiceMaper.Add(EMSvc.ServiceID, EMSvc);
            CyberServiceMaper.Add(FAQSvc.ServiceID, FAQSvc);

            foreach (var service in CyberServiceMaper.Values)
            {
                service.OnServiceCreate(this);
            }
        }

        public void OnModuleStart()
        {
            CyberServiceController.Current.BeforeServiceChange -= OnBeforeServiceChange;
            CyberServiceController.Current.BeforeServiceChange += OnBeforeServiceChange;

            CyberServiceController.Current.ServiceChange -= OnServiceChange;
            CyberServiceController.Current.ServiceChange += OnServiceChange;

            CyberServiceController.Current.ServiceLoaded -= OnServiceLoaded;
            CyberServiceController.Current.ServiceLoaded += OnServiceLoaded;

            CyberServiceController.Current.ServiceChanged -= OnServiceChanged;
            CyberServiceController.Current.ServiceChanged += OnServiceChanged;
        }

        public void OnModuleDestroy()
        {
            foreach (var service in CyberServiceMaper.Values)
            {
                service.OnServiceDestroy(this);
            }
        }

        public void OnIFaceWindowShowed()
        {
        }

        private void OnBeforeServiceChange(object sender, CyberServiceController.ServiceEventArgs args)
        {
            args.Current?.OnPreServiceViewInit(this);
        }

        private void OnServiceChange(object sender, CyberServiceController.ServiceEventArgs args)
        {
            args.Current?.OnServiceViewInstantiated(this);
        }

        private void OnServiceLoaded(object sender, CyberServiceController.ServiceEventArgs args)
        {
            args.Current?.OnServiceViewLoaded(this);
        }

        private void OnServiceChanged(object sender, CyberServiceController.ServiceEventArgs args)
        {
            args.Previous?.OnServiceUnloaded(this);
        }

        public string GetServicesBaseFolderLocation()
        {
            return "services";
        }

        public string GetPluginsBaseFolderLocation()
        {
            return "plugins";
        }

        public void RegisterExtensionAsCyberService(ICyberExtension cyberExtension)
        {
            var service = cyberExtension as ICyberService;
            if (service != null)
            {
                CyberExtensionServiceMapper.Add(service.ServiceID, service);
                service.OnServiceCreate(this);
                ExtensionServiceMapperCollectionChanged?.Invoke(this
                    , new ExtensionServiceMapperCollectionChangedEventArgs(NotifyCollectionChangedAction.Add
                        , service
                        , null));
            }
        }

        public void UnregisterExtensionAsCyberService(ICyberExtension cyberExtension)
        {
            var service = cyberExtension as ICyberService;
            if (service != null)
            {
                CyberExtensionServiceMapper.Remove(service.ServiceID);
                service.OnServiceDestroy(this);
                ExtensionServiceMapperCollectionChanged?.Invoke(this
                    , new ExtensionServiceMapperCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove
                        , null
                        , service));
            }
        }
    }

    public delegate void ExtensionServiceMapperCollectionChangedEventHandler(object sender, ExtensionServiceMapperCollectionChangedEventArgs args);
    public class ExtensionServiceMapperCollectionChangedEventArgs
    {
        public NotifyCollectionChangedAction Action { get; private set; }
        public ICyberService? NewService { get; private set; }
        public ICyberService? OldService { get; private set; }

        public ExtensionServiceMapperCollectionChangedEventArgs(NotifyCollectionChangedAction action
            , ICyberService? newService
            , ICyberService? oldService)
        {
            Action = action;
            NewService = newService;
            OldService = oldService;
        }
    }
}
