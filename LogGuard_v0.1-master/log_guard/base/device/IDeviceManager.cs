using cyber_base.implement.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.device
{
    internal interface IDeviceManager
    {
        void UpdateListDevices();

        /// <summary>
        /// force update the device list without open a waiting box
        /// </summary>
        void ForceUpdateListDevices();

        IDeviceItem SelectedDevice { get; set; }

        RangeObservableCollection<IDeviceItem> DeviceSource { get; }

        List<IDeviceHolder> DeviceHolders { get; }

        void AddDeviceHolder(IDeviceHolder holder);

        void RemoveDeviceHolder(IDeviceHolder holder);

        event FinishScanDeviceHandler FinishScanDevice;
        event SelectedDeviceChangedHandler SelectedDeviceChanged;
        event SerialPortChangedHandler SerialPortChanged;
        event SelectedDeviceUnplugedHandler SelectedDeviceUnpluged;

    }

    internal delegate void FinishScanDeviceHandler(object sender, EventArgs e);
    internal delegate void SerialPortChangedHandler(object sender, EventArgs e);
    internal delegate void SelectedDeviceChangedHandler(object sender, SelectedDeviceChangedEventArgs e);
    internal delegate void SelectedDeviceUnplugedHandler(object sender, EventArgs e);

    internal class SelectedDeviceChangedEventArgs : EventArgs
    {
        public IDeviceItem OldDevice { get; }
        public IDeviceItem NewDevice { get; }

        public SelectedDeviceChangedEventArgs(IDeviceItem oldMd, IDeviceItem newMd)
        {
            OldDevice = oldMd;
            NewDevice = newMd;
        }
    }
}
