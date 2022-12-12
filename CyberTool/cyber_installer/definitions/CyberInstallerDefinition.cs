using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.definitions
{
    internal class CyberInstallerDefinition
    {
        public static readonly string CONFIG_FOLDER_BASE_PATH = AppDomain.CurrentDomain.BaseDirectory
            + "\\.config";
        public static readonly string DATA_FOLDER_BASE_PATH = AppDomain.CurrentDomain.BaseDirectory
            + "\\.data";
        public static readonly string USER_CONFIG_FILE_BASE_PATH = AppDomain.CurrentDomain.BaseDirectory
            + "\\.config\\user_config.json";
        public static readonly string USER_DATA_FILE_BASE_PATH = AppDomain.CurrentDomain.BaseDirectory
            + "\\.data\\user_data.json";

        public const string CIBS_CALLER_ID = "CyberInstallerWindow{0367E847-B5C3-4CDD-9C34-B78A769AF73C}";
        public const string CIBS_UPDATE_CYBER_INSTALLER_CMD = "UpdateCyberInstaller";
        public static readonly string CIBS_BASE_PATH = AppDomain.CurrentDomain.BaseDirectory + "\\cibs\\cibs.exe";
        public static readonly string CIBS_FOLDER_BASE_PATH = AppDomain.CurrentDomain.BaseDirectory + "\\cibs";
        public static readonly string CIBS_FOLDER_ZIP_PATH = "cibs";
        public const string CYBER_INSTALLER_INDENTIFER = "h2s_swi";
        public const string CYBER_INSTALLER_SW_ID = "cyberinstaller";

        public const int CHECK_UPDATE_CYBER_INSTALLER_INTERVAL_TIME = 50000;
        public const int CHECK_UPDATE_CYBER_INSTALLER_DELAY_AFTER_MAIN_WINDOW_SHOWED = 2000;
        public const int REQUEST_CIBS_UPDATE_CYBER_INSTALLER_DELAY_TIME = 2000;

        public const string UPDATE_CYBER_INSTALLER_TASK_TYPE_KEY = "UPDATE_CYBER_INSTALLER_TASK_TYPE_KEY";
        public const string UPDATE_CYBER_INSTALLER_TASK_NAME = "Updating cyber installer";

    }

    internal class CyberInstallerKeyFeatureTag
    {
        public const string KEY_TAG_SWI_AT_DOWNLOAD_AND_INSTALL_FEATURE = "KEY_TAG_SWI_AT_DOWNLOAD_AND_INSTALL_FEATURE";
    }

}
