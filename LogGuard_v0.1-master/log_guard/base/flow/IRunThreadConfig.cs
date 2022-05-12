using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.flow
{
    public interface IRunThreadConfig
    {
        LogParserVO LogParserFormat { get; set; }
        List<TrippleToggleItemVO> LogTags { get; set; }
        List<TrippleToggleItemVO> LogMessages { get; set; }
    }


    public class TrippleToggleItemVO
    {
        public string Content { get; set; }
        public Status Stat { get; set; }

        public TrippleToggleItemVO(string content)
        {
            Content = content;
            Stat = Status.None;
        }

        public enum Status
        {
            None = 0,
            Show = 1,
            Remove = 2
        }

    }

    public class LogParserVO
    {
        public string DisplayName { get; set; }
        public string ParserTip { get; set; }
        public string Cmd { get; set; }
        public LogParserFormatContact FormatContact { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public enum LogParserFormatContact
    {
        NONE = -1,
        NORMAL_ADB_COMMAND = 0,
        TIME_ADB_COMMAND = 1,
        SHELL_CAT_ADB_COMMAND = 2,
        OPEN_LOG_FILE = 3,
        OPEN_DUMPSTATE_FILE = 4
    }

}
