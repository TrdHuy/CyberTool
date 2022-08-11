using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Device;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.Device
{
    public interface IDeviceHolder
    {
        RangeObservableCollection<DeviceItemViewModel> DevicesSource { get; set; }

        int DeviceCount { get; set; }
    }
}
