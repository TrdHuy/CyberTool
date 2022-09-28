using cyber_installer.@base;
using cyber_installer.implement.modules.server_contact_manager.contacts;
using cyber_installer.implement.modules.user_config_manager;
using cyber_installer.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.server_contact_manager
{
    internal class ServerContactManager : ICyberInstallerModule
    {
        private const string REQUEST_INFO_API_PATH = "/requestinfo";
        private const string REQUEST_INFO_HEADER_KEY = "h2sw-request-info";
        private RequestSoftwareDataContact _requestSoftwareDataContact;
        private string _remoteAdress = "";

        public static ServerContactManager Current
        {
            get => ModuleManager.SCM_Instance;
        }

        private ServerContactManager()
        {
            _requestSoftwareDataContact = new RequestSoftwareDataContact();
        }

        public void OnModuleCreate()
        {
            _requestSoftwareDataContact.Refresh();
        }

        public void OnModuleDestroy()
        {
        }

        public void OnModuleStart()
        {
            _remoteAdress = UserConfigManager.Current.CurrentConfig.RemoteAdress;
        }

        public async Task RequestSoftwareInfoFromCyberServer(Action<ICollection<ToolVO>?> requestedCallback
            , CancellationToken cancellationToken
            , bool isForce = false)
        {
            if (isForce)
            {
                _requestSoftwareDataContact.Refresh();
            }

            if (!string.IsNullOrEmpty(_remoteAdress) && !_requestSoftwareDataContact.IsFullOfDbSet)
            {
                using (HttpClient client = new HttpClient())
                {
                    IEnumerable? listToolSource = null;
                    try
                    {
                        listToolSource = await _requestSoftwareDataContact.RequestServerData(client
                       , REQUEST_INFO_HEADER_KEY
                       , _remoteAdress + REQUEST_INFO_API_PATH
                       , cancellationToken);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            throw new TaskCanceledException("Request software info task was canceled!");
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        if (ex.Message.Contains("No connection could be made because the target machine actively refused it"))
                        {
                            // TODO: Show message box here
                        }
                    }
                    catch { }
                    finally
                    {
                        requestedCallback?.Invoke(listToolSource as ICollection<ToolVO>);
                    }
                }
            }
            else
            {
                requestedCallback?.Invoke(null);
            }

        }
    }
}
