using cyber_base.async_task;
using cyber_base.implement.utils;
using cyber_installer.@base.async_task;
using cyber_installer.definitions;
using cyber_installer.implement.modules.utils;
using cyber_installer.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.ui_event_handler.async_task
{
    internal class InstallRootCATask : AbsParamAsyncTask
    {
        private Logger _logger = new Logger("InstallRootCATask", "cyber_installer");

        private string? _certLocation;
        private string? _password;
        private X509Certificate2? _certToInstall;

        private StoreName _storeName = StoreName.Root;
        private StoreLocation _storeLocation = StoreLocation.LocalMachine;

        public InstallRootCATask(string certLocation
            , string password
            , StoreName storeName
            , StoreLocation storeLocation
            , Action<AsyncTaskResult>? callback = null
            , string name = "Installing") : base(param: certLocation
                , name: name
                , completedCallback: callback
                , estimatedTime: 100
                , reportDelay: 100)
        {
            this._certLocation = certLocation;
            this._password = password;
            this._storeName = storeName;
            this._storeLocation = storeLocation;
            this._isEnableAutomaticallyReport = false;
        }

        public InstallRootCATask(X509Certificate2 cert
            , StoreName storeName
            , StoreLocation storeLocation
            , Action<AsyncTaskResult>? callback = null
            , string name = "Installing") : base(param: cert
                , name: name
                , completedCallback: callback
                , estimatedTime: 100
                , reportDelay: 100)
        {
            this._certToInstall = cert;
            this._storeName = storeName;
            this._storeLocation = storeLocation;
            this._isEnableAutomaticallyReport = false;
        }

        protected override bool IsTaskPossible(object param)
        {
            return true;
        }

        protected override async Task DoAsyncMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            await Task.Delay(10);
            X509Store store = new X509Store(_storeName, _storeLocation);
            store.Open(OpenFlags.ReadWrite);
            if (_certToInstall == null)
            {
                if (File.Exists(_certLocation))
                {
                    X509Certificate2 cert = new X509Certificate2(_certLocation, _password, X509KeyStorageFlags.DefaultKeySet);
                    var certProps = Utils.GetCertificateProperties(cert);
                    var certCNName = "";
                    try
                    {
                        certCNName = certProps["CN"];
                        var existedCertificates = store.Certificates.Find(
                            X509FindType.FindBySubjectName,
                            certCNName,
                            false);

                        if (existedCertificates != null && existedCertificates.Count > 0)
                        {
                            _logger.I("Certificate " + certCNName + " already exists");
                        }
                    }
                    catch { };


                    if (cert != null)
                    {
                        store.Add(cert);
                        _logger.I("Import " + certCNName + " successfully!");
                    }
                    else
                    {
                        _logger.I("Fail to import " + certCNName + "!");
                    }
                }

            }
            else
            {
                store.Add(_certToInstall);
                _logger.I("Import " + _certToInstall.Issuer + " successfully!");

            }
            store.Close();

        }

    }

}
