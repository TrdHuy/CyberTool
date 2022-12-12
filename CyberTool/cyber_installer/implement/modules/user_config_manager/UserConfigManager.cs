using cyber_base.implement.utils;
using cyber_installer.@base;
using cyber_installer.definitions;
using cyber_installer.implement.modules.utils;
using cyber_installer.model;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.user_config_manager
{
    internal class UserConfigManager : BaseCyberInstallerModule
    {
        private readonly string _configFolderPath = CyberInstallerDefinition.CONFIG_FOLDER_BASE_PATH;
        private readonly string _userConfigFilePath = CyberInstallerDefinition.USER_CONFIG_FILE_BASE_PATH;
        private UserConfig _currentUserConfig;

        public UserConfig CurrentConfig
        {
            get
            {
                return _currentUserConfig;
            }
        }

        public static UserConfigManager Current
        {
            get
            {
                return ModuleManager.UCM_Instance;
            }
        }

        public override void OnModuleCreate()
        {
            Utils.CreateIsNotExistFile(_userConfigFilePath
                , parentFolderAttr: FileAttributes.Directory | FileAttributes.Hidden);

            string json = File.ReadAllText(_userConfigFilePath, Encoding.UTF8);
            _currentUserConfig = JsonHelper.DeserializeObject<UserConfig>(json);
            if (string.IsNullOrEmpty(_currentUserConfig.RemoteAdress))
            {
                _currentUserConfig = GetDefaultUserConfig();
                ExportConfigToFile();
            }
        }

        private async void ExportConfigToFile()
        {
            string configJson = JsonHelper.SerializeObject(_currentUserConfig);
            await File.WriteAllTextAsync(_userConfigFilePath, configJson);
        }

        private UserConfig GetDefaultUserConfig()
        {
            return new UserConfig()
            {
                RemoteAdress = "http://107.98.32.108:8080",
                SSLRemoteAdress = "https://107.98.32.108:8088"
            };
        }
    }
}
