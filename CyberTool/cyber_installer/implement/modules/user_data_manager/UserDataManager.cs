using cyber_base.implement.utils;
using cyber_installer.@base;
using cyber_installer.@base.modules;
using cyber_installer.definitions;
using cyber_installer.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.user_data_manager
{
    internal class UserDataManager : ICyberInstallerModule, IUserDataManager
    {
        private readonly string _userDataFilePath = CyberInstallerDefinition.USER_DATA_FILE_PATH;
        private UserData _currentUserData;

        public static UserDataManager Current
        {
            get => ModuleManager.UDM_Instance;
        }

        public UserData CurrentUserData => _currentUserData;

        public async void ExportUserDataToFile()
        {
            if (!File.Exists(_userDataFilePath))
            {
                File.Create(_userDataFilePath).Dispose();
            }
            string dataJson = JsonHelper.SerializeObject(_currentUserData);
            await File.WriteAllTextAsync(_userDataFilePath, dataJson);
        }

        public void ImportUserDataFromFile()
        {
            if (!File.Exists(_userDataFilePath))
            {
                File.Create(_userDataFilePath).Dispose();
            }

            string json = File.ReadAllText(_userDataFilePath, Encoding.UTF8);
            _currentUserData = JsonHelper.DeserializeObject<UserData>(json);
        }

        public void OnModuleCreate()
        {
            ImportUserDataFromFile();
        }

        public void OnModuleDestroy()
        {
            ExportUserDataToFile();
        }

        public void OnModuleStart()
        {
        }

        public void UpdateUserData(UserData newData)
        {
            _currentUserData = newData;
        }
    }
}
