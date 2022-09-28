using cyber_installer.@base;
using cyber_installer.implement.modules.server_contact_manager;
using cyber_installer.implement.modules.user_config_manager;
using cyber_installer.implement.modules.view_model_manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules
{
    internal class ModuleManager
    {
        private static Collection<ICyberInstallerModule> _CyberModules = new Collection<ICyberInstallerModule>();

        private static ICyberInstallerModule? _VMM_Instance;
        private static ICyberInstallerModule? _UCM_Instance;
        private static ICyberInstallerModule? _SCM_Instance;

        public static void Init()
        {
            _CyberModules.Clear();
            _CyberModules.Add(VMM_Instance);
            _CyberModules.Add(UCM_Instance);
            _CyberModules.Add(SCM_Instance);

            foreach (var module in _CyberModules)
            {
                module.OnModuleCreate();
            }

            foreach (var module in _CyberModules)
            {
                module.OnModuleStart();
            }
        }

        public static void Destroy()
        {
            foreach (var module in _CyberModules)
            {
                module.OnModuleDestroy();
            }

        }

        public static ViewModelManager VMM_Instance
        {
            get
            {
                if (_VMM_Instance == null)
                {
                    _VMM_Instance = Activator.CreateInstance(typeof(ViewModelManager), true) as ICyberInstallerModule;
                }
                ArgumentNullException.ThrowIfNull(_VMM_Instance);
                return (ViewModelManager)_VMM_Instance;
            }
        }

        public static UserConfigManager UCM_Instance
        {
            get
            {
                if (_UCM_Instance == null)
                {
                    _UCM_Instance = Activator.CreateInstance(typeof(UserConfigManager), true) as ICyberInstallerModule;
                }
                ArgumentNullException.ThrowIfNull(_UCM_Instance);
                return (UserConfigManager)_UCM_Instance;
            }
        }

        public static ServerContactManager SCM_Instance
        {
            get
            {
                if (_SCM_Instance == null)
                {
                    _SCM_Instance = Activator.CreateInstance(typeof(ServerContactManager), true) as ICyberInstallerModule;
                }
                ArgumentNullException.ThrowIfNull(_SCM_Instance);
                return (ServerContactManager)_SCM_Instance;
            }
        }
    }
}