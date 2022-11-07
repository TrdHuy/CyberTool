using cyber_installer.@base;
using cyber_installer.implement.modules.server_contact_manager.http_requester;
using cyber_installer.implement.modules.user_config_manager;
using cyber_installer.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.server_contact_manager
{
    internal class ServerContactManager : ICyberInstallerModule
    {
        private SoftwareDataRequester? _softwareDataRequester;
        private CertificateDownloadRequester? _certificateDownloaRequester;
        private X509Certificate2? _certCache;
        public static ServerContactManager Current
        {
            get => ModuleManager.SCM_Instance;
        }

        private ServerContactManager()
        {
        }

        public void OnModuleCreate()
        {
            _softwareDataRequester?.Refresh();
        }

        public void OnModuleDestroy()
        {
        }

        public void OnModuleStart()
        {
            _certificateDownloaRequester = new CertificateDownloadRequester();
            _softwareDataRequester = new SoftwareDataRequester();
        }

        public async Task RequestSoftwareInfoFromCyberServer(Action<ICollection<ToolVO>?> requestedCallback
            , CancellationToken cancellationToken
            , bool isForce = false)
        {
            if (isForce)
            {
                _softwareDataRequester?.Refresh();
            }

            if (_softwareDataRequester != null && !_softwareDataRequester.IsFullOfDbSet)
            {
                using (HttpClient client = new HttpClient())
                {
                    IEnumerable? listToolSource = null;
                    try
                    {
                        listToolSource = await _softwareDataRequester.Request(client
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

        public async Task RequestDownloadCyberCertificate(Func<X509Certificate2?, Task> requestedCallback, bool force = false)
        {
            if (_certCache == null || force)
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        if (_certificateDownloaRequester != null)
                        {
                            _certCache = await _certificateDownloaRequester.Request(client);
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
                        await requestedCallback.Invoke(_certCache);
                    }
                }
            }
            else
            {
                await requestedCallback.Invoke(_certCache);
            }

        }
    }
}
