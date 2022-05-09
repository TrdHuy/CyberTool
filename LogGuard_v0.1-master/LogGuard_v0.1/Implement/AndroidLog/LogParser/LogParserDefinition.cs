using LogGuard_v0._1.Base.LogGuardFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.AndroidLog.LogParser
{
    public class LogParserDefinition
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
}
