using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.models.info
{
    internal class DeviceInfo
    {
        private Dictionary<string, object> _parts = new Dictionary<string, object>();
        public const string KEY_BUILD_NUMBER = "BuildNumber";
        public const string KEY_SERIAL_NUMBER = "SerialNumber";

        public DeviceInfo()
        {
        }

        public object this[string key]
        {
            get
            {
                try
                {
                    return _parts[key];
                }
                catch (Exception e)
                {
                    return null;
                }

                ;
            }

            set { _parts[key] = value; }
        }

    }
}
