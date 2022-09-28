using cyber_base.implement.utils;
using cyber_installer.@base;
using cyber_installer.definitions;
using cyber_installer.model;
using System.IO;
using System.Text;

namespace cyber_installer.implement.modules.user_config_manager
{
    internal class UserConfigManager : ICyberInstallerModule
    {
        private readonly string _userConfigFilePath = CyberInstallerDefinition.USER_CONFIG_FILE_PATH;
        private UserConfig? _currentUserConfig;

        public UserConfig CurrentConfig
        {
            get
            {
                return (_currentUserConfig?.Clone() as UserConfig) ?? GetDefaultUserConfig();
            }
        }

        public static UserConfigManager Current
        {
            get
            {
                return ModuleManager.UCM_Instance;
            }
        }

        public void OnModuleCreate()
        {
            if (File.Exists(_userConfigFilePath))
            {
                string json = File.ReadAllText(_userConfigFilePath, Encoding.UTF8);
                _currentUserConfig = JsonHelper.DeserializeObject<UserConfig>(json) ?? GetDefaultUserConfig();
            }
        }

        public void OnModuleDestroy()
        {
        }

        public void OnModuleStart()
        {
        }

        private UserConfig GetDefaultUserConfig()
        {
            return new UserConfig()
            {
                RemoteAdress = "http://107.127.131.89:8080"
            };
        }
    }
}
