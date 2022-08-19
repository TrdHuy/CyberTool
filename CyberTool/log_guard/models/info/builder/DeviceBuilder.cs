using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.models.info.builder
{
    internal class DeviceBuilder
    {
        private DeviceInfo _deviceInfo;

        public DeviceInfo DeviceInfo
        {
            get { return _deviceInfo; }
        }

        // A fresh builder instance should contain a blank log object, which
        // is used in further assembly.
        public DeviceBuilder()
        {
            _deviceInfo = new DeviceInfo();
        }

        public void Reset()
        {
            this._deviceInfo = new DeviceInfo();
        }


        public DeviceBuilder BuildBNumber(object val)
        {
            _deviceInfo[DeviceInfo.KEY_BUILD_NUMBER] = val;
            return this;
        }

        public DeviceBuilder BuildSerialNumber(object val)
        {
            _deviceInfo[DeviceInfo.KEY_SERIAL_NUMBER] = val;
            return this;
        }

        public DeviceInfo Build()
        {
            return _deviceInfo;
        }
    }
}
