using cyber_base.async_task;
using cyber_base.implement.utils;
using cyber_installer.@base.async_task;
using cyber_installer.definitions;
using cyber_installer.implement.modules.utils;
using cyber_installer.model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const string INSTALLATION_INFO_FILE_NAME = CyberInstallerDefinition.INSTALLATION_INFO_FILE_NAME;
        private const string INSTALLATION_INFO_FOLDER_NAME = CyberInstallerDefinition.INSTALLATION_INFO_FOLDER_NAME;

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
            _isEnableAutomaticallyReport = false;
        }

        protected override bool IsTaskPossible(object param)
        {
            return true;
        }

        protected override async Task DoAsyncMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            // Trường hợp latest version và tool đang ở trạng thái chưa được cài đăt
            if (_installingToolData.ToolStatus != ToolStatus.Installed
                && _installingToolData.ToolVersionSource.Count > 0
                && _installingToolData.ToolVersionSource.Last().VersionStatus != ToolVersionStatus.VersionInstalled)
            {
                // Tiến hành giải nén và cập nhật thông tin cài đặt
                var latestVersionTool = await ExtractZipAndUpdateInstallationData(this
                    , _installingToolData
                    , _installPath
                    , percentageOfTask: 70);
                await Task.Delay(300);
                var installationInfo = await ExportInstallationInfo(latestVersionTool.AssemblyName);
                CurrentProgress = 80;

                if (_installingToolData.ToolStatus == ToolStatus.Installed)
                {
                    await Task.Delay(300);
                    CreateUninstaller(installationInfo);
                    CurrentProgress = 90;

                    await Task.Delay(300);
                    ExtractIconToInstallaInfoFolder();
                }
                CurrentProgress = 100;
            }
            // Trường hợp latest version chưa được cài đặt  và tool đang ở trạng thái đã cài đăt
            else if (_installingToolData.ToolStatus == ToolStatus.Installed
                && _installingToolData.ToolVersionSource.Count > 0
                && _installingToolData.ToolVersionSource.Last().VersionStatus != ToolVersionStatus.VersionInstalled)
            {
                var installedSoftwareInfoFilePath = Utils.GetInstalledSoftwareInfoFilePath(_installingToolData.InstallPath);
                CurrentProgress = 0;

                if (File.Exists(installedSoftwareInfoFilePath))
                {
                    var isShouldUninstallSoftware = false;
                    var installedSoftwareInfoContent = await File.ReadAllTextAsync(installedSoftwareInfoFilePath);
                    var installedSoftwareInfo = JsonHelper.DeserializeObject<InstallationData>(installedSoftwareInfoContent ?? "");

                    // Kiểm tra điều kiện có nên gỡ cài đặt trước khi cập nhật không
                    if (installedSoftwareInfo != null && !string.IsNullOrEmpty(installedSoftwareInfo.AssemblyName))
                    {
                        await KillProcessIfExist(installedSoftwareInfo.AssemblyName, _installingToolData.ExecutePath);
                        CurrentProgress = 10;
                        await Task.Delay(CyberInstallerDefinition.AFTER_KILL_PROCESS_WAIT_TIME);
                        isShouldUninstallSoftware = false;
                    }

                    // Tiến hành gỡ cài đặt nếu thỏa mãn điều kiện
                    if (isShouldUninstallSoftware && installedSoftwareInfo != null)
                    {
                        // Xóa các file trong mục cài đặt
                        await Utils.DeleteAllFileInFolder(_installingToolData.InstallPath
                            , fileDeletingDelay: 300
                            , fileDeletedCallback: (deletedCount, total, deletedFile) =>
                            {
                                double progress = 1 / (double)total * 40;
                                CurrentProgress += progress;
                            });

                        // Xóa file desktop short cut
                        if (!string.IsNullOrEmpty(_installingToolData.ShortcutPath)
                            && File.Exists(_installingToolData.ShortcutPath))
                        {
                            File.Delete(_installingToolData.ShortcutPath);
                        }

                        // Tiến hành giải nén và cập nhật thông tin cài đặt
                        var latestVersionTool = await ExtractZipAndUpdateInstallationData(this
                            , _installingToolData
                            , _installPath
                            , percentageOfTask: 30);
                        await Task.Delay(300);

                        // Tiến hành ghi dữ liệu cài đặt lên thư mục chứa phần mềm 
                        var installationInfo = await ExportInstallationInfo(latestVersionTool.AssemblyName, installedSoftwareInfo);
                        CurrentProgress = 90;

                        // Cài icon hiển thị trên trình Installer
                        await Task.Delay(150);
                        ExtractIconToInstallaInfoFolder();
                        CurrentProgress = 95;

                        // Tạo lại desktop shortcut mà trước đấy đã xóa
                        if (!string.IsNullOrEmpty(_installingToolData.ShortcutPath))
                        {
                            _installingToolData.ShortcutPath = Utils.CreateDesktopShortCutToFile(_installingToolData.ExecutePath);
                        }
                        CurrentProgress = 100;
                    }
                    // Tiến hành cài đặt ghi đè lên các file cũ
                    else if (!isShouldUninstallSoftware && installedSoftwareInfo != null)
                    {
                        // Xóa file desktop short cut
                        if (!string.IsNullOrEmpty(_installingToolData.ShortcutPath)
                            && File.Exists(_installingToolData.ShortcutPath))
                        {
                            File.Delete(_installingToolData.ShortcutPath);
                        }
                        CurrentProgress = 20;


                        // Tiến hành giải nén và cập nhật thông tin cài đặt
                        var latestVersionTool = await ExtractZipAndUpdateInstallationData(this
                            , _installingToolData
                            , _installPath
                            , percentageOfTask: 60);
                        await Task.Delay(300);

                        // Tiến hành ghi dữ liệu cài đặt lên thư mục chứa phần mềm 
                        var installationInfo = await ExportInstallationInfo(latestVersionTool.AssemblyName, installedSoftwareInfo);
                        CurrentProgress = 90;

                        // Cài icon hiển thị trên trình Installer
                        await Task.Delay(150);
                        ExtractIconToInstallaInfoFolder();
                        CurrentProgress = 95;

                        // Tạo lại desktop shortcut mà trước đấy đã xóa
                        if (!string.IsNullOrEmpty(_installingToolData.ShortcutPath))
                        {
                            _installingToolData.ShortcutPath = Utils.CreateDesktopShortCutToFile(_installingToolData.ExecutePath);
                        }
                        CurrentProgress = 100;
                    }
                }

            }
        }

        private static async Task<ToolVersionData> ExtractZipAndUpdateInstallationData(
            InstallSoftwareTask taskInstance
            , ToolData installingToolData
            , string installPath
            , int percentageOfTask
            , Func<ZipArchiveEntry, bool>? shouldExtractEntry = null)
        {
            var latestVersionTool = installingToolData.ToolVersionSource.Last();
            var folderLocation = installPath;
            var zipFilePath = latestVersionTool.DownloadFilePath;

            try
            {
                await Utils.ExtractZipToFolder(zipFilePath
                , folderLocation
                , entryExtractedDelay: 100
                , shouldExtractEntry: shouldExtractEntry
                , entryExtractedCallback: (extractedCount, total, zipEntry) =>
                {
                    taskInstance.CurrentProgress = (double)extractedCount / (double)total * percentageOfTask;
                });

                File.Delete(zipFilePath);
                latestVersionTool.VersionStatus = ToolVersionStatus.VersionInstalled;
                installingToolData.CurrentInstalledVersion = latestVersionTool.Version;
                installingToolData.ExecutePath = installPath + "\\" + latestVersionTool.ExecutePath;
                installingToolData.InstallPath = installPath;
                installingToolData.ToolStatus = ToolStatus.Installed;
            }
            catch
            {
                latestVersionTool.VersionStatus = ToolVersionStatus.VersionInstalledFail;
                installingToolData.ToolStatus = ToolStatus.InstallFailed;
            }

            return latestVersionTool;
        }

        private void ExtractIconToInstallaInfoFolder()
        {
            var installationInfoFolderPath = _installPath + "\\" + INSTALLATION_INFO_FOLDER_NAME;
            var oldIconPath = _installingToolData.IconSource;
            var newFilePath = installationInfoFolderPath + "\\" + Path.GetFileName(oldIconPath);
            if (File.Exists(oldIconPath))
            {
                File.Move(oldIconPath, newFilePath, true);
            }
            _installingToolData.IconSource = newFilePath;
        }

        private async Task<InstallationData> ExportInstallationInfo(string assemblyName, InstallationData? oldInstallation = null)
        {
            var installationInfoFilePath = CreateCyberInfoFileForInstalledSoftware();

            if (oldInstallation == null)
            {
                var installationInfo = new InstallationData()
                {
                    ToolName = _installingToolData.Name,
                    Guid = Guid.NewGuid().ToString("B"),
                    AssemblyName = assemblyName,
                    ToolKey = _installingToolData.StringId,
                    CurrentInstalledVersion = _installingToolData.CurrentInstalledVersion,
                    ExecutePath = _installingToolData.ExecutePath,
                    InstallPath = _installingToolData.InstallPath,
                };
                var installationInfoJson = JsonHelper.SerializeObject(installationInfo);
                await File.WriteAllTextAsync(installationInfoFilePath, installationInfoJson);
                return installationInfo;
            }
            else
            {
                oldInstallation.ToolName = _installingToolData.Name;
                oldInstallation.AssemblyName = assemblyName;
                oldInstallation.ToolKey = _installingToolData.StringId;
                oldInstallation.CurrentInstalledVersion = _installingToolData.CurrentInstalledVersion;
                oldInstallation.ExecutePath = _installingToolData.ExecutePath;
                oldInstallation.InstallPath = _installingToolData.InstallPath;

                var installationInfoJson = JsonHelper.SerializeObject(oldInstallation);
                await File.WriteAllTextAsync(installationInfoFilePath, installationInfoJson);
                return oldInstallation;
            }

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

        private async Task KillProcessIfExist(string processName, string processRunPath)
        {
            var pLst = Process.GetProcessesByName(processName);
            foreach (var process in pLst)
            {
                if (!process.HasExited && processRunPath == process.MainModule?.FileName)
                {
                    process.Kill();
                    await process.WaitForExitAsync();
                }
            }
        }
    }
}
