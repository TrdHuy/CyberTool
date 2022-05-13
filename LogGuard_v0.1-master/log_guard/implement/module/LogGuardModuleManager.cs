using log_guard.@base.module;
using log_guard.implement.device;
using log_guard.implement.flow.view_helper;
using log_guard.implement.process;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.module
{
    internal class LogGuardModuleManager
    {
        private static Collection<ILogGuardModule> _Modules;

        private static ILogGuardModule? _PM_Instance;
        private static ILogGuardModule? _LGVH_Instance;
        private static ILogGuardModule? _DM_Instance;
        private static ILogGuardModule? _DCE_Instance;
        public static void Init()
        {
            _Modules = new Collection<ILogGuardModule>();

            _Modules.Add(PM_Instance);
            _Modules.Add(LGVH_Instance);
            //_Modules.Add(DM_Instance);
            _Modules.Add(DCE_Instance);

            foreach(var module in _Modules)
            {
                module.OnModuleInit();
            }

            foreach (var module in _Modules)
            {
                module.OnModuleStart();
            }
        }

        public static ProcessManager? PM_Instance
        {
            get
            {
                if (_PM_Instance == null)
                {
                    _PM_Instance = Activator.CreateInstance(typeof(ProcessManager)) as ILogGuardModule;
                }
                return _PM_Instance as ProcessManager;
            }
        }

        public static LogGuardViewHelper? LGVH_Instance
        {
            get
            {
                if (_LGVH_Instance == null)
                {
                    _LGVH_Instance = Activator.CreateInstance(typeof(LogGuardViewHelper)) as ILogGuardModule;
                }
                return _LGVH_Instance as LogGuardViewHelper;
            }
        }

        public static DeviceManager? DM_Instance
        {
            get
            {
                if (_DM_Instance == null)
                {
                    _DM_Instance = Activator.CreateInstance(typeof(DeviceManager)) as ILogGuardModule;
                }
                return _DM_Instance as DeviceManager;
            }
        }

        public static DeviceCmdExecuter? DCE_Instance
        {
            get
            {
                if (_DCE_Instance == null)
                {
                    _DCE_Instance = Activator.CreateInstance(typeof(DeviceCmdExecuter)) as ILogGuardModule;
                }
                return _DCE_Instance as DeviceCmdExecuter;
            }
        }

    }
}
