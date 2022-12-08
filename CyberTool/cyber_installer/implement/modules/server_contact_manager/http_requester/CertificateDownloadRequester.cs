using cyber_installer.@base.http_requester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.server_contact_manager.http_requester
{
    internal class CertificateDownloadRequester : BaseHttpRequester<X509Certificate2?>
    {
        private const string DOWNLOAD_CERTIFICATE_API_PATH = "/downloadcertificate";
        private const string REQUEST_DOWNLOAD_CERTIFICATE_HEADER_KEY = "h2sw-download-certificate";
        private const string REQUEST_DOWNLOAD_CYBER_ROOT_CA_HEADER_ID = "DOWNLOAD_CYBER_ROOT_CA";

        private const string RESPONSE_DOWNLOAD_CERTIFICATE_NAME_HEADER_ID = "CERTIFICATE_NAME";
        private const string RESPONSE_DOWNLOAD_CERTIFICATE_PASSWORD_HEADER_ID = "CERTIFICATE_PASSWORD";


        private const int TIME_OUT_FOR_REQUEST_OF_SEMAPHORE = 100;
        private SemaphoreSlim _requestDownloadCertificateSemaphore;

        public CertificateDownloadRequester()
        {
            _requestDownloadCertificateSemaphore = new SemaphoreSlim(1, 1);
        }

        private async Task<X509Certificate2?> RequestDownloadCertificate(HttpClient httpClient)
        {
            var isContinue = await _requestDownloadCertificateSemaphore.WaitAsync(TIME_OUT_FOR_REQUEST_OF_SEMAPHORE);
            try
            {
                if (!isContinue)
                {
                    return null;
                }

                httpClient.DefaultRequestHeaders.Add(REQUEST_DOWNLOAD_CERTIFICATE_HEADER_KEY, REQUEST_DOWNLOAD_CYBER_ROOT_CA_HEADER_ID);

                var response = await httpClient.GetAsync(GetRemoteAddress() + DOWNLOAD_CERTIFICATE_API_PATH);
                var responseContent = await response.Content.ReadAsByteArrayAsync();

                var certificateName = "";
                var certificatePassword = "";

                X509Certificate2? cert = null;

                try
                {
                    certificateName = response.Headers.GetValues(RESPONSE_DOWNLOAD_CERTIFICATE_NAME_HEADER_ID)
                   .FirstOrDefault();
                    certificatePassword = response.Headers.GetValues(RESPONSE_DOWNLOAD_CERTIFICATE_PASSWORD_HEADER_ID)
                        .FirstOrDefault();
                    cert = new X509Certificate2(responseContent, certificatePassword, X509KeyStorageFlags.DefaultKeySet);

                }
                catch { }

                return cert;
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
            finally
            {
                _requestDownloadCertificateSemaphore.Release();
            }
        }

        public override async Task<X509Certificate2?> Request(params object[] param)
        {
            try
            {
                var httpClient = param[0] as HttpClient ?? throw new ArgumentNullException();
                return await RequestDownloadCertificate(httpClient);
            }
            catch
            {
                return null;
            }
        }

    }
}
