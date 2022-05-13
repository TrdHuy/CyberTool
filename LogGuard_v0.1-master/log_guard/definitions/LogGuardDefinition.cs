using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.definitions
{
    public class LogGuardDefinition
    {
        public const string LOG_GUARD_PAGE_URI_ORIGINAL_STRING = "/log_guard;component/views/usercontrols/LogGuard.xaml";
        public static readonly long LOG_GUARD_PAGE_LOADING_DELAY_TIME = 500;

        public const string LOG_GUARD_PAGE_HEADER_GEOMETRY_DATA = "M41,7.22A35.77,35.77,0,0,1,22.73.41,1.91,1.91,0,0,0,21.47,0,1.87,1.87,0,0,0,20.2.42,35.37,35.37,0,0,1,2.06,7.22L0,7.34H0l.31,2c.22,1.41,5.51,34.37,20.19,40.4a1.88,1.88,0,0,0,.8.25h.33a2,2,0,0,0,.8-.24c14.76-6,20.08-39,20.3-40.41l.31-2h0ZM34.67,26.06c-.69,2-1.41,3.91-2.15,5.65a12,12,0,0,1-22,0,106,106,0,0,1-6.17-21h0A39.5,39.5,0,0,0,21.47,4.09a39.77,39.77,0,0,0,17.2,6.63h0A119.05,119.05,0,0,1,34.67,26.06Z";

    }


    public enum LogGuardState
    {
        NONE = 0,
        RUNNING = 1,
        PAUSING = 2,
        STOP = 3,
    }

    public enum LogParserOption
    {
        MAIN_EVENTS_SYSTEM_PARSER = 1,
        TIMEFORMAT_PARSER = 2,
        RADIO_TIMEFORMAT_PARSER = 3,
        EVENTS_TIMEFORMAT_PARSER = 4,
        THREADTIMEFORMAT_PARSER = 5,
        ALL_THREADTIMEFORMAT_PARSER = 6,
        ALL_PARSER = 7,
        DUMPSTATE_PARSER = 8,
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

    public enum LogGuardViewKeyDefinition
    {
        LogWatcherViewer = 1,
        LogWatcherZoomButton = 2,
    }
}
