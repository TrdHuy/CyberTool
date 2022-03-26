using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.Device
{
    public interface IDeviceManager
    {
        void UpdateListDevices();

        /// <summary>
        /// force update the device list without open a waiting box
        /// </summary>
        void ForceUpdateListDevices();

        RangeObservableCollection<DeviceItemViewModel> DeviceSource { get; }

        List<IDeviceHolder> DeviceHolders { get; }

        void AddDeviceHolder(IDeviceHolder holder);

        void RemoveDeviceHolder(IDeviceHolder holder);

        event FinishScanDeviceHandler FinishScanDevice;

    }

    public delegate void FinishScanDeviceHandler(object sender);
}
