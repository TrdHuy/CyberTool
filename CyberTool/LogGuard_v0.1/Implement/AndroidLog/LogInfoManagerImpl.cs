using LogGuard_v0._1.Base.AndroidLog;
using LogGuard_v0._1.Base.AndroidLog.LogParser;
using LogGuard_v0._1.Base.Log;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Implement.AndroidLog.LogParser;

namespace LogGuard_v0._1.Implement.AndroidLog
{
    public class LogInfoManagerImpl : ILogInfoManager
    {
        private static LogInfoManagerImpl _instance;

        private AbstractLogParser _logParser;

        private int _logCounter = 0;

        private LogInfoManagerImpl()
        {
            ResetLogInfos();
        }

        public void ResetLogInfos()
        {
            _logCounter = 0;
        }

        public LogInfo ParseLogInfos(string line, bool isReadFromFile, bool isReadFromDumpstateFile)
        {

            LogInfo logInfo = _logParser.ParseLogInfos(line, _logCounter);

            if (logInfo != null)
            {
                _logCounter++;
                return logInfo;
            }
            else
            {
                return null;
            }
        }

        public void UpdateLogParser(IRunThreadConfig runThreadConfig)
        {

            switch (runThreadConfig.LogParserFormat.FormatContact)
            {
                case LogParserFormatContact.NORMAL_ADB_COMMAND:
                    _logParser = new AdbCmdLogParser();
                    break;
                case LogParserFormatContact.TIME_ADB_COMMAND:
                    _logParser = new TimeAdbCmdLogParser();
                    break;
                case LogParserFormatContact.OPEN_DUMPSTATE_FILE:
                    _logParser = new DumpstateLogParser();
                    break;
                default:
                    _logParser = new AdbCmdLogParser();
                    break;
            }
        }


        public static LogInfoManagerImpl Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogInfoManagerImpl();
                }
                return _instance;
            }
        }
    }
}
