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

        public event CurrentServiceChangeHandler CurrentServiceChange;
        public event CurrentServiceChangedHandler CurrentServiceChanged;

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
            UpdateCurrentServiceView();
        }

        public void UpdateCurrentServiceByID(string id)
        {
            if (string.IsNullOrEmpty(id)
                || id == CurrentService.ServiceID)
            {
                return;
            }

            bool isChanged = false;

            if (_ServiceManager.CyberServiceMaper[id] != null)
            {
                PreviousService = CurrentService;
                CurrentService = _ServiceManager.CyberServiceMaper[id];
                isChanged = true;

                UpdateCurrentServiceView();
            }
            var args = new ServiceEventArgs(CurrentService, PreviousService, isChanged);
            CurrentServiceChange?.Invoke(this, args);

            if (!isChanged || !args.Handled)
            {
                CurrentServiceChanged?.Invoke(this, args);
            }
        }

        private void UpdateCurrentServiceView()
        {
            if (CurrentService.IsUnderconstruction)
            {
                CurrentServiceView = new Underconstruction();
            }
            else
            {
                CurrentServiceView = CurrentService.GetServiceView();
            }
        }

        internal delegate void CurrentServiceChangeHandler(object sender, ServiceEventArgs args);
        internal delegate void CurrentServiceChangedHandler(object sender, ServiceEventArgs args);

        internal class ServiceEventArgs
        {
            public bool IsChanged { get; }
            public bool Handled { get; set; }
            public ICyberService Previous { get; }
            public ICyberService Current { get; }

            public ServiceEventArgs(ICyberService cur, ICyberService pre, bool isChanged)
            {
                Current = cur;
                Previous = pre;
                IsChanged = isChanged;
            }
        }
    }



}
