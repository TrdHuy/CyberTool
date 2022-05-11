using cyber_base.service;
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
        public static ICyberService Current { get; private set; }

        public ICyberServiceManager ServiceManager { get; protected set; }

        public abstract bool IsUnderconstruction { get; }

        public abstract string ServiceID { get; protected set; }

        public abstract long ServicePageLoadingDelayTime { get; protected set; }

        public abstract string HeaderGeometryData { get; protected set; }

        public abstract string Header { get; protected set; }

        public abstract Uri ServiceResourceUri { get; protected set; }

        public virtual void OnServiceCreate(ICyberServiceManager cyberServiceManager)
        {
            ServiceManager = cyberServiceManager;
            Current = this;
            var resource = new ResourceDictionary
            {
                Source = ServiceResourceUri
            };
            ServiceManager?.App.Resources.MergedDictionaries.Add(resource);
        }

        public abstract object? GetServiceView();

        public virtual void OnPreServiceViewInit(ICyberServiceManager cyberServiceManager)
        {
        }

        public virtual void OnServiceUnloaded(ICyberServiceManager cyberServiceManager)
        {
        }

        public virtual void OnServiceViewInstantiated(ICyberServiceManager cyberServiceManager)
        {
        }
    }
}
