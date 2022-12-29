using cyber_base.implement.utils;
using cyber_installer.@base.async_task;
using cyber_installer.definitions;
using cyber_installer.model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cyber_base.async_task;
using cyber_installer.implement.modules.utils;
using System.Diagnostics;

namespace cyber_installer.implement.modules.ui_event_handler.async_task
{
    internal class UninstallSoftwareTask : AbsParamAsyncTask
    {

        private ToolData _installingToolData;

        public UninstallSoftwareTask(ToolData toolData
            , Action<AsyncTaskResult>? callback = null
            , string name = "Uninstalling") : base(param: toolData
                , name: name
                , completedCallback: callback
                , estimatedTime: 0
                , delayTime: 0
                , reportDelay: 100)
        {
            _installingToolData = toolData;
            _isEnableAutomaticallyReport = false;
            _isDelayBeforeExecutingTask = true;
        }

        protected override bool IsTaskPossible(object param)
        {
            return true;
        }

        protected override async Task DoAsyncMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            if (_installingToolData.ToolStatus == ToolStatus.Installed)
            {
                var installedSoftwareInfoFilePath = Utils.GetInstalledSoftwareInfoFilePath(_installingToolData.InstallPath);
                if (File.Exists(installedSoftwareInfoFilePath))
                {
                    var isShouldUninstallSoftware = false;
                    var installedSoftwareInfoContent = await File.ReadAllTextAsync(installedSoftwareInfoFilePath);
                    var installedSoftwareInfo = JsonHelper.DeserializeObject<InstallationData>(installedSoftwareInfoContent ?? "");

                    if (installedSoftwareInfo != null && !string.IsNullOrEmpty(installedSoftwareInfo.AssemblyName))
                    {
                        await KillProcessIfExist(installedSoftwareInfo.AssemblyName, _installingToolData.ExecutePath);
                        CurrentProgress = 10;
                        await Task.Delay(CyberInstallerDefinition.AFTER_KILL_PROCESS_WAIT_TIME);

                        isShouldUninstallSoftware = true;
                    }

                    if (isShouldUninstallSoftware && installedSoftwareInfo != null)
                    {
                        RemoveUninstaller(installedSoftwareInfo);
                        CurrentProgress = 20;
                        await Task.Delay(2000);

                        await Utils.DeleteAllFileInFolder(_installingToolData.InstallPath
                            , fileDeletingDelay: 300
                            , fileDeletedCallback: (deletedCount, total, deletedFile) =>
                            {
                                double progress = 1 / (double)total * 80;
                                CurrentProgress += progress;
                            });

                        if (!string.IsNullOrEmpty(_installingToolData.ShortcutPath)
                            && File.Exists(_installingToolData.ShortcutPath))
                        {
                            File.Delete(_installingToolData.ShortcutPath);
                        }
                    }

                    CurrentProgress = 100;
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

        private void RemoveUninstaller(InstallationData installationInfo)
        {
            using (RegistryKey? parent = Registry.LocalMachine.OpenSubKey(
                         @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {

                if (parent == null)
                {
                    throw new Exception("Uninstall registry key not found.");
                }
                try
                {
                    RegistryKey? key = null;
                    try
                    {
                        parent.DeleteSubKey(installationInfo.Guid);
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
                        "An error occurred deleting subkey of uninstallation registry key!",
                        ex);
                }
            }
        }
    }
}