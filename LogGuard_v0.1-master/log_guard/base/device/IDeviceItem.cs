using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.device
{
    internal interface IDeviceItem
    {
        object BuildNumber { get; }
        object SerialNumber { get; }
    }
}
