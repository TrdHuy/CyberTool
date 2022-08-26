using extension_manager_service.@base;
using extension_manager_service.implement.server_contact_manager;
using extension_manager_service.implement.view_model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.implement.module
{
    internal class ModuleManager
    {
        private static Collection<IExtensionManagerModule> _Modules;

        private static IExtensionManagerModule? _VMM_Instance;
        private static IExtensionManagerModule? _SCM_Instance;

        public static ViewModelManager VMM_Instance
        {
            get
            {
                if (_VMM_Instance == null)
                {
                    _VMM_Instance = Activator.CreateInstance(typeof(ViewModelManager)) as ViewModelManager;
                }
                ArgumentNullException.ThrowIfNull(_VMM_Instance);
                return (ViewModelManager)_VMM_Instance;
            }
        }
        
        public static ServerContactManager SCM_Instance
        {
            get
            {
                if (_SCM_Instance == null)
                {
                    _SCM_Instance = Activator.CreateInstance(typeof(ServerContactManager)) as ServerContactManager;
                }
                ArgumentNullException.ThrowIfNull(_SCM_Instance);
                return (ServerContactManager)_SCM_Instance;
            }
        }

        static ModuleManager()
        {
            _Modules = new Collection<IExtensionManagerModule>();
        }

        public static void Init()
        {
            _Modules.Clear();
            _Modules.Add(VMM_Instance);
            _Modules.Add(SCM_Instance);

            foreach (var module in _Modules)
            {
                module.OnModuleStart();
            }
        }

        public static void OnViewInstantiated()
        {
            foreach (var module in _Modules)
            {
                module.OnViewInstantiated();
            }
        }

        public static void Destroy()
        {
            foreach (var module in _Modules)
            {
                module.OnDestroy();
            }
        }


    }
}
