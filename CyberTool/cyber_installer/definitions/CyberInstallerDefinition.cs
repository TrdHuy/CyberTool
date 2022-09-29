using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.definitions
{
    internal class CyberInstallerDefinition
    {
        public static readonly string USER_CONFIG_FILE_PATH = AppDomain.CurrentDomain.BaseDirectory
            + ".config/user_config.json";
        public static readonly string USER_DATA_FILE_PATH = AppDomain.CurrentDomain.BaseDirectory
            + ".data/user_data.json";
    }
}
