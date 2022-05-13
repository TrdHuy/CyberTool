using cyber_base.service;
using cyber_tool.@base.module;
using cyber_tool.utils;
using cyber_tool.windows.cyber_iface.views.usercontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_tool.services
{
    /// <summary>
    /// Class này dùng để điều khiển trạng thái các service 
    /// trên CyberTool
    /// </summary>
    internal class CyberServiceController : ICyberModule
    {
        public static CyberServiceController Current
        {
            get
            {
                return CyberToolModuleManager.CSC_Insatace;
            }
        }

        private CyberServiceManager _ServiceManager;
        public ICyberService CurrentService { get; private set; }
        public ICyberService PreviousService { get; private set; }
        public object? CurrentServiceView { get; private set; }

        public event BeforeServiceChangeHandler BeforeServiceChange;
        public event ServiceChangeHandler ServiceChange;
        public event ServiceChangedHandler ServiceChanged;

        private CyberServiceController()
        {
        }

        public void OnModuleInit()
        {
        }

        public void OnModuleStart()
        {
            _ServiceManager = CyberServiceManager.Current;
            CurrentService = _ServiceManager.LogGuardService;
        }

        public void OnIFaceWindowShowed()
        {
            UpdateCurrentServiceView(new ServiceEventArgs(CurrentService, PreviousService));
        }

        public void UpdateCurrentServiceByID(string id)
        {
            if (string.IsNullOrEmpty(id)
                || id == CurrentService.ServiceID)
            {
                return;
            }

            if (_ServiceManager.CyberServiceMaper[id] != null)
            {
                PreviousService = CurrentService;
                CurrentService = _ServiceManager.CyberServiceMaper[id];
                var args = new ServiceEventArgs(CurrentService, PreviousService);

                UpdateCurrentServiceView(args);
            }

        }

        private void UpdateCurrentServiceView(ServiceEventArgs args)
        {
            BeforeServiceChange?.Invoke(this, args);

            if (CurrentService.IsUnderconstruction)
            {
                CurrentServiceView = new Underconstruction();
            }
            else
            {
                CurrentServiceView = CurrentService.GetServiceView();
            }

            if (!args.Handled)
            {
                ServiceChange?.Invoke(this, args);
            }
            if (!args.Handled)
            {
                ServiceChanged?.Invoke(this, args);
            }
        }

        internal delegate void BeforeServiceChangeHandler(object sender, ServiceEventArgs args);
        internal delegate void ServiceChangeHandler(object sender, ServiceEventArgs args);
        internal delegate void ServiceChangedHandler(object sender, ServiceEventArgs args);

        internal class ServiceEventArgs
        {
            public bool Handled { get; set; }
            public ICyberService Previous { get; }
            public ICyberService Current { get; }

            public ServiceEventArgs(ICyberService cur, ICyberService pre)
            {
                Current = cur;
                Previous = pre;
            }
        }
    }



}
