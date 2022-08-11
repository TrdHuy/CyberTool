using cyber_base.implement.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.device
{
    internal interface IDeviceHolder
    {
        RangeObservableCollection<IDeviceItem> DevicesSource { get; set; }

        int DeviceCount { get; set; }
    }
}
