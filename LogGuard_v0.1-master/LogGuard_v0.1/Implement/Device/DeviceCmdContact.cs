using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.Device
{
    public class DeviceCmdContact
    {
        public const string CMD_ROOT = "adb ";
        public const string CMD_DEVICES = " devices ";
        public const string CMD_ADB_SHELL = "shell ";
        public const string CMD_BUILD_NUMBER = "getprop ril.official_cscver";
        public const string CMD_SERIAL_NUMBER = "getprop ril.serialnumber";
        public const string CMD_KILL_SERVER = " kill-server";

        public const string ADB_PATH = @"adb.exe";

        public const int ADB_SHELL_COMMAND_TYPE = 1;
        public const int ADB_NONE_SHELL_COMMAND_TYPE = 2;
    }
}
