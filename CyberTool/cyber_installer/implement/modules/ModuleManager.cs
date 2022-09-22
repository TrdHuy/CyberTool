using cyber_installer.@base;
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

        public static void Init()
        {
            _CyberModules.Clear();
            _CyberModules.Add(VMM_Instance);

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
    }
}