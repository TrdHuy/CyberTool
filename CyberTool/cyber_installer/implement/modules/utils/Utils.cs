using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading;
using cyber_installer.definitions;
using System.Windows.Media.Imaging;

namespace cyber_installer.implement.modules.utils
{
    internal static class Utils
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

        public static async Task ExtractToFileAsync(this ZipArchiveEntry entry, string folderLocation)
        {
            if (Directory.Exists(folderLocation))
            {
                using (Stream stream = entry.Open())
                {
                    if (entry.FullName.EndsWith('/'))
                    {
                        Directory.CreateDirectory(folderLocation + "\\" + entry.FullName.Replace("/", "\\"));
                    }
                    else
                    {
                        using (FileStream fileStream = File.Create(folderLocation + "\\" + entry.Name))
                            await stream.CopyToAsync(fileStream);
                    }
                }
            }
        }

        public static async IAsyncEnumerable<T> WithEnforcedCancellation<T>(this IAsyncEnumerable<T> source
            , [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            cancellationToken.ThrowIfCancellationRequested();

            await foreach (var item in source)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
        }

        public static string GetInstalledSoftwareInfoFilePath(string intallFolderPath)
        {
            return intallFolderPath
                + "\\" + CyberInstallerDefinition.INSTALLATION_INFO_FOLDER_NAME
                + "\\" + CyberInstallerDefinition.INSTALLATION_INFO_FILE_NAME;
        }

        public static async Task<BitmapImage?> CreateBitmapImageFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var ms = new MemoryStream(await File.ReadAllBytesAsync(filePath)))
                {
                    var result = new BitmapImage();
                    result.BeginInit();
                    result.CacheOption = BitmapCacheOption.OnLoad;
                    result.StreamSource = ms;
                    result.EndInit();
                    return result;
                }
            }
            return null;
        }

        public static async Task ExtractZipToFolder(string zipFilePath
            , string folderLocation
            , int entryExtractedDelay = 0
            , Func<ZipArchive, int>? countTotalFileToExtract = null
            , Func<ZipArchiveEntry, bool>? shouldExtractEntry = null
            , EntryExtractedCallbackHandler? entryExtractedCallback = null)
        {
            var totalFiles = 0;
            var extractedFileCount = 0;
            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                totalFiles = countTotalFileToExtract?.Invoke(archive) ?? archive.Entries.Count;
                extractedFileCount = 0;
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (shouldExtractEntry?.Invoke(entry) ?? true)
                    {
                        await entry.ExtractToFileAsync(folderLocation);
                        extractedFileCount++;
                        entryExtractedCallback?.Invoke(extractedFileCount, totalFiles, entry);
                        await Task.Delay(entryExtractedDelay);
                    }
                }
            }
        }

        public static async Task DeleteAllFileInFolder(string folderLocation
            , int fileDeletingDelay = 0
            , Func<FileInfo, bool>? shouldDeleteFile = null
            , FileDeletedCallbackHandler? fileDeletedCallback = null)
        {
            DirectoryInfo di = new DirectoryInfo(folderLocation);
            int deletedFileCount = 0;
            int totalFile = 0;

            async Task DeleteFileAsync(FileInfo file)
            {
                await Task.Run(() =>
                {
                    if (File.Exists(file.FullName))
                    {
                        File.Delete(file.FullName);
                    }
                });
            }

            async Task<int> CountFilesInFolder(DirectoryInfo di)
            {
                int count = di.GetFiles().Length;
                await Task.Run(async () =>
                {
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        count += await CountFilesInFolder(dir);
                    }
                });
                return count;
            }

            async Task DeleteFileInFolder(DirectoryInfo di)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    if (shouldDeleteFile?.Invoke(file) ?? true)
                    {
                        await DeleteFileAsync(file);
                        deletedFileCount++;
                        fileDeletedCallback?.Invoke(deletedFileCount, totalFile, file);
                        await Task.Delay(fileDeletingDelay);
                    }
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    await DeleteFileInFolder(dir);
                    dir.Delete();
                }
            }

            totalFile = await CountFilesInFolder(di);
            await DeleteFileInFolder(di);
        }

        public static string CreateDesktopShortCutToFile(string sourceFilePath)
        {
            object shDesktop = (object)"Desktop";
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\" + Path.GetFileNameWithoutExtension(sourceFilePath) + ".lnk";
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Hotkey = "Ctrl+Shift+C";
            shortcut.TargetPath = sourceFilePath;
            shortcut.Save();
            return shortcutAddress;
        }

        public delegate void FileDeletedCallbackHandler(int deletedFileCount
            , int totalFile
            , FileInfo deletedFileInfo);

        public delegate void EntryExtractedCallbackHandler(int extractedFileCount
            , int totalFile
            , ZipArchiveEntry deletedFileInfo);
    }
}
