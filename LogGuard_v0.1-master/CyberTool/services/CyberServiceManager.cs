﻿using cyber_base.app;
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
        public ICyberService? LogGuardSvc { get; private set; }
        public ICyberService? DashboardSvc { get; private set; }
        public ICyberService? IssueManagerService { get; private set; }
        public ICyberService? ExtensionService { get; private set; }
        public ICyberService? AboutService { get; private set; }
        public ICyberService? LogoutService { get; private set; }

        public Dictionary<string, ICyberService> CyberServiceMaper { get; }


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

            LogGuardSvc = LogGuardService.Current;
            DashboardSvc = DashboardService.Current;

            LogGuardSvc.OnServiceCreate(this);
            DashboardSvc.OnServiceCreate(this);

            CyberServiceMaper.Add(DashboardSvc.ServiceID, DashboardSvc);
            CyberServiceMaper.Add(LogGuardSvc.ServiceID, LogGuardSvc);
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

    }
}
