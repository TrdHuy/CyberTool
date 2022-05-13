using cyber_tool.@base.module;
using cyber_tool.plugins;
using cyber_tool.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_tool.utils
{
    public class CyberToolModuleManager
    {
        private static Collection<ICyberModule> _CyberModules;
        private static ICyberModule? _CPM_Instance;
        private static ICyberModule? _CSM_Insatace;
        private static ICyberModule? _CSC_Insatace;

        public static void Init()
        {
            _CyberModules = new Collection<ICyberModule>();
            var CyberPluginsManager = CPM_Instance;
            var CyberServiceManager = CSM_Insatace;
            var CyberServiceController = CSC_Insatace;

            _CyberModules.Add(CyberPluginsManager);
            _CyberModules.Add(CyberServiceManager);
            _CyberModules.Add(CyberServiceController);


            foreach (var module in _CyberModules)
            {
                module.OnModuleInit();
            }

            foreach (var module in _CyberModules)
            {
                module.OnModuleStart();
            }
        }

        public static void OnIFaceShowed()
        {
            foreach (var module in _CyberModules)
            {
                module.OnIFaceWindowShowed();
            }
        }

        public static CyberPluginsManager? CPM_Instance
        {
            get
            {
                if (_CPM_Instance == null)
                {
                    _CPM_Instance = Activator.CreateInstance(typeof(CyberPluginsManager), true) as ICyberModule;
                }
                return _CPM_Instance as CyberPluginsManager;
            }
        }


        public static CyberServiceManager? CSM_Insatace
        {
            get
            {
                if (_CSM_Insatace == null)
                {
                    _CSM_Insatace = Activator.CreateInstance(typeof(CyberServiceManager), true) as ICyberModule;
                }
                return _CSM_Insatace as CyberServiceManager;
            }
        }

        internal static CyberServiceController? CSC_Insatace
        {
            get
            {
                if (_CSC_Insatace == null)
                {
                    _CSC_Insatace = Activator.CreateInstance(typeof(CyberServiceController), true) as ICyberModule;
                }
                return _CSC_Insatace as CyberServiceController;
            }
        }
    }
}
