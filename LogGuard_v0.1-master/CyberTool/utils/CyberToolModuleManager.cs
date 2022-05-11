using cyber_tool.plugins;
using cyber_tool.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_tool.utils
{
    public class CyberToolModuleManager
    {
        private static object? _CPM_Instance;
        private static object? _CSM_Insatace;

        public static void Init()
        {
            var CyberPluginsManager = CPM_Instance;
            var CyberServiceManager = CSM_Insatace;
        }

        public static CyberPluginsManager? CPM_Instance
        {
            get
            {
                if (_CPM_Instance == null)
                {
                    _CPM_Instance = Activator.CreateInstance(typeof(CyberPluginsManager), true);
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
                    _CSM_Insatace = Activator.CreateInstance(typeof(CyberServiceManager), true);
                }
                return _CSM_Insatace as CyberServiceManager;
            }
        }
    }
}
