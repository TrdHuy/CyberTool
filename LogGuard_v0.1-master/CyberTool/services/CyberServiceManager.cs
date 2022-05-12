using cyber_base.service;
using cyber_tool.@base.module;
using cyber_tool.utils;
using dashboard_service;
using log_guard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cyber_tool.services
{
    /// <summary>
    /// Class này dùng để giao tiếp giữa Cyber tool và các project
    /// triển khai Cyber service
    /// </summary>
    public class CyberServiceManager : ICyberServiceManager, ICyberModule
    {
        private CyberServiceController _ServiceController;

        public ICyberService LogGuardService { get; private set; }
        public ICyberService DashboardService { get; private set; }
        public ICyberService IssueManagerService { get; private set; }
        public ICyberService ExtensionService { get; private set; }
        public ICyberService AboutService { get; private set; }
        public ICyberService LogoutService { get; private set; }

        public Dictionary<string, ICyberService> CyberServiceMaper { get; }


        public static CyberServiceManager Current
        {
            get
            {
                return CyberToolModuleManager.CSM_Insatace;
            }
        }

        public Application App
        {
            get
            {
                return cyber_tool.App.Current;
            }
        }

        private CyberServiceManager()
        {
            CyberServiceMaper = new Dictionary<string, ICyberService>();
        }

        public void OnModuleInit()
        {
            CyberServiceMaper.Clear();

            LogGuardService = new LogGuardService();
            DashboardService = new DashboardService();

            LogGuardService.OnServiceCreate(this);
            DashboardService.OnServiceCreate(this);

            CyberServiceMaper.Add(DashboardService.ServiceID, DashboardService);
            CyberServiceMaper.Add(LogGuardService.ServiceID, LogGuardService);
        }

        public void OnModuleStart()
        {
            _ServiceController = CyberServiceController.Current;
            _ServiceController.CurrentServiceChange -= OnCurrentServiceViewGenerated;
            _ServiceController.CurrentServiceChange += OnCurrentServiceViewGenerated;

            _ServiceController.CurrentServiceChanged -= OnCurrentServiceChanged;
            _ServiceController.CurrentServiceChanged += OnCurrentServiceChanged;

        }

        private void OnCurrentServiceViewGenerated(object sender, CyberServiceController.ServiceEventArgs args)
        {
            if (args.IsChanged)
            {
                args.Previous.OnServiceUnloaded(this);

                args.Current.OnPreServiceViewInit(this);
            }
            else
            {
                args.Handled = true;
            }
        }

        private void OnCurrentServiceChanged(object sender, CyberServiceController.ServiceEventArgs args)
        {
            args.Current.OnServiceViewInstantiated(this);
        }
    }
}
