﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.device
{
    internal interface IDeviceItem
    {
        string BuildNumber { get; }
        string SerialNumber { get; }
    }
}
