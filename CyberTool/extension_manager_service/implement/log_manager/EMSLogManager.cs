using cyber_base.implement.utils;
using extension_manager_service.implement.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.implement.log_manager
{
    internal class EMSLogManager : BaseExtensionManagerModule
    {
        private Logger _emsLogger;
        public static EMSLogManager Current
        {
            get => ModuleManager.ELM_Instance;
        }

        private EMSLogManager()
        {
            _emsLogger = new Logger("", Assembly.GetExecutingAssembly().GetName().Name ?? "extension_manager");
        }

        public static void D(string log, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "")
        {
            Current._emsLogger.D(log, caller, filePath);
        }

        public static void E(string log, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "")
        {
            Current._emsLogger.E(log, caller, filePath);
        }

        public static void I(string log, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "")
        {
            Current._emsLogger.I(log, caller, filePath);
        }

        public static void W(string log, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "")
        {
            Current._emsLogger.W(log, caller, filePath);
        }

        public static void F(string log, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "")
        {
            Current._emsLogger.F(log, caller, filePath);
        }

        public static void V(string log, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "")
        {
            Current._emsLogger.V(log, caller, filePath);
        }
    }
}
