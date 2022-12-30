using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.definitions
{
    public class CyberInstallerDefinition
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
        public const int IMPORT_USER_DATA_TIME_OUT = 1000;
        public const int AFTER_KILL_PROCESS_WAIT_TIME = 2000;

        public const string INSTALLATION_INFO_FILE_NAME = "ci.json";
        public const string INSTALLATION_INFO_FOLDER_NAME = ".h2sw";

        public enum ManageableTaskKeyDefinition
        {
            [ManageableTaskInfo(description:"Update and install latest version of Cyber Installer"
                , name:"Updating")]
            UPDATE_CYBER_INSTALLER_TASK_TYPE_KEY = 1,

            [ManageableTaskInfo(description:"Uninstall the software which has been setup on local machine via Cyber Installer"
                , name:"Uninstalling")]
            UNINSTALL_SOFTWARE_TASK_TYPE_KEY = 2,

            [ManageableTaskInfo(description: "Download the software from Cyber server and then install it on current local machine via Cyber Installer"
               , name: "Download and install")]
            DOWNLOAD_AND_INSTALL_SOFTWARE_TASK_TYPE_KEY = 3,

            [ManageableTaskInfo(description: "Download the latest version software from Cyber server and then install it on current local machine via Cyber Installer"
               , name: "Updating")]
            UPDATE_SOFTWARE_TASK_TYPE_KEY = 4,
        }


    }

    internal class CyberInstallerKeyFeatureTag
    {
        public const string KEY_TAG_SWI_AT_DOWNLOAD_AND_INSTALL_FEATURE = "KEY_TAG_SWI_AT_DOWNLOAD_AND_INSTALL_FEATURE";
        public const string KEY_TAG_SWI_AT_UNISTALL_FEATURE = "KEY_TAG_SWI_AT_UNISTALL_FEATURE";
        public const string KEY_TAG_SWI_AT_UPDATE_SOFTWARE_FEATURE = "KEY_TAG_SWI_AT_UPDATE_SOFTWARE_FEATURE";
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ManageableTaskInfoAttribute : Attribute
    {
        public string Description { get; private set; }
        public string Name { get; private set; }
        public ManageableTaskInfoAttribute(string description, string name)
        {
            this.Name = name;
            this.Description = description;
        }
    }
   
}
