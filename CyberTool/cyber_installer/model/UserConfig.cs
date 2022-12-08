using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.model
{
    internal struct UserConfig
    {
        public UserConfig()
        {
        }

        public string RemoteAdress { get; set; } = "";
        public string SSLRemoteAdress { get; set; } = "";
    }
}
