using System.Diagnostics;

namespace cyber_installer_background_service
{
    internal class Program
    {
        private const string CYBER_INSTALLER_REQUESTER_ID = "CyberInstallerWindow{0367E847-B5C3-4CDD-9C34-B78A769AF73C}";
        private const string UPDATE_CYBER_INSTALLER_CMD = "UpdateCyberInstaller";

        static void Main(string[] args)
        {
            Console.ReadKey();
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }
            var requesterID = "";
            var requesterProcessId = -1;
            var cmdID = "";
            if (args.Length == 3)
            {
                requesterID = args[0];
                cmdID = args[1];
                requesterProcessId = Convert.ToInt32(args[2]);
                switch (requesterID)
                {
                    case CYBER_INSTALLER_REQUESTER_ID:
                        {
                            if (cmdID == UPDATE_CYBER_INSTALLER_CMD)
                            {

                                try
                                {
                                    var requesterProcess = Process.GetProcessById(requesterProcessId);
                                    if (!requesterProcess.HasExited && requesterProcess?.ProcessName == "Cyber Installer")
                                    {
                                        requesterProcess?.Kill();
                                        requesterProcess?.WaitForExit();
                                    }
                                }
                                catch
                                {

                                }
                                
                            }
                            break;
                        }
                }

                Console.WriteLine("Press any key to close the program!");
                Console.ReadKey();
            }
        }
    }
}