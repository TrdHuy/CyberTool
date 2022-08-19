using extension_manager_service.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.implement.module
{
    internal class BaseExtensionManagerModule : IExtensionManagerModule
    {
        public virtual void OnDestroy()
        {
        }

        public virtual void OnModuleStart()
        {
        }

        public virtual void OnViewInstantiated()
        {
        }
    }
}
