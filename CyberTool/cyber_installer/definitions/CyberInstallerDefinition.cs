using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.definitions
{
    internal class CyberInstallerDefinition
    {
        public static readonly string CONFIG_FOLDER_PATH = AppDomain.CurrentDomain.BaseDirectory
            + ".config/";
        public static readonly string USER_CONFIG_FILE_PATH = AppDomain.CurrentDomain.BaseDirectory
            + ".config/user_config.json";
        public static readonly string USER_DATA_FILE_PATH = AppDomain.CurrentDomain.BaseDirectory
            + ".data/user_data.json";
        public const string CYBER_INSTALLER_INDENTIFER = "h2s_swi";

    }

    internal class CyberInstallerKeyFeatureTag
    {
        public const string KEY_TAG_SWI_AT_DOWNLOAD_AND_INSTALL_FEATURE = "KEY_TAG_SWI_AT_DOWNLOAD_AND_INSTALL_FEATURE";
    }

}
