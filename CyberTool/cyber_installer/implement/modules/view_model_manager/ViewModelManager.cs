using cyber_base.view_model;
using cyber_installer.@base;
using cyber_installer.view_models.tabs.available_tab;
using cyber_installer.view_models.tabs.installed_tab;
using System;
using System.Collections.Generic;

namespace cyber_installer.implement.modules.view_model_manager
{
    internal class ViewModelManager : AbstractViewModelManager, ICyberInstallerModule
    {
        private static Dictionary<Type, object> DataContextCache = new Dictionary<Type, object>();

        public static ViewModelManager Current
        {
            get
            {
                return ModuleManager.VMM_Instance;
            }
        }

        public AvailableTabViewModel AvailableTabViewModel
        {
            get
            {
                return (AvailableTabViewModel)DataContextCache[typeof(AvailableTabViewModel)];
            }
        }

        public InstalledTabViewModel InstalledTabViewModel
        {
            get
            {
                return (InstalledTabViewModel)DataContextCache[typeof(InstalledTabViewModel)];
            }
        }

        public void OnModuleCreate()
        {
            DataContextCache = new Dictionary<Type, object>();
        }

        public void OnModuleStart()
        {
            DataContextCache.Clear();
        }

        protected override void AddDataContextByTypeToCache(Type dataContextType, object dataContext)
        {
            DataContextCache.Add(dataContextType, dataContext);
        }

        protected override object? GetDataContextByTypeFromCache(Type dataContextType)
        {
            return DataContextCache[dataContextType];
        }

        protected override bool IsDataContextTypeExistInCache(Type dataContextType)
        {
            return DataContextCache.ContainsKey(dataContextType);
        }

        protected override object RemoveDataContextByTypeFromCache(Type dataContextType)
        {
            return DataContextCache.Remove(dataContextType);
        }

        public override void Dispose()
        {
            DataContextCache.Clear();
        }


        public void OnModuleDestroy()
        {
            DataContextCache.Clear();
        }

        public void OnMainWindowShowed()
        {
        }
    }
}
