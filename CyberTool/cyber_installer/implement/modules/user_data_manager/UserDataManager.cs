﻿using cyber_base.implement.utils;
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
    internal class UserDataManager : IUserDataManager
    {
        private readonly bool IS_IMPORT_DATA_ASYNC = false;
        private readonly string _userDataFilePath = CyberInstallerDefinition.USER_DATA_FILE_PATH;
        private UserData _currentUserData;
        private Task? _importUserDataFromFileTaskCache;

        public static UserDataManager Current
        {
            get => ModuleManager.UDM_Instance;
        }

        public UserData CurrentUserData => _currentUserData;

        public async Task ExportUserDataToFile()
        {
            if (!File.Exists(_userDataFilePath))
            {
                File.Create(_userDataFilePath).Dispose();
            }
            string dataJson = JsonHelper.SerializeObject(_currentUserData);
            await File.WriteAllTextAsync(_userDataFilePath, dataJson);
        }
        public void OnModuleCreate()
        {
            ImportUserDataFromFile(IS_IMPORT_DATA_ASYNC);
        }

        public async void OnModuleDestroy()
        {
            await ExportUserDataToFile();
        }

        public void OnModuleStart()
        {
        }

        public void UpdateUserData(UserData newData)
        {
            _currentUserData = newData;
        }

        private async void ImportUserDataFromFile(bool isAsync = false)
        {
            if (isAsync)
            {
                _importUserDataFromFileTaskCache = ImportUserDataFromFileTaskAsync();
                await _importUserDataFromFileTaskCache;
            }
            else
            {
                ImportUserDataFromFileTask();
            }

        }

        private void ImportUserDataFromFileTask()
        {
            if (!File.Exists(_userDataFilePath))
            {
                File.Create(_userDataFilePath).Dispose();
            }

            string json = File.ReadAllText(_userDataFilePath, Encoding.UTF8);
            _currentUserData = JsonHelper.DeserializeObject<UserData>(json);
        }

        private async Task ImportUserDataFromFileTaskAsync()
        {
            if (!File.Exists(_userDataFilePath))
            {
                File.Create(_userDataFilePath).Dispose();
            }

            string json = await File.ReadAllTextAsync(_userDataFilePath, Encoding.UTF8);
            _currentUserData = JsonHelper.DeserializeObject<UserData>(json);
        }

        public async Task<bool> WaitForImportUserDataTask(int timeOutMilliseconds)
        {

            if (_importUserDataFromFileTaskCache != null)
            {
                await _importUserDataFromFileTaskCache.WaitAsync(TimeSpan.FromMilliseconds(timeOutMilliseconds));
                return _importUserDataFromFileTaskCache.IsCompletedSuccessfully;
            }
            else if (!IS_IMPORT_DATA_ASYNC)
            {
                return true;
            }

            return false;
        }
    }
}
