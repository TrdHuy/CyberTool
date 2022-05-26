using cyber_base.service;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cyber_base.implement.service
{
    public abstract class AbstractCyberService : ICyberService
    {
        public ICyberServiceManager? ServiceManager { get; protected set; }

        public abstract bool IsUnderconstruction { get; }

        public abstract string ServiceID { get; protected set; }

        public abstract long ServicePageLoadingDelayTime { get; protected set; }

        public abstract string HeaderGeometryData { get; protected set; }

        public abstract string Header { get; protected set; }

        public abstract Uri ServiceResourceUri { get; protected set; }

        protected object? ServiceViewContext { get; set; }
        protected object? CurrentServiceView { get; set; }

        public virtual void OnServiceCreate(ICyberServiceManager cyberServiceManager)
        {
            ServiceManager = cyberServiceManager;
            var resource = new ResourceDictionary
            {
                Source = ServiceResourceUri
            };
            ServiceManager?.App?.CyberApp?.Resources.MergedDictionaries.Add(resource);
        }

        public object? GetServiceView()
        {
            CurrentServiceView = GenerateServiceView();

            ServiceViewContext = CurrentServiceView?
                .GetType()?
                .GetProperty("DataContext")?
                .GetValue(CurrentServiceView, null);

            return CurrentServiceView;
        }


        public abstract void OnPreServiceViewInit(ICyberServiceManager cyberServiceManager);

        public virtual void OnServiceViewInstantiated(ICyberServiceManager cyberServiceManager)
        {
            (ServiceViewContext as BaseViewModel)?.OnViewInstantiated();
        }

        public virtual void OnServiceViewLoaded(ICyberServiceManager cyberServiceManager)
        {
            (ServiceViewContext as BaseViewModel)?.OnBegin();
        }

        public virtual void OnServiceUnloaded(ICyberServiceManager cyberServiceManager)
        {
            (ServiceViewContext as BaseViewModel)?.OnDestroy();
        }

        protected abstract object? GenerateServiceView();

    }
}
