using cyber_base.async_task;
using cyber_installer.@base;
using cyber_installer.implement.modules.ui_event_handler.async_task;
using cyber_installer.implement.modules.user_data_manager;
using cyber_installer.implement.modules.utils;
using cyber_installer.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.server_contact_manager.security
{
    internal class CertificateManager : ICyberInstallerModule
    {
        public bool IsCyberCertificateInstalled { get; private set; } = false;

        public static CertificateManager Current
        {
            get => ModuleManager.CM_Instance;
        }

        private CertificateManager()
        {
        }

        public void OnModuleCreate()
        {
        }

        public void OnModuleDestroy()
        {
        }

        public void OnModuleStart()
        {
            InstallNewwestCyberCertificate();
        }

        private async void InstallNewwestCyberCertificate()
        {
            await ServerContactManager.Current.RequestDownloadCyberCertificate(
                requestedCallback: async (certificate) =>
                {
                    var certData = UserDataManager.Current.CurrentUserData.CertificateData;
                    X509Certificate2? certFromStore = null;
                    if (!certData.IsEmpty())
                    {
                        certFromStore = Utils.GetCertificateFromStoreByThumbprint(certData.TRCA_Thumbprint
                            , StoreName.Root
                            , StoreLocation.LocalMachine);
                        if (certificate != null
                            && (certFromStore == null
                                || certFromStore != null
                                    && certFromStore.Thumbprint != certificate.Thumbprint))
                        {
                            IsCyberCertificateInstalled = await InstallNewCertifacate(certificate, certData);
                        }
                        else
                        {
                            IsCyberCertificateInstalled = true;
                        }

                    }
                    else if (certificate != null)
                    {
                        IsCyberCertificateInstalled = await InstallNewCertifacate(certificate, certData);
                    }
                }
                , force: true);
        }

        private async Task<bool> InstallNewCertifacate(X509Certificate2 certificate, CertificateData userCertData)
        {
            var certProp = Utils.GetCertificateProperties(certificate);
            var installCertificateTask = new InstallRootCATask(certificate
                , StoreName.Root
                , StoreLocation.LocalMachine);

            await installCertificateTask.Execute();
            if (installCertificateTask.Result.MesResult != MessageAsyncTaskResult.Faulted
            || installCertificateTask.Result.MesResult != MessageAsyncTaskResult.Aborted)
            {
                userCertData.TRCA_CNName = certProp["CN"];
                userCertData.TRCA_Expriation = certificate.GetExpirationDateString();
                userCertData.TRCA_Thumbprint = certificate.Thumbprint;
                await UserDataManager.Current.ExportUserDataToFile();
                return true;
            }
            return false;
        }
    }
}
