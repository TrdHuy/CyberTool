using log_guard.models.vo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.definitions
{
    internal class LogGuardDefinition
    {
        public const string LOG_GUARD_PAGE_URI_ORIGINAL_STRING = "/log_guard;component/views/usercontrols/LogGuard.xaml";
        public static readonly long LOG_GUARD_PAGE_LOADING_DELAY_TIME = 500;
        public const string LOG_GUARD_PAGE_HEADER_GEOMETRY_DATA = "M41,7.22A35.77,35.77,0,0,1,22.73.41,1.91,1.91,0,0,0,21.47,0,1.87,1.87,0,0,0,20.2.42,35.37,35.37,0,0,1,2.06,7.22L0,7.34H0l.31,2c.22,1.41,5.51,34.37,20.19,40.4a1.88,1.88,0,0,0,.8.25h.33a2,2,0,0,0,.8-.24c14.76-6,20.08-39,20.3-40.41l.31-2h0ZM34.67,26.06c-.69,2-1.41,3.91-2.15,5.65a12,12,0,0,1-22,0,106,106,0,0,1-6.17-21h0A39.5,39.5,0,0,0,21.47,4.09a39.77,39.77,0,0,0,17.2,6.63h0A119.05,119.05,0,0,1,34.67,26.06Z";

        public const string LOG_GUARD_SERVICE_TAG = "LogGuardService";
    }

    internal class LogGuardKeyFeatureTag
    {
        public const string KEY_TAG_MSW_LOGWATCHER_PLAY_FEATURE = "KEY_TAG_MSW_LOGWATCHER_PLAY_FEATURE";
        public const string KEY_TAG_MSW_LOGWATCHER_STOP_FEATURE = "KEY_TAG_MSW_LOGWATCHER_STOP_FEATURE";
        public const string KEY_TAG_MSW_LOGWATCHER_CLEAR_FEATURE = "KEY_TAG_MSW_LOGWATCHER_CLEAR_FEATURE";
        public const string KEY_TAG_MSW_LOGWATCHER_IMPORT_LOG_FILE_FEATURE = "KEY_TAG_MSW_LOGWATCHER_IMPORT_LOG_FILE_FEATURE";
        public const string KEY_TAG_MSW_LOGWATCHER_ZOOM_FEATURE = "KEY_TAG_MSW_LOGWATCHER_ZOOM_FEATURE";
        public const string KEY_TAG_MSW_LOGWATCHER_REFRESH_DEVICE_FEATURE = "KEY_TAG_MSW_LOGWATCHER_REFRESH_DEVICE_FEATURE";
        public const string KEY_TAG_MSW_LOGWATCHER_CTRLA_GESTURE_FEATURE = "KEY_TAG_MSW_LOGWATCHER_CTRLA_GESTURE_FEATURE";
        public const string KEY_TAG_MSW_LOGWATCHER_DELETE_GESTURE_FEATURE = "KEY_TAG_MSW_LOGWATCHER_DELETE_GESTURE_FEATURE";
        public const string KEY_TAG_MSW_LOGWATCHER_TAG_DOUBLE_CLICK_GESTURE_FEATURE = "KEY_TAG_MSW_LOGWATCHER_TAG_DOUBLE_CLICK_GESTURE_FEATURE";
        public const string KEY_TAG_MSW_LOGWATCHER_MESSAGE_DOUBLE_CLICK_GESTURE_FEATURE = "KEY_TAG_MSW_LOGWATCHER_MESSAGE_DOUBLE_CLICK_GESTURE_FEATURE";

        public const string KEY_TAG_MSW_LOGMANAGER_DELETE_TAG_ITEM_FEATURE = "KEY_TAG_MSW_LOGMANAGER_DELETE_TAG_ITEM_FEATURE";
        public const string KEY_TAG_MSW_LOGMANAGER_EDIT_TAG_ITEM_FEATURE = "KEY_TAG_MSW_LOGMANAGER_EDIT_TAG_ITEM_FEATURE";

        public const string KEY_TAG_MSW_LOGMANAGER_DELETE_MESSAGE_ITEM_FEATURE = "KEY_TAG_MSW_LOGMANAGER_DELETE_MESSAGE_ITEM_FEATURE";
        public const string KEY_TAG_MSW_LOGMANAGER_EDIT_MESSAGE_ITEM_FEATURE = "KEY_TAG_MSW_LOGMANAGER_EDIT_MESSAGE_ITEM_FEATURE";

        public const string KEY_TAG_MSW_LOGWATCHER_PARSER_ITEM_SELECTED_GESTURE_FEATURE = "KEY_TAG_MSW_LOGWATCHER_PARSER_ITEM_SELECTED_GESTURE_FEATURE";

    }

    internal class LogParserDefinition
    {
        public static readonly Dictionary<LogParserOption, LogParserVO> LOG_PARSER_FORMATS_LIST = new Dictionary<LogParserOption, LogParserVO>();

        static LogParserDefinition()
        {
            LOG_PARSER_FORMATS_LIST.Add(LogParserOption.MAIN_EVENTS_SYSTEM_PARSER, new LogParserVO()
            {
                DisplayName = "Parse \"main\", \"events\" & \"system\" log buffer"
            ,
                Cmd = " logcat -b main -b events -b system"
            ,
                FormatContact = LogParserFormatContact.NORMAL_ADB_COMMAND
            ,
                ParserTip = "Run command: adb logcat -b main -b events -b system"
            });

            LOG_PARSER_FORMATS_LIST.Add(LogParserOption.TIMEFORMAT_PARSER, new LogParserVO()
            {
                DisplayName = "Parse with 'time' format"
            ,
                Cmd = " logcat -v time"
            ,
                FormatContact = LogParserFormatContact.TIME_ADB_COMMAND
            ,
                ParserTip = "Run command: adb logcat -v time"
            });

            LOG_PARSER_FORMATS_LIST.Add(LogParserOption.RADIO_TIMEFORMAT_PARSER, new LogParserVO()
            {
                DisplayName = "Parse \"radio\" log buffer with 'time' format"
            ,
                Cmd = " logcat -b radio -v time"
            ,
                FormatContact = LogParserFormatContact.TIME_ADB_COMMAND
            ,
                ParserTip = "Run command: adb logcat -b radio -v time"
            });

            LOG_PARSER_FORMATS_LIST.Add(LogParserOption.EVENTS_TIMEFORMAT_PARSER, new LogParserVO()
            {
                DisplayName = "Parse \"events\" log buffer with 'time' format"
            ,
                Cmd = " logcat -b events -v time"
            ,
                FormatContact = LogParserFormatContact.TIME_ADB_COMMAND
            ,
                ParserTip = "Run command: adb logcat -b events -v time"
            });

            LOG_PARSER_FORMATS_LIST.Add(LogParserOption.THREADTIMEFORMAT_PARSER, new LogParserVO()
            {
                DisplayName = "Parse with 'threadtime' format"
            ,
                Cmd = " logcat -v threadtime"
            ,
                FormatContact = LogParserFormatContact.NORMAL_ADB_COMMAND
            ,
                ParserTip = "Run command: adb logcat -v threadtime"
            });

            LOG_PARSER_FORMATS_LIST.Add(LogParserOption.ALL_THREADTIMEFORMAT_PARSER, new LogParserVO()
            {
                DisplayName = "Parse all log buffers with 'threadtime' format"
            ,
                Cmd = " logcat -v threadtime -b all"
            ,
                FormatContact = LogParserFormatContact.NORMAL_ADB_COMMAND
            ,
                ParserTip = "Run command: adb logcat -v threadtime -b all"
            });

            LOG_PARSER_FORMATS_LIST.Add(LogParserOption.ALL_PARSER, new LogParserVO()
            {
                DisplayName = "Parse all log buffers"
            ,
                Cmd = " logcat all"
            ,
                FormatContact = LogParserFormatContact.NORMAL_ADB_COMMAND
            ,
                ParserTip = "Run command: adb logcat all"
            });

            LOG_PARSER_FORMATS_LIST.Add(LogParserOption.DUMPSTATE_PARSER, new LogParserVO()
            {
                DisplayName = "Import dumpstate"
            ,
                Cmd = ""
            ,
                FormatContact = LogParserFormatContact.OPEN_DUMPSTATE_FILE
            ,
                ParserTip = "Open a log file with dumpstate format"
            });
        }


    }

    public enum ElementViewType
    {
        /// <summary>
        /// kiểu view cho android log
        /// </summary>
        LogView = 0,

        /// <summary>
        /// kiểu view cho row có thể mở rộng 
        /// </summary>
        ExpandableRowView = 1,
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
    public enum LogGuardState
    {
        NONE = 0,
        RUNNING = 1,
        PAUSING = 2,
        STOP = 3,
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
        LogWatcherContentControl = 1,
        LogWatcherZoomButton = 2,
        LogWatcher = 2,
    }
}
