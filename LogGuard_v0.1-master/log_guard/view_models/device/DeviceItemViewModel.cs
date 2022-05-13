using cyber_base.view_model;
using log_guard.@base.device;
using log_guard.models.info;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.device
{
    internal class DeviceItemViewModel : BaseViewModel, IDeviceItem
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
            return BuildNumber + "";
        }

    }
}
