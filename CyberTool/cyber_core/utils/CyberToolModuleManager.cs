using cyber_core.@base.module;
using cyber_core.services;
using System;
using System.Collections.ObjectModel;

namespace cyber_core.utils
{
    public class CyberToolModuleManager
    {
        private static Collection<ICyberCoreModule> _CyberModules = new Collection<ICyberCoreModule>();
        private static ICyberCoreModule? _CSM_Insatace;
        private static ICyberCoreModule? _CSC_Insatace;

        public static void Init()
        {
            _CyberModules.Clear();

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

        public static void Destroy()
        {
            foreach (var module in _CyberModules)
            {
                module.OnModuleDestroy();
            }
        }

        public static void OnIFaceShowed()
        {
            foreach (var module in _CyberModules)
            {
                module.OnIFaceWindowShowed();
            }
        }
        

        public static CyberServiceManager CSM_Insatace
        {
            get
            {
                if (_CSM_Insatace == null)
                {
                    _CSM_Insatace = Activator.CreateInstance(typeof(CyberServiceManager), true) as ICyberCoreModule;
                }
                ArgumentNullException.ThrowIfNull(_CSM_Insatace);
                return (CyberServiceManager)_CSM_Insatace;
            }
        }

        internal static CyberServiceController CSC_Insatace
        {
            get
            {
                if (_CSC_Insatace == null)
                {
                    _CSC_Insatace = Activator.CreateInstance(typeof(CyberServiceController), true) as ICyberCoreModule;
                }
                ArgumentNullException.ThrowIfNull(_CSC_Insatace);
                return (CyberServiceController)_CSC_Insatace;
            }
        }
    }
}
