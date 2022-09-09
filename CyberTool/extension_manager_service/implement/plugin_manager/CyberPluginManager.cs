using cyber_base.app;
using cyber_base.extension;
using cyber_base.implement.utils;
using cyber_base.service;
using extension_manager_service.definitions;
using extension_manager_service.implement.log_manager;
using extension_manager_service.implement.module;
using extension_manager_service.implement.server_contact_manager;
using extension_manager_service.models.user_data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace extension_manager_service.implement.plugin_manager
{
    internal class CyberPluginManager : ICyberExtensionManager, ICyberAppModule
    {
        private SemaphoreSlim _instantiatedUserDataSemaphore = new SemaphoreSlim(1, 1);
        private const bool IS_USE_ROAMING_FOLDER = false;
        private const string TAG = "h2sw_solution";
        private const string USER_DATA_FOLDER_NAME = "data";
        private const string USER_DATA_FILE_NAME = "user_data.json";
        private string _userDataFolderPath = "";
        private string _userDataFilePath = "";
        private bool _isUserDateLoaded = false;

        private List<ICyberExtension> _pluginDllSource = new List<ICyberExtension>();
        private _MainUD? _rawUserData;

        public static CyberPluginManager Current
        {
            get => ModuleManager.CPM_Instance;
        }

        public void OnModuleStart()
        {
            LoadAllPluginDllFromUserData();
        }

        public async void OnModuleDestroy()
        {
            await ExportUserDataAsJson();
        }

        public async Task<bool> CheckPlugiIsInstalled(string pluginKey)
        {
            if (!_isUserDateLoaded)
            {
                await _instantiatedUserDataSemaphore.WaitAsync();
            }
            var pluginUD = _rawUserData?.PluginData?.Where(p => p.PluginKey == pluginKey)
                    .FirstOrDefault();
            if (pluginUD != null)
            {
                return !string.IsNullOrEmpty(pluginUD.CurrentInstalledVersionPath);
            }
            return false;
        }

        public string GetPluginInstallLocationPath(string pluginKey, string pluginVersion)
        {
            return (ExtensionManagerService
                    .Current
                    .ServiceManager?
                    .GetPluginsBaseFolderLocation() ?? "plugins")
                    + "\\" + pluginKey + "\\" + pluginVersion;
        }

        public async Task<bool> DownloadPluginFromCyberServer(string pluginKey, string pluginVersion)
        {
            var isDownloadable = true;
            var pluginUD = _rawUserData?.PluginData?.Where(p => p.PluginKey == pluginKey)
                   .FirstOrDefault();
            if (pluginUD != null)
            {
                isDownloadable = pluginUD.PluginVersionSource?.Where(v => Version.Parse(v.Version)
                    == Version.Parse(pluginVersion)).FirstOrDefault() == null;
                if (isDownloadable)
                {
                    EMSLogManager.V("Not found: pluginKey=" + pluginKey + ", pluginVersion" + pluginVersion + ", so can be downloadable");
                }
            }
            else
            {
                EMSLogManager.V("Not found: pluginKey=" + pluginKey + ", so can be downloadable");
            }

            if (isDownloadable)
            {
                await ServerContactManager.Current.DownloadPluginFromCyberServer(pluginKey
                    , pluginVersion
                    , requestedCallback: (pluginVersionUD) =>
                    {
                        if (pluginVersionUD != null)
                        {
                            if (pluginUD == null)
                            {
                                pluginUD = new PluginUD()
                                {
                                    PluginKey = pluginKey,
                                    PluginVersionSource = new List<PluginVersionUD>(),
                                };
                                _rawUserData?.PluginData?.Add(pluginUD);
                            }
                            pluginUD.PluginVersionSource?.Add(pluginVersionUD);
                        }
                        else
                        {
                            isDownloadable = false;
                        }
                    });
            }

            return isDownloadable;
        }

        public bool InstallDownloadedPlugin(string pluginKey, string pluginVersion)
        {
            bool isSuccess = false;
            try
            {
                var pluginUD = _rawUserData?.PluginData?.Where(p => p.PluginKey == pluginKey)
                   .FirstOrDefault();

                if (pluginUD != null)
                {
                    var pluginVersionUD = pluginUD.PluginVersionSource?
                        .Where(pvU => Version.Parse(pvU.Version) == Version.Parse(pluginVersion))
                        .FirstOrDefault();
                    if (pluginVersionUD != null && !pluginVersionUD.IsExtractedZip)
                    {
                        var folderLocation = (ExtensionManagerService
                           .Current
                           .ServiceManager?
                           .GetPluginsBaseFolderLocation() ?? "plugins")
                            + "\\" + pluginKey + "\\" + pluginVersion;
                        var zipFilePath = pluginVersionUD.DownloadFilePath;

                        ZipFile.ExtractToDirectory(
                            sourceArchiveFileName: zipFilePath
                            , destinationDirectoryName: folderLocation
                            , overwriteFiles: true);

                        File.Delete(zipFilePath);
                        pluginVersionUD.IsExtractedZip = true;
                        pluginUD.CurrentInstalledVersionPath = pluginVersionUD.ExecutePath;
                        pluginUD.CurrentInstalledVersionMainClassName = pluginVersionUD.MainClassName;
                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                EMSLogManager.E(ex.ToString());
            }
            return isSuccess;
        }

        public bool UninstallDownloadedPlugin(string pluginKey)
        {
            bool isSuccess = false;
            try
            {
                var pluginUD = _rawUserData?.PluginData?.Where(p => p.PluginKey == pluginKey)
                   .FirstOrDefault();

                if (pluginUD != null)
                {
                    Directory.Delete((ExtensionManagerService
                     .Current
                     .ServiceManager?
                     .GetPluginsBaseFolderLocation() ?? "plugins")
                     + "\\" + pluginKey, true);
                    _rawUserData?.PluginData?.Remove(pluginUD);
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                EMSLogManager.E(ex.ToString());
            }
            return isSuccess;
        }

        public async void LoadAllPluginDllFromUserData()
        {
            await _instantiatedUserDataSemaphore.WaitAsync();
            try
            {
                if (_rawUserData != null && _rawUserData.PluginData != null)
                {
                    foreach (var pluginData in _rawUserData.PluginData)
                    {
                        if (pluginData.CurrentInstalledVersionPath != "" && File.Exists(pluginData.CurrentInstalledVersionPath))
                        {
                            var fileInfo = new FileInfo(pluginData.CurrentInstalledVersionPath);
                            var plugin = Assembly.LoadFile(fileInfo.FullName);

                            var mainClassType = plugin.GetType(pluginData.CurrentInstalledVersionMainClassName);

                            if (mainClassType != null)
                            {
                                var instance = 
                                    mainClassType.GetProperty("Current")?.GetValue(null) as ICyberExtension 
                                    ?? Activator.CreateInstance(mainClassType) as ICyberExtension;

                                if (instance != null)
                                {
                                    _pluginDllSource.Add(instance);
                                }

                                if (instance is ICyberService)
                                {
                                    ExtensionManagerService
                                        .Current
                                        .ServiceManager?
                                        .RegisterExtensionAsCyberService(instance);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            finally
            {
                _instantiatedUserDataSemaphore.Release();
            }

        }

        private CyberPluginManager()
        {
            InitUserData();
        }

        private async void InitUserData()
        {
            await _instantiatedUserDataSemaphore.WaitAsync();
            try
            {
                if (IS_USE_ROAMING_FOLDER)
                {
                    InitUserDataRoamingFolderLocation();
                }
                else
                {
                    InitUserDataBaseFolderLocation();
                }
                await LoadUserData();
            }
            catch
            {

            }
            finally
            {
                _instantiatedUserDataSemaphore.Release();
            }
        }

        private void InitUserDataBaseFolderLocation()
        {
            var servicesFolderPath = ExtensionManagerService
                .Current
                .ServiceManager?.GetServicesBaseFolderLocation() ?? "services";
            _userDataFolderPath = servicesFolderPath + "\\"
                + ExtensionManagerDefinition.SERVICE_KEY + "\\"
                + USER_DATA_FOLDER_NAME;
            if (!Directory.Exists(_userDataFolderPath))
            {
                Directory.CreateDirectory(_userDataFolderPath);
            }
            _userDataFilePath = _userDataFolderPath + @"\" + USER_DATA_FILE_NAME;
            if (!File.Exists(_userDataFilePath))
            {
                File.Create(_userDataFilePath).Dispose();
            }
        }

        private void InitUserDataRoamingFolderLocation()
        {
            try
            {
                var attribs = Assembly.GetCallingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                var assemblyDataPath = "";
                if (attribs.Length > 0)
                {
                    assemblyDataPath = ((AssemblyCompanyAttribute)attribs[0]).Company
                        + @"\" + Assembly.GetCallingAssembly().GetName().Name;
                }
                else
                {
                    assemblyDataPath = TAG + @"\" + Assembly.GetCallingAssembly().GetName().Name;
                }

                _userDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _userDataFolderPath = _userDataFolderPath + @"\" + assemblyDataPath;
                _userDataFolderPath = _userDataFolderPath + @"\" + USER_DATA_FOLDER_NAME;
                if (!Directory.Exists(_userDataFolderPath))
                {
                    Directory.CreateDirectory(_userDataFolderPath);
                }
                _userDataFilePath = _userDataFolderPath + @"\" + USER_DATA_FILE_NAME;
                if (!File.Exists(_userDataFilePath))
                {
                    File.Create(_userDataFilePath).Dispose();
                }
            }
            catch
            {
                try
                {
                    _userDataFolderPath = Path.GetDirectoryName(Assembly
                        .GetEntryAssembly()?
                        .Location ?? "") ?? "";
                    _userDataFolderPath = _userDataFolderPath + @"\" + USER_DATA_FOLDER_NAME;

                    if (!Directory.Exists(_userDataFolderPath))
                    {
                        Directory.CreateDirectory(_userDataFolderPath);
                    }
                    _userDataFilePath = _userDataFolderPath + @"\" + USER_DATA_FILE_NAME;
                    if (!File.Exists(_userDataFilePath))
                    {
                        File.Create(_userDataFilePath).Dispose();
                    }
                }
                catch
                {

                }
            }
        }

        private async Task ExportUserDataAsJson()
        {
            if (_rawUserData != null)
            {
                var path = _userDataFilePath;
                var json = JsonHelper.SerializeObject(_rawUserData);
                await File.WriteAllTextAsync(path, json);
            }
        }

        private async Task LoadUserData()
        {
            var path = _userDataFilePath;
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            string json = await File.ReadAllTextAsync(path, Encoding.UTF8);
            _rawUserData = JsonHelper.DeserializeObject<_MainUD>(json) ?? new _MainUD();
            if (_rawUserData.PluginData == null)
            {
                _rawUserData.PluginData = new List<PluginUD>();
            }
            _isUserDateLoaded = true;
        }

    }
}
