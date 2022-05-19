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

        static LogGuardModuleManager()
        {
            _Modules = new Collection<ILogGuardModule>();
        }

        public static void Init()
        {
            _Modules.Clear();

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

        public static StateController SC_Instance
        {
            get
            {
                if (_SC_Instance == null)
                {
                    if (RUNE.IS_SUPPORT_HIGH_CPU_LOG_CAPTURE)
                        _SC_Instance = new HighCpu_StateController();

                    else
                        _SC_Instance = new LowCpu_StateController();
                }
                return (StateController)_SC_Instance;
            }
        }
        public static LogGuardCommandExecuterFactory LGCEF_Instance
        {
            get
            {
                if (_LGCEF_Instance == null)
                {
                    _LGCEF_Instance = new LogGuardCommandExecuterFactory();
                }
                return (LogGuardCommandExecuterFactory)_LGCEF_Instance;
            }
        }

        public static LogGuardKeyActionListener LGKAL_Instance
        {
            get
            {
                if (_LGKAL_Instance == null)
                {
                    _LGKAL_Instance = new LogGuardKeyActionListener();
                }
                return (LogGuardKeyActionListener)_LGKAL_Instance;
            }
        }
        public static LogInfoManager LIF_Instance
        {
            get
            {
                if (_LIF_Instance == null)
                {
                    _LIF_Instance = new LogInfoManager();
                }
                return (LogInfoManager)_LIF_Instance;
            }
        }
        public static RunThreadConfigManager RTCM_Instance
        {
            get
            {
                if (_RTCM_Instance == null)
                {
                    _RTCM_Instance = new RunThreadConfigManager();
                }
                return (RunThreadConfigManager)_RTCM_Instance;
            }
        }
        public static SourceHighlightManager SHM_Instance
        {
            get
            {
                if (_SHM_Instance == null)
                {
                    _SHM_Instance = new SourceHighlightManager();
                }
                return (SourceHighlightManager)_SHM_Instance;
            }

        }
        public static SourceManager SM_Instance
        {
            get
            {
                if (_SM_Instance == null)
                {
                    _SM_Instance = new SourceManager();
                }
                return (SourceManager)_SM_Instance;
            }

        }
        public static SourceFilterManager SFM_Instance
        {
            get
            {
                if (_SFM_Instance == null)
                {
                    _SFM_Instance = new SourceFilterManager();
                }
                return (SourceFilterManager)_SFM_Instance;
            }
        }

        public static ViewModelManager VMM_Instance
        {
            get
            {
                if (_VMM_Instance == null)
                {
                    _VMM_Instance = new ViewModelManager();
                }
                return (ViewModelManager)_VMM_Instance;
            }
        }

        public static ProcessManager PM_Instance
        {
            get
            {
                if (_PM_Instance == null)
                {
                    _PM_Instance = new ProcessManager();
                }
                return (ProcessManager)_PM_Instance;
            }
        }

        public static LogGuardViewHelper LGVH_Instance
        {
            get
            {
                if (_LGVH_Instance == null)
                {
                    _LGVH_Instance = new LogGuardViewHelper();
                }
                return (LogGuardViewHelper)_LGVH_Instance;
            }
        }

        public static DeviceManager DM_Instance
        {
            get
            {
                if (_DM_Instance == null)
                {
                    _DM_Instance = new DeviceManager();
                }
                return (DeviceManager)_DM_Instance;
            }
        }

        public static DeviceCmdExecuter DCE_Instance
        {
            get
            {
                if (_DCE_Instance == null)
                {
                    _DCE_Instance = new DeviceCmdExecuter();
                }
                return (DeviceCmdExecuter)_DCE_Instance;
            }
        }

    }
}
