using cyber_base.implement.utils;
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
using System.Threading.Tasks;

namespace extension_manager_service.implement.plugin_manager
{
    internal class CyberPluginManager : BaseExtensionManagerModule
    {
        private const bool IS_USE_ROAMING_FOLDER = false;
        private const string TAG = "h2sw_solution";
        private const string USER_DATA_FOLDER_NAME = "data";
        private const string USER_DATA_FILE_NAME = "user_data.json";
        private string _userDataFolderPath = "";
        private string _userDataFilePath = "";

        private _MainUD? _rawUserData;

        public static CyberPluginManager Current
        {
            get => ModuleManager.CPM_Instance;
        }

        public override async void OnDestroy()
        {
            await ExportUserDataAsJson();
            base.OnDestroy();
        }

        public bool CheckPlugiIsInstalled(string pluginKey)
        {
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

        private CyberPluginManager()
        {
            if (IS_USE_ROAMING_FOLDER)
            {
                InitUserDataRoamingFolderLocation();
            }
            else
            {
                InitUserDataBaseFolderLocation();
            }
            InitUserData();
        }

        private async void InitUserData()
        {
            await LoadUserData();
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
    }
}
