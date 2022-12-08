using cyber_installer.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using cyber_installer.implement.modules.user_config_manager;
using cyber_installer.implement.modules.server_contact_manager.security;

namespace cyber_installer.@base.http_requester
{
    internal abstract class BaseHttpRequester<T>
    {
        private const string TAG = "h2sw_solution";
        private const string USER_DATA_FOLDER_NAME = "data";
        private const string DOWNLOADED_SW_FOLDER_NAME = "tools";
        private static string _downloadedSwFolder = "";

        static BaseHttpRequester()
        {
            var assemblyDataPath = "";
            var attribs = Assembly.GetCallingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
            if (attribs.Length > 0)
            {
                assemblyDataPath = ((AssemblyCompanyAttribute)attribs[0]).Company
                    + @"\" + Assembly.GetCallingAssembly().GetName().Name;
            }
            else
            {
                assemblyDataPath = TAG + @"\" + Assembly.GetCallingAssembly().GetName().Name;
            }
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var userDataFolder = appDataFolder + @"\" + assemblyDataPath + @"\" + USER_DATA_FOLDER_NAME;
            _downloadedSwFolder = userDataFolder + @"\" + DOWNLOADED_SW_FOLDER_NAME;
        }

        protected string GetRemoteAddress()
        {
            if (CertificateManager.Current.IsCyberCertificateInstalled)
            {
                return UserConfigManager.Current.CurrentConfig.SSLRemoteAdress;
            }
            else
            {
                return UserConfigManager.Current.CurrentConfig.RemoteAdress;
            }
        }

        protected string GetToolDownloadFileFolder(string toolKey, string toolVersion)
        {
            return _downloadedSwFolder + "\\" + toolKey + "\\" + toolVersion;
        }

        public abstract Task<T> Request(params object[] param);
    }
}
