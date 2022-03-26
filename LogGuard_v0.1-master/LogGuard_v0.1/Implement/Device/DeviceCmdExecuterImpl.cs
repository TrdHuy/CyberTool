using LogGuard_v0._1.Base.Device;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.Device
{
    public class DeviceCmdExecuterImpl : IDeviceCmdExecuter
    {
        private static DeviceCmdExecuterImpl _instance;

        public Process CreateProcess(string cmd)
        {
            Process process = new Process();
            process.StartInfo.FileName = "adb.exe";
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = cmd;
            process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;

            return process;
        }

        public string CreateCommandADB(string command, int type, bool asroot, bool multiDevice, string serialNumber)
        {
            string cmd = "";

            switch (type)
            {
                case DeviceCmdContact.ADB_SHELL_COMMAND_TYPE:
                    cmd = asroot ? (multiDevice ? " -s " + serialNumber : "") + " shell su -c " + command :
                        (multiDevice ? " -s " + serialNumber : "") + " shell " + command;
                    break;
                case DeviceCmdContact.ADB_NONE_SHELL_COMMAND_TYPE:
                    cmd = (multiDevice ? " -s " + serialNumber : "") + command;
                    break;
                default:
                    break;
            }

            return cmd;
        }

        public static DeviceCmdExecuterImpl Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DeviceCmdExecuterImpl();
                }
                return _instance;
            }
        }
    }
}