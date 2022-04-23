using LogGuard_v0._1.Base.Device;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.Device;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCListOfDevice
{
    public class ListOfDeviceUCViewModel : BaseViewModel, IDeviceHolder
    {
        private RangeObservableCollection<DeviceItemViewModel> _deviceItemVMs;
        private int _deviceCount;
        private bool _isLoadingDevice;

        [Bindable(true)]
        public MSW_UC_ListOfDeviceControlButtonCommand CommandViewModel { get; set; }

        [Bindable(true)]
        public RangeObservableCollection<DeviceItemViewModel> DevicesSource
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
            CommandViewModel = new MSW_UC_ListOfDeviceControlButtonCommand(this);
            DeviceManagerImpl.Current.AddDeviceHolder(this);
        }

        public ListOfDeviceUCViewModel(BaseViewModel baseViewModel) : base(baseViewModel)
        {
            CommandViewModel = new MSW_UC_ListOfDeviceControlButtonCommand(this);
            DeviceManagerImpl.Current.AddDeviceHolder(this);

        }

        public override void OnDestroy()
        {
            DeviceManagerImpl.Current.RemoveDeviceHolder(this);
        }

        public override void OnBegin()
        {
            IsLoadingDevice = true;
            DeviceManagerImpl.Current.FinishScanDevice -= OnFinishScanDevice;
            DeviceManagerImpl.Current.ForceUpdateListDevices();
            DeviceManagerImpl.Current.FinishScanDevice += OnFinishScanDevice;
            DeviceManagerImpl.Current.SerialPortChanged -= OnSerialPortChanged;
            DeviceManagerImpl.Current.SerialPortChanged += OnSerialPortChanged;
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
