using extension_manager_service.@base;
using extension_manager_service.implement.log_manager;
using extension_manager_service.implement.plugin_manager;
using extension_manager_service.implement.server_contact_manager;
using extension_manager_service.implement.ui_event_handler;
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
        private static IExtensionManagerModule? _CPM_Instance;
        private static IExtensionManagerModule? _EKAL_Instance;
        private static IExtensionManagerModule? _ECEF_Instance;
        private static IExtensionManagerModule? _ELM_Instance;
        public static EMSLogManager ELM_Instance
        {
            get
            {
                if (_ELM_Instance == null)
                {
                    _ELM_Instance = Activator.CreateInstance(typeof(EMSLogManager), true) as EMSLogManager;
                }
                ArgumentNullException.ThrowIfNull(_ELM_Instance);
                return (EMSLogManager)_ELM_Instance;
            }
        }

        public static EMSCommandExecuterFactory ECEF_Instance
        {
            get
            {
                if (_ECEF_Instance == null)
                {
                    _ECEF_Instance = Activator.CreateInstance(typeof(EMSCommandExecuterFactory), true) as EMSCommandExecuterFactory;
                }
                ArgumentNullException.ThrowIfNull(_ECEF_Instance);
                return (EMSCommandExecuterFactory)_ECEF_Instance;
            }
        }

        public static EMSKeyActionListener EKAL_Instance
        {
            get
            {
                if (_EKAL_Instance == null)
                {
                    _EKAL_Instance = Activator.CreateInstance(typeof(EMSKeyActionListener), true) as EMSKeyActionListener;
                }
                ArgumentNullException.ThrowIfNull(_EKAL_Instance);
                return (EMSKeyActionListener)_EKAL_Instance;
            }
        }

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
                    _SCM_Instance = Activator.CreateInstance(typeof(ServerContactManager), true) as ServerContactManager;
                }
                ArgumentNullException.ThrowIfNull(_SCM_Instance);
                return (ServerContactManager)_SCM_Instance;
            }
        }

        public static CyberPluginManager CPM_Instance
        {
            get
            {
                if (_CPM_Instance == null)
                {
                    _CPM_Instance = Activator.CreateInstance(typeof(CyberPluginManager), true) as CyberPluginManager;
                }
                ArgumentNullException.ThrowIfNull(_CPM_Instance);
                return (CyberPluginManager)_CPM_Instance;
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
            _Modules.Add(CPM_Instance);
            _Modules.Add(EKAL_Instance);
            _Modules.Add(ECEF_Instance);

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
            _Modules.Clear();
            _VMM_Instance = null;
            _CPM_Instance = null;
            _SCM_Instance = null;
            _EKAL_Instance = null;
            _ECEF_Instance = null;
        }


    }
}
