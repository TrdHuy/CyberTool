using cyber_installer.@base;
using cyber_installer.implement.modules.server_contact_manager;
using cyber_installer.implement.modules.sw_installing_manager;
using cyber_installer.implement.modules.ui_event_handler;
using cyber_installer.implement.modules.user_config_manager;
using cyber_installer.implement.modules.user_data_manager;
using cyber_installer.implement.modules.view_model_manager;
using cyber_installer.implement.modules.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cyber_installer.implement.modules.server_contact_manager.security;

namespace cyber_installer.implement.modules
{
    internal class ModuleManager
    {
        private static Collection<ICyberInstallerModule> _CyberModules = new Collection<ICyberInstallerModule>();

        private static ICyberInstallerModule? _VMM_Instance;
        private static ICyberInstallerModule? _UCM_Instance;
        private static ICyberInstallerModule? _SCM_Instance;
        private static ICyberInstallerModule? _UDM_Instance;
        private static ICyberInstallerModule? _SIM_Instance;
        private static ICyberInstallerModule? _CEF_Instance;
        private static ICyberInstallerModule? _KAL_Instance;
        private static ICyberInstallerModule? _CM_Instance;

        public static void Init()
        {
            _CyberModules.Clear();
            _CyberModules.Add(VMM_Instance);
            _CyberModules.Add(UCM_Instance);
            _CyberModules.Add(SCM_Instance);
            _CyberModules.Add(UDM_Instance);
            _CyberModules.Add(SIM_Instance);
            _CyberModules.Add(CEF_Instance);
            _CyberModules.Add(KAL_Instance);
            _CyberModules.Add(CM_Instance);

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
        public static CertificateManager CM_Instance
        {
            get
            {
                if (_CM_Instance == null)
                {
                    _CM_Instance = Activator.CreateInstance(typeof(CertificateManager), true) as ICyberInstallerModule;
                }
                ArgumentNullException.ThrowIfNull(_CM_Instance);
                return (CertificateManager)_CM_Instance;
            }


        }
        
        public static KeyActionListener KAL_Instance
        {
            get
            {
                if (_KAL_Instance == null)
                {
                    _KAL_Instance = Activator.CreateInstance(typeof(KeyActionListener), true) as ICyberInstallerModule;
                }
                ArgumentNullException.ThrowIfNull(_KAL_Instance);
                return (KeyActionListener)_KAL_Instance;
            }
        }

        public static CommandExecuterFactory CEF_Instance
        {
            get
            {
                if (_CEF_Instance == null)
                {
                    _CEF_Instance = Activator.CreateInstance(typeof(CommandExecuterFactory), true) as ICyberInstallerModule;
                }
                ArgumentNullException.ThrowIfNull(_CEF_Instance);
                return (CommandExecuterFactory)_CEF_Instance;
            }

        }

        public static SwInstallingManager SIM_Instance
        {
            get
            {
                if (_SIM_Instance == null)
                {
                    _SIM_Instance = Activator.CreateInstance(typeof(SwInstallingManager), true) as ICyberInstallerModule;
                }
                ArgumentNullException.ThrowIfNull(_SIM_Instance);
                return (SwInstallingManager)_SIM_Instance;
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

        public static UserDataManager UDM_Instance
        {
            get
            {
                if (_UDM_Instance == null)
                {
                    _UDM_Instance = Activator.CreateInstance(typeof(UserDataManager), true) as ICyberInstallerModule;
                }
                ArgumentNullException.ThrowIfNull(_UDM_Instance);
                return (UserDataManager)_UDM_Instance;
            }
        }
    }
}