using System.Diagnostics;
using System.IO.Compression;

namespace cyber_installer_background_service
{
    internal class Program
    {
        private const string CYBER_INSTALLER_REQUESTER_ID = "CyberInstallerWindow{0367E847-B5C3-4CDD-9C34-B78A769AF73C}";
        private const string UPDATE_CYBER_INSTALLER_CMD = "UpdateCyberInstaller";
        private const string CIBS_FOLDER_ZIP_PATH = "cibs";
        private const string CYBER_INSTALLER_EXE_ZIP_PATH = "Cyber Installer.exe";

        static void Main(string[] args)
        {
            var requesterID = "";
            var requesterProcessId = -1;
            var cmdID = "";
            try
            {
                if (args.Length >= 3)
                {
                    requesterID = args[0];
                    cmdID = args[1];
                    requesterProcessId = Convert.ToInt32(args[2]);
                    switch (requesterID)
                    {
                        case CYBER_INSTALLER_REQUESTER_ID:
                            {
                                if (cmdID == UPDATE_CYBER_INSTALLER_CMD && args.Length == 5)
                                {
                                    var zipFilePath = args[3].ToString();
                                    var installFolderLocation = args[4].ToString();
                                    if (File.Exists(zipFilePath) && Directory.Exists(installFolderLocation))
                                    {

                                        var requesterProcess = Process.GetProcessById(requesterProcessId);
                                        if (!requesterProcess.HasExited && requesterProcess?.ProcessName == "Cyber Installer")
                                        {
                                            requesterProcess?.Kill();
                                            requesterProcess?.WaitForExit();
                                        }

                                        var cyberInstallerExePath = "";
                                        using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                                        {
                                            foreach (ZipArchiveEntry entry in archive.Entries)
                                            {
                                                if (Path.GetDirectoryName(entry.FullName) != CIBS_FOLDER_ZIP_PATH)
                                                {
                                                    var extractLocation = installFolderLocation
                                                        + "\\" + Path.GetFileName(entry.FullName);
                                                    entry.ExtractToFile(extractLocation
                                                        , overwrite: true);

                                                    if (Path.GetFileName(entry.FullName) == CYBER_INSTALLER_EXE_ZIP_PATH)
                                                    {
                                                        cyberInstallerExePath = extractLocation;
                                                    }
                                                }
                                            }

                                            // Re-open Cyber Installer
                                            if (File.Exists(cyberInstallerExePath))
                                            {
                                                Process p = new Process();
                                                p.StartInfo.FileName = cyberInstallerExePath;
                                                p.Start();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Install file not found, please retry!!");
                                    }

                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }
            Environment.Exit(0);
        }


    }
}