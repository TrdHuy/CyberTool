using log_guard.@base.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.module
{
    public class BaseLogGuardModule : ILogGuardModule
    {
        public virtual void OnModuleDestroy()
        {
        }

        public virtual void OnModuleStart()
        {
        }
    }
}
