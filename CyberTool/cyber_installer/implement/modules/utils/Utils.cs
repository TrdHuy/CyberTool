using System;
using cyber_installer.@base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace cyber_installer.implement.modules.utils
{
    internal class Utils
    {
        public static bool CheckCertificateExist(string cnName
            , StoreName storeName
            , StoreLocation storeLocation)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            var existedCertificates = store.Certificates.Find(
                X509FindType.FindBySubjectName,
                cnName,
                false);
            store.Close();

            if (existedCertificates != null && existedCertificates.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static X509Certificate2? GetCertificateFromStore(string cnName
            , string expirationTimeString
            , StoreName storeName
            , StoreLocation storeLocation)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            var existedCertificates = store.Certificates.Find(
                X509FindType.FindBySubjectName,
                cnName,
                false);
            store.Close();

            if (existedCertificates != null && existedCertificates.Count > 0)
            {
                foreach (var cert in existedCertificates)
                {
                    if (cert.GetExpirationDateString() == expirationTimeString)
                    {
                        return cert;
                    }
                }
            }
            return null;
        }

        public static X509Certificate2? GetCertificateFromStoreByThumbprint(string thumbprint
            , StoreName storeName
            , StoreLocation storeLocation)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            var existedCertificates = store.Certificates.Find(
                X509FindType.FindByThumbprint,
                thumbprint,
                false);
            store.Close();

            if (existedCertificates != null && existedCertificates.Count > 0)
            {
                return existedCertificates[0];
            }
            return null;
        }

        public static Dictionary<string, string> GetCertificateProperties(X509Certificate2 cert)
        {
            var certProps = cert.Issuer.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.Split('='))
                .ToDictionary(x => x[0], x => x[1]);
            return certProps;
        }
    }
}
