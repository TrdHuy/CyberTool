using cyber_base.view_model;
using cyber_installer.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
