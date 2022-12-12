﻿using System;
using cyber_installer.@base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.IO;
using System.IO.Compression;

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

        public static Version GetAppVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName()?.Version ?? new Version(0, 0, 0, 0);
            return version;
            //return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        public static string BuildProcessArgs(params string[] args)
        {
            string result = "";
            int i = 0;
            foreach (string arg in args)
            {
                result += "\"" + arg + "\"";
                if (i < args.Length)
                {
                    result += " ";
                }
                i++;
            }
            return result;
        }
        public static void CreateIsNotExistFile(string filePath
            , FileAttributes parentFolderAttr = FileAttributes.Directory
            , FileAttributes fileAttr = FileAttributes.Normal)
        {
            if (!File.Exists(filePath))
            {
                var folderPath = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(folderPath)
                    && !Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    DirectoryInfo di = new DirectoryInfo(folderPath);
                    di.Attributes = parentFolderAttr;
                }

                File.Create(filePath).Dispose();
                FileInfo fi = new FileInfo(filePath);
                fi.Attributes = fileAttr;
            }
        }

        public static async Task ExtractZipArchiveEntry(ZipArchiveEntry entry, string folderLocation)
        {
            if (Directory.Exists(folderLocation))
            {
                using (Stream stream = entry.Open())
                {
                    using (FileStream fileStream = File.Create(folderLocation + "\\" + entry.Name))
                        await stream.CopyToAsync(fileStream);
                }
            }
        }
    }
}
