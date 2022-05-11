using log_guard.flow.view_helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.flow
{
    public static class LogGuardSingletonController
    {
        private static object _LG_ViewHelper;

        public static LogGuardViewHelper LG_ViewHelper
        {
            get
            {
                if(_LG_ViewHelper == null)
                {
                    _LG_ViewHelper = Activator.CreateInstance(typeof(LogGuardViewHelper));
                }
                return _LG_ViewHelper as LogGuardViewHelper;
            }
        }
    }
}
