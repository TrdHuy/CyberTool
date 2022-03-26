using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Windows.MainWindow.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Device
{
    public class DeviceItemViewModel : BaseViewModel
    {
        private DeviceInfo _deviceInfo;

        public DeviceItemViewModel(DeviceInfo deviceInfo)
        {
            this._deviceInfo = deviceInfo;
        }
        
        [Bindable(true)]
        public object BuildNumber
        {
            get
            {
                return _deviceInfo[DeviceInfo.KEY_BUILD_NUMBER];
            }
        }

        [Bindable(true)]
        public object SerialNumber
        {
            get
            {
                return _deviceInfo[DeviceInfo.KEY_SERIAL_NUMBER];
            }
        }

        public override string ToString()
        {
            return BuildNumber+"";
        }

    }
}
