using cyber_base.service;
using cyber_core.@base.module;
using cyber_core.utils;
using cyber_core.windows.cyber_iface.views.usercontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cyber_core.services
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

        public ICyberService? CurrentService { get; private set; }
        public ICyberService? PreviousService { get; private set; }
        public FrameworkElement? CurrentServiceView { get; private set; }

        public event BeforeServiceChangeHandler? BeforeServiceChange;
        public event ServiceChangeHandler? ServiceChange;
        public event ServiceChangedHandler? ServiceChanged;
        public event ServiceViewLoadedHandler? ServiceLoaded;

        private CyberServiceController()
        {
        }

        public void OnModuleInit()
        {
        }

        public void OnModuleStart()
        {
            CurrentService = CyberServiceManager.Current.LogGuardSvc;
        }

        public void OnIFaceWindowShowed()
        {
            UpdateCurrentServiceView(new ServiceEventArgs(CurrentService, PreviousService));
        }

        public void UpdateCurrentServiceByID(string id)
        {
            if (string.IsNullOrEmpty(id)
                || id == CurrentService?.ServiceID)
            {
                return;
            }

            if (CyberServiceManager.Current.CyberServiceMaper[id] != null)
            {
                PreviousService = CurrentService;
                CurrentService = CyberServiceManager.Current.CyberServiceMaper[id];
                var arg = new ServiceEventArgs(CurrentService, PreviousService);

                UpdateCurrentServiceView(arg);
            }

        }

        private void UpdateCurrentServiceView(ServiceEventArgs args)
        {

            BeforeServiceChange?.Invoke(this, args);

            if(CurrentService == null)
            {
                CurrentServiceView = new Underconstruction();
            }
            else
            {
                if (CurrentService.IsUnderconstruction)
                {
                    CurrentServiceView = new Underconstruction();
                }
                else
                {
                    CurrentServiceView = CurrentService.GetServiceView() as FrameworkElement;

                    if (CurrentServiceView != null)
                    {
                        var onloaded = new Action<object, RoutedEventArgs>((s, e) =>
                        {
                            ServiceLoaded?.Invoke(this, args);
                        });
                        CurrentServiceView.Loaded -= new RoutedEventHandler(onloaded);
                        CurrentServiceView.Loaded += new RoutedEventHandler(onloaded);
                    }

                }
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
        internal delegate void ServiceViewLoadedHandler(object sender, ServiceEventArgs args);
        internal delegate void ServiceChangedHandler(object sender, ServiceEventArgs args);

        internal class ServiceEventArgs
        {
            public bool Handled { get; set; }
            public ICyberService? Previous { get; }
            public ICyberService? Current { get; }

            public ServiceEventArgs(ICyberService? cur, ICyberService? pre)
            {
                Current = cur;
                Previous = pre;
            }
        }
    }



}
