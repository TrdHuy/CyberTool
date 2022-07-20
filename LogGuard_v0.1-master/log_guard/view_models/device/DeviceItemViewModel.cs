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
        public string BuildNumber
        {
            get
            {
                return _deviceInfo[DeviceInfo.KEY_BUILD_NUMBER].ToString();
            }
        }

        [Bindable(true)]
        public string SerialNumber
        {
            get
            {
                return _deviceInfo[DeviceInfo.KEY_SERIAL_NUMBER].ToString();
            }
        }

        public override string ToString()
        {
            return BuildNumber + "";
        }

    }
}
