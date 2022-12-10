using cyber_installer.@base;
using cyber_installer.definitions;
using cyber_installer.implement.modules.server_contact_manager;
using cyber_installer.implement.modules.user_data_manager;
using cyber_installer.implement.modules.utils;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using System;
using System.Collections.Generic;
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
        private const int CHECK_UPDATE_INTERVAL_TIME = 50000;
        private const int CHECK_UPDATE_DELAY_AFTER_MAIN_WINDOW_SHOWED = 2000;
        private CancellationTokenSource? _requestDataTaskCancellationTokenSource;
        private bool _isUpdateable;
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

        private async void CheckCyberInstallerUpdateAvailable(object? sender = null, ElapsedEventArgs? e = null)
        {
            _requestDataTaskCancellationTokenSource = new CancellationTokenSource();
            await ServerContactManager.Current.RequestSoftwareInfoFromCyberServer(
                swKey: CyberInstallerDefinition.CYBER_INSTALLER_SW_ID
               , cancellationToken: _requestDataTaskCancellationTokenSource.Token
               , requestedCallback: (result) =>
               {
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
