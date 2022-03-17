using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.LogGuardFlow
{
    public interface IRunThreadConfig
    {
        string CurLogLevelFilter { get; set; }

        LogParserVO LogParserFormat { get; set; }
    }

    public class LogParserVO
    {
        public string Cmd { get; set; }
        public int FormatContact { get; set; }

        public override string ToString()
        {
            return Cmd;
        }
    }

    public class LogParserFormatContact
    {
        public const int NONE = -1;
        public const int NORMAL_ADB_COMMAND = 0;
        public const int TIME_ADB_COMMAND = 1;
        public const int SHELL_CAT_ADB_COMMAND = 2;
        public const int OPEN_LOG_FILE = 3;
        public const int OPEN_DUMPSTATE_FILE = 4;
    }
}
