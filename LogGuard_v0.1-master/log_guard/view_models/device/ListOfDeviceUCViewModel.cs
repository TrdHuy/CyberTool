using cyber_base.implement.utils;
using cyber_base.view_model;
using log_guard.@base.device;
using log_guard.implement.device;
using log_guard.view_models.command.device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.device
{
    internal class ListOfDeviceUCViewModel : BaseViewModel, IDeviceHolder
    {
        private RangeObservableCollection<IDeviceItem> _deviceItemVMs;
        private int _deviceCount;
        private bool _isLoadingDevice;

        [Bindable(true)]
        public LOF_ButtonCommand CommandViewModel { get; set; }

        [Bindable(true)]
        public RangeObservableCollection<IDeviceItem> DevicesSource
        {
            get
            {
                return _deviceItemVMs;
            }
            set
            {
                _deviceItemVMs = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public int DeviceCount
        {
            get
            {
                return _deviceCount;
            }
            set
            {
                _deviceCount = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsLoadingDevice
        {
            get
            {
                return _isLoadingDevice;
            }
            set
            {
                _isLoadingDevice = value;
                InvalidateOwn();
            }
        }

        public ListOfDeviceUCViewModel()
        {
            CommandViewModel = new LOF_ButtonCommand(this);
            DeviceManager.Current.AddDeviceHolder(this);
        }

        public ListOfDeviceUCViewModel(BaseViewModel baseViewModel) : base(baseViewModel)
        {
            CommandViewModel = new LOF_ButtonCommand(this);
            DeviceManager.Current.AddDeviceHolder(this);

        }

        public override void OnDestroy()
        {
            DeviceManager.Current.RemoveDeviceHolder(this);
        }

        public override void OnBegin()
        {
            IsLoadingDevice = true;
            DeviceManager.Current.FinishScanDevice -= OnFinishScanDevice;
            DeviceManager.Current.ForceUpdateListDevices();
            DeviceManager.Current.FinishScanDevice += OnFinishScanDevice;
            DeviceManager.Current.SerialPortChanged -= OnSerialPortChanged;
            DeviceManager.Current.SerialPortChanged += OnSerialPortChanged;
        }

        private void OnSerialPortChanged(object sender, EventArgs e)
        {
            IsLoadingDevice = true;
        }

        private void OnFinishScanDevice(object sender, EventArgs e)
        {
            IsLoadingDevice = false;
        }
    }
}
