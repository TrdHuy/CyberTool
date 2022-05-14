using log_guard._config;
using log_guard.@base.module;
using log_guard.implement.device;
using log_guard.implement.flow.log_manager;
using log_guard.implement.flow.run_thread_config;
using log_guard.implement.flow.source_filter_manager;
using log_guard.implement.flow.source_highlight_manager;
using log_guard.implement.flow.source_manager;
using log_guard.implement.flow.state_controller;
using log_guard.implement.flow.view_helper;
using log_guard.implement.flow.view_model;
using log_guard.implement.process;
using log_guard.implement.ui_event_handler;
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
        private static ILogGuardModule? _VMM_Instance;
        private static ILogGuardModule? _RTCM_Instance;
        private static ILogGuardModule? _SFM_Instance;
        private static ILogGuardModule? _LGKAL_Instance;
        private static ILogGuardModule? _LGCEF_Instance;
        private static ILogGuardModule? _SHM_Instance;
        private static ILogGuardModule? _SM_Instance;
        private static ILogGuardModule? _LIF_Instance;
        private static ILogGuardModule? _SC_Instance;

        public static void Init()
        {
            _Modules = new Collection<ILogGuardModule>();

            _Modules.Add(PM_Instance);
            _Modules.Add(LGVH_Instance);
            _Modules.Add(DM_Instance);
            _Modules.Add(DCE_Instance);
            _Modules.Add(VMM_Instance);
            _Modules.Add(RTCM_Instance);
            _Modules.Add(SFM_Instance);
            _Modules.Add(LGKAL_Instance);
            _Modules.Add(LGCEF_Instance);
            _Modules.Add(SHM_Instance);
            _Modules.Add(SM_Instance);
            _Modules.Add(LIF_Instance);
            _Modules.Add(SC_Instance);

            foreach (var module in _Modules)
            {
                module.OnModuleInit();
            }

            foreach (var module in _Modules)
            {
                module.OnModuleStart();
            }
        }

        public static void Destroy()
        {
            _Modules.Clear();
            _PM_Instance = null;
            _LGVH_Instance = null;
            _DM_Instance = null;
            _DCE_Instance = null;
            _VMM_Instance = null;
            _RTCM_Instance = null;
            _SFM_Instance = null;
            _LGKAL_Instance = null;
            _LGCEF_Instance = null;
            _SHM_Instance = null;
            _SM_Instance = null;
            _LIF_Instance = null;
            _SC_Instance = null;
        }
        public static StateController? SC_Instance
        {
            get
            {
                if (_SC_Instance == null)
                {
                    if (RUNE.IS_SUPPORT_HIGH_CPU_LOG_CAPTURE)
                        _SC_Instance = Activator.CreateInstance(typeof(HighCpu_StateController)) as ILogGuardModule;

                    else
                        _SC_Instance = Activator.CreateInstance(typeof(LowCpu_StateController)) as ILogGuardModule;
                }
                return _SC_Instance as StateController;
            }
        }
        public static LogGuardCommandExecuterFactory? LGCEF_Instance
        {
            get
            {
                if (_LGCEF_Instance == null)
                {
                    _LGCEF_Instance = Activator.CreateInstance(typeof(LogGuardCommandExecuterFactory)) as ILogGuardModule;
                }
                return _LGCEF_Instance as LogGuardCommandExecuterFactory;
            }
        }

        public static LogGuardKeyActionListener? LGKAL_Instance
        {
            get
            {
                if (_LGKAL_Instance == null)
                {
                    _LGKAL_Instance = Activator.CreateInstance(typeof(LogGuardKeyActionListener)) as ILogGuardModule;
                }
                return _LGKAL_Instance as LogGuardKeyActionListener;
            }
        }
        public static LogInfoManager? LIF_Instance
        {
            get
            {
                if (_LIF_Instance == null)
                {
                    _LIF_Instance = Activator.CreateInstance(typeof(LogInfoManager)) as ILogGuardModule;
                }
                return _LIF_Instance as LogInfoManager;
            }
        }
        public static RunThreadConfigManager? RTCM_Instance
        {
            get
            {
                if (_RTCM_Instance == null)
                {
                    _RTCM_Instance = Activator.CreateInstance(typeof(RunThreadConfigManager)) as ILogGuardModule;
                }
                return _RTCM_Instance as RunThreadConfigManager;
            }
        }
        public static SourceHighlightManager? SHM_Instance
        {
            get
            {
                if (_SHM_Instance == null)
                {
                    _SHM_Instance = Activator.CreateInstance(typeof(SourceHighlightManager)) as ILogGuardModule;
                }
                return _SHM_Instance as SourceHighlightManager;
            }

        }
        public static SourceManager? SM_Instance
        {
            get
            {
                if (_SM_Instance == null)
                {
                    _SM_Instance = Activator.CreateInstance(typeof(SourceManager)) as ILogGuardModule;
                }
                return _SM_Instance as SourceManager;
            }

        }
        public static SourceFilterManager? SFM_Instance
        {
            get
            {
                if (_SFM_Instance == null)
                {
                    _SFM_Instance = Activator.CreateInstance(typeof(SourceFilterManager)) as ILogGuardModule;
                }
                return _SFM_Instance as SourceFilterManager;
            }
        }

        public static ViewModelManager? VMM_Instance
        {
            get
            {
                if (_VMM_Instance == null)
                {
                    _VMM_Instance = Activator.CreateInstance(typeof(ViewModelManager)) as ILogGuardModule;
                }
                return _VMM_Instance as ViewModelManager;
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
