using honeyboard_release_service.@base.module;
using honeyboard_release_service.implement.async_task_execute_helper;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.implement.ui_event_handler;
using honeyboard_release_service.implement.view_helper;
using honeyboard_release_service.implement.view_manager.notebook_header;
using honeyboard_release_service.implement.view_manager.notebook_item;
using honeyboard_release_service.implement.view_model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.module
{

    internal class PublisherModuleManager
    {
        private static Collection<IPublisherModule> _Modules;

        private static IPublisherModule? _VMM_Instance;
        private static IPublisherModule? _CNHVM_Instance;
        private static IPublisherModule? _CNIVM_Instance;
        private static IPublisherModule? _PVH_Instance;
        private static IPublisherModule? _SPCEF_Instance;
        private static IPublisherModule? _PKAL_Instance;
        private static IPublisherModule? _ATM_Instance;
        private static IPublisherModule? _RPM_Instance;
        
        static PublisherModuleManager()
        {
            _Modules = new Collection<IPublisherModule>();
        }

        public static void Init()
        {
            _Modules.Clear();

            _Modules.Add(VMM_Instance);
            _Modules.Add(PVH_Instance);
            _Modules.Add(CNIVM_Instance);
            _Modules.Add(CNHVM_Instance);
            _Modules.Add(SPCEF_Instance);
            _Modules.Add(PKAL_Instance);
            _Modules.Add(ATM_Instance);
            _Modules.Add(RPM_Instance);

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
            _Modules.Clear();
            _VMM_Instance = null;
            _PVH_Instance = null;
            _CNHVM_Instance = null;
            _CNIVM_Instance = null;
            _SPCEF_Instance = null;
            _PKAL_Instance = null;
            _ATM_Instance = null;
            _RPM_Instance = null;
        }

        public static ReleasingProjectManager RPM_Instance
        {
            get
            {
                if (_RPM_Instance == null)
                {
                    _RPM_Instance = Activator.CreateInstance(typeof(ReleasingProjectManager), true) as ReleasingProjectManager; ;
                }
                ArgumentNullException.ThrowIfNull(_RPM_Instance);
                return (ReleasingProjectManager)_RPM_Instance;
            }
        }

        public static AsyncTaskManager ATM_Instance
        {
            get
            {
                if (_ATM_Instance == null)
                {
                    _ATM_Instance = Activator.CreateInstance(typeof(AsyncTaskManager), true) as AsyncTaskManager; ;
                }
                ArgumentNullException.ThrowIfNull(_ATM_Instance);
                return (AsyncTaskManager)_ATM_Instance;
            }
        }

        public static ViewModelManager VMM_Instance
        {
            get
            {
                if (_VMM_Instance == null)
                {
                    _VMM_Instance = Activator.CreateInstance(typeof(ViewModelManager), true) as ViewModelManager; ;
                }
                ArgumentNullException.ThrowIfNull(_VMM_Instance);
                return (ViewModelManager)_VMM_Instance;
            }
        }

        public static CalendarNotebookHeaderViewManager CNHVM_Instance
        {
            get
            {
                if (_CNHVM_Instance == null)
                {
                    _CNHVM_Instance = Activator.CreateInstance(typeof(CalendarNotebookHeaderViewManager), true) as CalendarNotebookHeaderViewManager;
                }
                ArgumentNullException.ThrowIfNull(_CNHVM_Instance);
                return (CalendarNotebookHeaderViewManager)_CNHVM_Instance;
            }
        }

        public static CalendarNotebookItemViewManager CNIVM_Instance
        {
            get
            {
                if (_CNIVM_Instance == null)
                {
                    _CNIVM_Instance = Activator.CreateInstance(typeof(CalendarNotebookItemViewManager), true) as CalendarNotebookItemViewManager;
                }
                ArgumentNullException.ThrowIfNull(_CNIVM_Instance);
                return (CalendarNotebookItemViewManager)_CNIVM_Instance;
            }
        }

        public static SwPublisherCommandExecuterFactory SPCEF_Instance
        {
            get
            {
                if (_SPCEF_Instance == null)
                {
                    _SPCEF_Instance = Activator.CreateInstance(typeof(SwPublisherCommandExecuterFactory), true) as SwPublisherCommandExecuterFactory;
                }
                ArgumentNullException.ThrowIfNull(_SPCEF_Instance);
                return (SwPublisherCommandExecuterFactory)_SPCEF_Instance;
            }
        }

        public static PublisherKeyActionListener PKAL_Instance
        {
            get
            {
                if (_PKAL_Instance == null)
                {
                    _PKAL_Instance = Activator.CreateInstance(typeof(PublisherKeyActionListener), true) as PublisherKeyActionListener;
                }
                ArgumentNullException.ThrowIfNull(_PKAL_Instance);
                return (PublisherKeyActionListener)_PKAL_Instance;
            }
        }

        public static PublisherViewHelper PVH_Instance
        {
            get
            {
                if (_PVH_Instance == null)
                {
                    _PVH_Instance = new PublisherViewHelper();
                }
                return (PublisherViewHelper)_PVH_Instance;
            }
        }

    }
}
