using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.model
{
    internal class UserConfig : ICloneable
    {
        public string RemoteAdress { get; set; } = "";

        public object Clone()
        {
            return new UserConfig()
            {
                RemoteAdress = this.RemoteAdress,
            };
        }
    }
}
