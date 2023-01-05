using Microsoft.Build.Framework;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using Microsoft.Build.Utilities;

namespace cyber_build_task
{
    public class ExtractVersionPackageInfoTask : BaseCyberInstallerPackageBuilderTask
    {
        public ExtractVersionPackageInfoTask(TaskLoggingHelper tlogHepler) : base(tlogHepler)
        {
        }

        [Required]
        public string VersionBuildZipFilePath { get; set; } = "";
        [Required]
        public string PathToMainExe { get; set; } = "";
        [Required]
        public string Version { get; set; } = "";
        [Required]
        public string MainAssemblyName { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        [Required]
        public string InfoFilePath { get; set; } = "";
        [Required]
        public string BuildDirectoryPath { get; set; } = "";
        [Required]
        public string FinalBuildReleasePath { get; set; } = "";

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(VersionBuildZipFilePath)
                || string.IsNullOrEmpty(PathToMainExe)
                || string.IsNullOrEmpty(Version)
                || string.IsNullOrEmpty(MainAssemblyName)
                || string.IsNullOrEmpty(Description)
                || string.IsNullOrEmpty(FinalBuildReleasePath)
                || string.IsNullOrEmpty(BuildDirectoryPath)
                || string.IsNullOrEmpty(InfoFilePath))
            {
                return false;
            }

            using (var archive = ZipFile.OpenRead(VersionBuildZipFilePath))
            {
                var isExistMainExePath = false;
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName == PathToMainExe)
                    {
                        isExistMainExePath = true;
                        break;
                    }
                }
                if (!isExistMainExePath)
                {
                    throw new FileNotFoundException("Path to main exe was not found!");
                }
            }
            var versionInfo = new
            {
                Version = Version,
                MainAssemblyName = MainAssemblyName,
                PathToMainExe = PathToMainExe,
                Description = Description,
            };
            var info = JsonConvert.SerializeObject(versionInfo);
            if (!File.Exists(InfoFilePath))
            {
                File.Create(InfoFilePath).Dispose();
            }
            File.WriteAllText(InfoFilePath, info);

            var tempBuildFolderPath = BuildDirectoryPath + "\\temp_" + Version.ToString();
            if (Directory.Exists(tempBuildFolderPath))
            {
                Directory.Delete(tempBuildFolderPath, true);
            }
            Directory.CreateDirectory(tempBuildFolderPath);
            File.Move(VersionBuildZipFilePath, tempBuildFolderPath + "\\" + Path.GetFileName(VersionBuildZipFilePath));
            File.Move(InfoFilePath, tempBuildFolderPath + "\\" + Path.GetFileName(InfoFilePath));
            if (File.Exists(FinalBuildReleasePath))
                File.Delete(FinalBuildReleasePath);
            ZipFile.CreateFromDirectory(tempBuildFolderPath, FinalBuildReleasePath);
            Directory.Delete(tempBuildFolderPath, true);
            Process.Start(FinalBuildReleasePath);
            Log.LogMessageFromText("Build success at " + FinalBuildReleasePath, MessageImportance.High);
            return true;
        }
    }
}
