using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;

namespace cyber_build_task
{
    public class DeleteAllFileInFolderTask : BaseCyberInstallerPackageBuilderTask
    {
        [Required]
        public string FolderPath { get; set; } = "";

        public DeleteAllFileInFolderTask(TaskLoggingHelper tlogHepler) : base(tlogHepler)
        {

        }

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(FolderPath))
            {
                return false;
            }
            if (Directory.Exists(FolderPath))
            {
                var dI = new DirectoryInfo(FolderPath);
                DeleteAllFileAndFolder(dI);
            }
            else
            {
                Log.LogError($"Fail to execute DeleteAllFileInFolderTask: {FolderPath} not exist!");
            }
            return true;
        }

        private void DeleteAllFileAndFolder(DirectoryInfo dI)
        {
            foreach (DirectoryInfo dir in dI.GetDirectories())
            {
                DeleteAllFileAndFolder(dir);
                dir.Delete(true);
            }

            foreach (FileInfo file in dI.GetFiles())
            {
                try
                {
                    file.Delete();
                    Log.LogMessageFromText($"Deleted {file.FullName}", MessageImportance.Normal);
                }
                catch (Exception ex)
                {
                    Log.LogError($"DeleteAllFileInFolderTask: Fail to delete {file.FullName}: {ex.Message}");
                }
            }
        }
    }
}
