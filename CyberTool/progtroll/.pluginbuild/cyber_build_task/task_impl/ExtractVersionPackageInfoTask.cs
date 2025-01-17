﻿using Microsoft.Build.Framework;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using Microsoft.Build.Utilities;
using System.Net;
using Newtonsoft.Json.Linq;
using System;

namespace cyber_build_task
{
    public class ExtractVersionPackageInfoTask : BaseCyberInstallerPackageBuilderTask
    {
        private const string RemoteAdress = "http://107.98.32.108:8080";
        private const string RequestCyberSwPackageBuildParamPath = "/cyberswpackbuildparam";
        private const string RequestInfoHeaderKey = "h2sw-request-info";
        private const string RequestCyberSwPackageBuildParamHeaderId = "GET_CYBER_SW_PACKAGE_BUILD_PARAM";

        public ExtractVersionPackageInfoTask(TaskLoggingHelper tlogHepler) : base(tlogHepler)
        {
        }

        [Required]
        public string VersionBuildZipFilePath { get; set; } = "";
        [Required]
        public string PathToMainDll { get; set; } = "";
        [Required]
        public string MainClassName { get; set; } = "";
        [Required]
        public string Version { get; set; } = "";
        [Required]
        public string MainAssemblyName { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        [Required]
        public string CompressedBuildFileName { get; set; } = "";
        [Required]
        public string BuildDirectoryPath { get; set; } = "";
        [Required]
        public string FinalBuildReleasePath { get; set; } = "";

        public override bool Execute()
        {
            Log.LogMessageFromText($"huy.td1: Start ExtractVersionPackageInfoTask", MessageImportance.High);
            Log.LogMessageFromText($"huy.td1: VersionBuildZipFilePath={VersionBuildZipFilePath}", MessageImportance.High);
            Log.LogMessageFromText($"huy.td1: PathToMainDll={PathToMainDll}", MessageImportance.High);
            Log.LogMessageFromText($"huy.td1: Version={Version}", MessageImportance.High);
            Log.LogMessageFromText($"huy.td1: MainAssemblyName={MainAssemblyName}", MessageImportance.High);
            Log.LogMessageFromText($"huy.td1: MainClassName={MainClassName}", MessageImportance.High);
            Log.LogMessageFromText($"huy.td1: Description={Description}", MessageImportance.High);
            Log.LogMessageFromText($"huy.td1: FinalBuildReleasePath={FinalBuildReleasePath}", MessageImportance.High);
            Log.LogMessageFromText($"huy.td1: BuildDirectoryPath={BuildDirectoryPath}", MessageImportance.High);

            if (string.IsNullOrEmpty(VersionBuildZipFilePath)
                || string.IsNullOrEmpty(PathToMainDll)
                || string.IsNullOrEmpty(Version)
                || string.IsNullOrEmpty(MainAssemblyName)
                || string.IsNullOrEmpty(MainClassName)
                || string.IsNullOrEmpty(Description)
                || string.IsNullOrEmpty(FinalBuildReleasePath)
                || string.IsNullOrEmpty(BuildDirectoryPath))
            {
                Log.LogMessageFromText($"Missing some required property!", MessageImportance.High);
                return false;
            }

            WebRequest request = HttpWebRequest.Create(RemoteAdress + RequestCyberSwPackageBuildParamPath);
            request.Headers.Add(RequestInfoHeaderKey, RequestCyberSwPackageBuildParamHeaderId);
            var buildInfoFileName = "";
            var packageBuildFileName = "";
            try
            {
                Log.LogMessageFromText($"GET {RemoteAdress + RequestCyberSwPackageBuildParamPath}", MessageImportance.High);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            string responseText = reader.ReadToEnd();
                            dynamic packBuildInfo = JObject.Parse(responseText);
                            buildInfoFileName = packBuildInfo.InfoFileName;
                            packageBuildFileName = packBuildInfo.MainBuildFileName;
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                // 404, or cannot connect to server address
                Log.LogError("Fail to get cyber package build param (please contact huy.td1):" + ex.Message);
                return false;
            }

            if (string.IsNullOrEmpty(buildInfoFileName) 
                || string.IsNullOrEmpty(packageBuildFileName))
            {
                Log.LogError("Fail to get cyber package build param (please contact huy.td1)");
                return false;
            }

            using (var archive = ZipFile.OpenRead(VersionBuildZipFilePath))
            {
                var isExistMainDllPath = false;
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName == PathToMainDll)
                    {
                        isExistMainDllPath = true;
                        break;
                    }
                }
                if (!isExistMainDllPath)
                {
                    throw new FileNotFoundException("Path to main dll was not found!");
                }
            }
            var versionInfo = new
            {
                Version = Version,
                MainAssemblyName = MainAssemblyName,
                PathToMainDll = PathToMainDll,
                MainClassName = MainClassName,
                Description = Description,
                CompressedBuildFileName = CompressedBuildFileName,
            };
            var info = JsonConvert.SerializeObject(versionInfo);
            var tempBuildFolderPath = BuildDirectoryPath + "\\temp_" + Version.ToString();
            var infoFilePath = tempBuildFolderPath + "\\" + buildInfoFileName;
            if (Directory.Exists(tempBuildFolderPath))
            {
                Directory.Delete(tempBuildFolderPath, true);
            }

            Directory.CreateDirectory(tempBuildFolderPath);

            if (!File.Exists(infoFilePath))
            {
                File.Create(infoFilePath).Dispose();
            }

            File.WriteAllText(infoFilePath, info);
            File.Move(VersionBuildZipFilePath
                , tempBuildFolderPath + "\\" + Path.GetFileName(packageBuildFileName));

            if (File.Exists(FinalBuildReleasePath))
                File.Delete(FinalBuildReleasePath);

            // zip file build 
            ZipFile.CreateFromDirectory(tempBuildFolderPath, FinalBuildReleasePath);

            // Xóa file build sau khi zip 
            Directory.Delete(tempBuildFolderPath, true);

            // Mở thư mục chứa final file build (file zip) 
            Process.Start(FinalBuildReleasePath);
            Log.LogMessageFromText("Build success at " + FinalBuildReleasePath, MessageImportance.High);
            return true;
        }
    }
}
