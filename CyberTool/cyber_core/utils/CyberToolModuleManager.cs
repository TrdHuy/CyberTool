using cyber_core.@base.module;
using cyber_core.plugins;
using cyber_core.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_core.utils
{
    public class CyberToolModuleManager
    {
        private static Collection<ICyberModule> _CyberModules;
        private static ICyberModule _CPM_Instance;
        private static ICyberModule _CSM_Insatace;
        private static ICyberModule _CSC_Insatace;

        public static void Init()
        {
            _CyberModules = new Collection<ICyberModule>();

            _CyberModules.Add(CPM_Instance);
            _CyberModules.Add(CSM_Insatace);
            _CyberModules.Add(CSC_Insatace);


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

        public static CyberPluginsManager CPM_Instance
        {
            get
            {
                if (_CPM_Instance == null)
                {
                    _CPM_Instance = Activator.CreateInstance(typeof(CyberPluginsManager), true) as ICyberModule;
                }
                return (CyberPluginsManager)_CPM_Instance;
            }
        }


        public static CyberServiceManager CSM_Insatace
        {
            get
            {
                if (_CSM_Insatace == null)
                {
                    _CSM_Insatace = Activator.CreateInstance(typeof(CyberServiceManager), true) as ICyberModule;
                }
                return (CyberServiceManager)_CSM_Insatace;
            }
        }

        internal static CyberServiceController CSC_Insatace
        {
            get
            {
                if (_CSC_Insatace == null)
                {
                    _CSC_Insatace = Activator.CreateInstance(typeof(CyberServiceController), true) as ICyberModule;
                }
                return (CyberServiceController)_CSC_Insatace;
            }
        }
    }
}
