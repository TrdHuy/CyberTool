using cyber_base.service;
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
    public class CyberServiceManager : ICyberServiceManager
    {
        public Collection<ICyberService> CyberServicesCollection { get; }
        public ICyberService LogGuardService { get; private set; }
        public ICyberService DashboardService { get; private set; }
        public ICyberService IssueManagerService { get; private set; }
        public ICyberService ExtensionService { get; private set; }
        public ICyberService AboutService { get; private set; }
        public ICyberService LogoutService { get; private set; }

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
            CyberServicesCollection = new Collection<ICyberService>();
            LogGuardService = new LogGuardService();
            DashboardService = new DashboardService();

            LogGuardService.OnServiceCreate(this);
            DashboardService.OnServiceCreate(this);

            CyberServicesCollection.Add(DashboardService);
            CyberServicesCollection.Add(LogGuardService);
        }
    }
}
