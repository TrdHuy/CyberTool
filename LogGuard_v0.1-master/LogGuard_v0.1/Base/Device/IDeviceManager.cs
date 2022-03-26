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

        DeviceItemViewModel SelectedDevice { get; set; }

        RangeObservableCollection<DeviceItemViewModel> DeviceSource { get; }

        List<IDeviceHolder> DeviceHolders { get; }

        void AddDeviceHolder(IDeviceHolder holder);

        void RemoveDeviceHolder(IDeviceHolder holder);

        event FinishScanDeviceHandler FinishScanDevice;
        event SelectedDeviceChangedHandler SelectedDeviceChanged;
        event SerialPortChangedHandler SerialPortChanged;
        event SelectedDeviceUnplugedHandler SelectedDeviceUnpluged;

    }

    public delegate void FinishScanDeviceHandler(object sender, EventArgs e);
    public delegate void SerialPortChangedHandler(object sender, EventArgs e);
    public delegate void SelectedDeviceChangedHandler(object sender, SelectedDeviceChangedEventArgs e);
    public delegate void SelectedDeviceUnplugedHandler(object sender, EventArgs e);

    public class SelectedDeviceChangedEventArgs : EventArgs
    {
        public DeviceItemViewModel OldDevice { get; }
        public DeviceItemViewModel NewDevice { get; }


        public SelectedDeviceChangedEventArgs(DeviceItemViewModel oldMd, DeviceItemViewModel newMd)
        {
            OldDevice = oldMd;
            NewDevice = newMd;
        }
    }
}
