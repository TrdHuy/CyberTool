﻿using cyber_base.async_task;
using cyber_base.implement.utils;
using cyber_installer.@base.async_task;
using cyber_installer.model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.ui_event_handler.async_task
{
    internal class InstallSoftwareTask : AbsParamAsyncTask
    {
        private const string INSTALLATION_INFO_FILE_NAME = "ci.json";
        private const string INSTALLATION_INFO_FOLDER_NAME = ".h2sw";

        private ToolData _installingToolData;
        private string _installPath;


        public InstallSoftwareTask(ToolData toolData
            , string installPath
            , Action<AsyncTaskResult>? callback = null
            , string name = "Installing") : base(param: toolData
                , name: name
                , completedCallback: callback
                , estimatedTime: 2000
                , reportDelay: 100)
        {
            _installingToolData = toolData;
            _installPath = installPath;
            _isEnableReport = false;
        }

        protected override bool IsTaskPossible(object param)
        {
            return true;
        }

        protected override async Task DoAsyncMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            await Task.Delay(1000);

            // Trường hợp version đang ở trạng thái chưa được cài đặt
            if (_installingToolData.ToolStatus != ToolStatus.Installed
                && _installingToolData.ToolVersionSource.Count > 0)
            {
                var latestVersionTool = _installingToolData.ToolVersionSource[0];
                var folderLocation = _installPath;
                var zipFilePath = latestVersionTool.DownloadFilePath;

                ZipFile.ExtractToDirectory(
                    sourceArchiveFileName: zipFilePath
                    , destinationDirectoryName: folderLocation
                    , overwriteFiles: true);

                File.Delete(zipFilePath);
                latestVersionTool.VersionStatus = ToolVersionStatus.VersionInstalled;
                _installingToolData.CurrentInstalledVersion = latestVersionTool.Version;
                _installingToolData.ExecutePath = _installPath + "\\" + latestVersionTool.ExecutePath;
                _installingToolData.InstallPath = _installPath;
                _installingToolData.ToolStatus = ToolStatus.Installed;

                var installationInfo = await ExportInstallationInfo();
                CreateUninstaller(installationInfo);

            }
        }

        private async Task<InstallationData> ExportInstallationInfo()
        {
            var installationInfoFilePath = CreateCyberInfoFileForInstalledSoftware();
            var installationInfo = new InstallationData()
            {
                ToolName = _installingToolData.ToolName,
                Guid = Guid.NewGuid().ToString("B"),
                ToolKey = _installingToolData.ToolKey,
                CurrentInstalledVersion = _installingToolData.CurrentInstalledVersion,
                ExecutePath = _installingToolData.ExecutePath,
                InstallPath = _installingToolData.InstallPath,
            };
            var installationInfoJson = JsonHelper.SerializeObject(installationInfo);
            await File.WriteAllTextAsync(installationInfoFilePath, installationInfoJson);
            return installationInfo;
        }
        private string CreateCyberInfoFileForInstalledSoftware()
        {
            var installationInfoFolderPath = _installPath + "\\" + INSTALLATION_INFO_FOLDER_NAME;
            if (!Directory.Exists(installationInfoFolderPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(installationInfoFolderPath);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            var installationInfoFilePath = installationInfoFolderPath + "\\" + INSTALLATION_INFO_FILE_NAME;
            if (!File.Exists(installationInfoFilePath))
            {
                File.Create(installationInfoFilePath).Dispose();
            }
            return installationInfoFilePath;
        }

        private void CreateUninstaller(InstallationData installationInfo)
        {
            using (RegistryKey? parent = Registry.LocalMachine.OpenSubKey(
                         @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {
                var uninstallPath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;

                if (parent == null)
                {
                    throw new Exception("Uninstall registry key not found.");
                }
                if (string.IsNullOrEmpty(uninstallPath)) return;
                try
                {
                    RegistryKey? key = null;

                    try
                    {

                        string guidText = installationInfo.Guid;
                        key = parent.OpenSubKey(guidText, true) ??
                              parent.CreateSubKey(guidText);

                        if (key == null)
                        {
                            throw new Exception(String.Format("Unable to create uninstaller '{0}\\{1}'", guidText, guidText));
                        }
                        key.SetValue("DisplayName", installationInfo.ToolName);
                        key.SetValue("ApplicationVersion", installationInfo.CurrentInstalledVersion.Trim());
                        key.SetValue("Publisher", "h2sw");
                        key.SetValue("DisplayIcon", installationInfo.ExecutePath);
                        key.SetValue("DisplayVersion", installationInfo.ToolName);
                        key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                        key.SetValue("UninstallString", uninstallPath);
                    }
                    finally
                    {
                        if (key != null)
                        {
                            key.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        "An error occurred writing uninstall information to the registry.  The service is fully installed but can only be uninstalled manually through the command line.",
                        ex);
                }
            }
        }
    }
}