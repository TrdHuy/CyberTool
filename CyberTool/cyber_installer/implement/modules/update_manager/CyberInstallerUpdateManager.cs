using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_installer.@base;
using cyber_installer.definitions;
using cyber_installer.implement.modules.server_contact_manager;
using cyber_installer.implement.modules.sw_installing_manager;
using cyber_installer.implement.modules.user_data_manager;
using cyber_installer.implement.modules.utils;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using cyber_installer.view_models.tabs.available_tab;
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
using System.Timers;

namespace cyber_installer.implement.modules.update_manager
{
    class CyberInstallerUpdateManager : BaseCyberInstallerModule
    {
        private readonly string CIBS_FOLDER_ZIP_PATH
            = CyberInstallerDefinition.CIBS_FOLDER_ZIP_PATH;
        private readonly string CIBS_FOLDER_BASE_PATH
            = CyberInstallerDefinition.CIBS_FOLDER_BASE_PATH;
        private readonly string CIBS_BASE_PATH
            = CyberInstallerDefinition.CIBS_BASE_PATH;

        private const int CHECK_UPDATE_INTERVAL_TIME
            = CyberInstallerDefinition.CHECK_UPDATE_CYBER_INSTALLER_INTERVAL_TIME;
        private const int CHECK_UPDATE_DELAY_AFTER_MAIN_WINDOW_SHOWED
            = CyberInstallerDefinition.CHECK_UPDATE_CYBER_INSTALLER_DELAY_AFTER_MAIN_WINDOW_SHOWED;
        private const int REQUEST_CIBS_UPDATE_CYBER_INSTALLER_DELAY_TIME
            = CyberInstallerDefinition.REQUEST_CIBS_UPDATE_CYBER_INSTALLER_DELAY_TIME;

        private CancellationTokenSource? _requestDataTaskCancellationTokenSource;
        private bool _isUpdateable;
        private ToolVO? _cyberInstallerVOCache;
        private System.Timers.Timer _checkUpdateTimer = new System.Timers.Timer(CHECK_UPDATE_INTERVAL_TIME);

        public delegate void IsUpdateableChangedHandler(object sender, bool isUpdateable);
        public event IsUpdateableChangedHandler? IsUpdateableChanged;

        public bool IsUpdateable
        {
            get
            {
                return _isUpdateable;
            }
            private set
            {
                var isShouldNotify = _isUpdateable != value;
                _isUpdateable = value;
                if (isShouldNotify)
                {
                    IsUpdateableChanged?.Invoke(this, _isUpdateable);
                }
            }
        }

        public static CyberInstallerUpdateManager Current
        {
            get => ModuleManager.CIUM_Instance;
        }

        public override void OnModuleCreate()
        {
            _checkUpdateTimer.Elapsed += CheckCyberInstallerUpdateAvailable;
            _checkUpdateTimer.AutoReset = false;
            _checkUpdateTimer.Enabled = true;
            App.Current.RegisterManageableTask(
                CyberInstallerDefinition.UPDATE_CYBER_INSTALLER_TASK_TYPE_KEY
                , CyberInstallerDefinition.UPDATE_CYBER_INSTALLER_TASK_NAME
                , maxCore: 1
                , initCore: 1);

        }

        public override async void OnMainWindowShowed()
        {
            await Task.Delay(CHECK_UPDATE_DELAY_AFTER_MAIN_WINDOW_SHOWED);
            CheckCyberInstallerUpdateAvailable();
        }

        public override void OnModuleDestroy()
        {
            _checkUpdateTimer.Close();
        }

        public async Task UpdateLatestCyberInstallerVersion()
        {
            if (IsUpdateable
                && _cyberInstallerVOCache != null
                && App.Current.IsTaskAvailable(CyberInstallerDefinition.UPDATE_CYBER_INSTALLER_TASK_TYPE_KEY))
            {

                ToolData? downloadedCIToolData = null;
                var cancellationTokenSource = new CancellationTokenSource();

                var downloadTask = new SelfReferenceCancelableAsyncTask(async (task, cts, atr) =>
                    {
                        downloadedCIToolData = await SwInstallingManager
                            .Current
                            .StartDownloadingLatestVersionToolTask(_cyberInstallerVOCache
                       , downloadProgressChangedCallback: (s, e) =>
                       {
                           task.SetCurrentProgress(e);
                       });
                        return atr;
                    }
                   , cancellationTokenSource: cancellationTokenSource
                   , name: "Downloading!"
                   , isEnableAutomaticallyReport: false
                   , reportDelay: 0);


                var installTask = new SelfReferenceCancelableAsyncTask(async (task, cts, atr) =>
                    {
                        if (downloadedCIToolData != null)
                        {
                            var latestVersionTool = downloadedCIToolData.ToolVersionSource[0];
                            var folderLocation = CIBS_FOLDER_BASE_PATH;
                            var zipFilePath = latestVersionTool.DownloadFilePath;

                            // Install new cibs version
                            await Utils.ExtractZipToFolder(zipFilePath
                               , folderLocation
                               , entryExtractedDelay: 300
                               , countTotalFileToExtract: (zA) =>
                               {
                                   var totalFile = 0;
                                   foreach (ZipArchiveEntry entry in zA.Entries)
                                   {
                                       if (Path.GetDirectoryName(entry.FullName) == CIBS_FOLDER_ZIP_PATH)
                                       {
                                           totalFile++;
                                       }
                                   }
                                   return totalFile;
                               }
                               , shouldExtractEntry: (entry) =>
                               {
                                   // Extract new cibs version
                                   return Path.GetDirectoryName(entry.FullName) == CIBS_FOLDER_ZIP_PATH;
                               }
                               , entryExtractedCallback: (eFC, tF, fI) =>
                               {
                                   task.SetCurrentProgress((double)eFC / (double)tF * 100);
                               });

                            await Task.Delay(REQUEST_CIBS_UPDATE_CYBER_INSTALLER_DELAY_TIME);
                        }

                        return atr;
                    }
                   , cancellationTokenSource: cancellationTokenSource
                   , name: "Installing cibs!"
                   , isEnableAutomaticallyReport: false
                   , reportDelay: 0);


                List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
                tasks.Add(downloadTask);
                tasks.Add(installTask);

                MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                    , new CancellationTokenSource()
                    , null
                    , name: "Update Cyber Installer"
                    , reportType: MultiAsyncTaskReportType.Manual);

                var message = await App.Current.ExecuteManageableMultipleTasks(
                    CyberInstallerDefinition.UPDATE_CYBER_INSTALLER_TASK_TYPE_KEY
                    , multiTask
                    , isBybassIfSemaphoreNotAvaild: true
                    , semaphoreTimeOut: 0
                    , isCancelable: false
                    , isUseMultiTaskReport: false);
                if (message == CyberContactMessage.Done)
                {
                    if (downloadedCIToolData != null)
                    {
                        var latestVersionTool = downloadedCIToolData.ToolVersionSource[0];
                        var rawFolderLocation = AppDomain.CurrentDomain.BaseDirectory;
                        // Must cut 2 last '\' charecters for passing through process argument
                        var folderLocation = rawFolderLocation.Substring(0, rawFolderLocation.Length - 1);
                        var zipFilePath = latestVersionTool.DownloadFilePath;

                        if (File.Exists(CIBS_BASE_PATH))
                        {
                            await Task.Delay(REQUEST_CIBS_UPDATE_CYBER_INSTALLER_DELAY_TIME);
                            Process p = new Process();
                            p.StartInfo.FileName = CIBS_BASE_PATH;
                            var callerID = CyberInstallerDefinition.CIBS_CALLER_ID;
                            var updateCmd = CyberInstallerDefinition.CIBS_UPDATE_CYBER_INSTALLER_CMD;
                            var currentCIProcessID = Process.GetCurrentProcess().Id;
                            var args = Utils.BuildProcessArgs(callerID
                                , updateCmd
                                , currentCIProcessID + ""
                                , zipFilePath
                                , folderLocation);
                            p.StartInfo.Arguments = args;
                            p.Start();
                        }
                    }


                }

            }
        }

        private async void CheckCyberInstallerUpdateAvailable(object? sender = null, ElapsedEventArgs? e = null)
        {
            _requestDataTaskCancellationTokenSource = new CancellationTokenSource();
            await ServerContactManager.Current.RequestSoftwareInfoFromCyberServer(
                swKey: CyberInstallerDefinition.CYBER_INSTALLER_SW_ID
               , cancellationToken: _requestDataTaskCancellationTokenSource.Token
               , requestedCallback: (result) =>
               {
                   _cyberInstallerVOCache = result;
                   if (result != null)
                   {
                       var currentPackageVersion = Utils.GetAppVersion();
                       if (currentPackageVersion < System.Version.Parse(result.ToolVersions.Last().Version))
                       {
                           IsUpdateable = true;
                       }
                   }
               });
        }

    }
}
