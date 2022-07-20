using log_guard.@base._log.android_log;
using log_guard.@base.flow;
using log_guard.@base.module;
using log_guard.definitions;
using log_guard.implement.flow.log_manager.parsers;
using log_guard.implement.module;
using log_guard.models.info;

namespace log_guard.implement.flow.log_manager
{
    internal class LogInfoManager : BaseLogGuardModule, ILogInfoManager
    {

        private AbstractLogParser _logParser;

        private int _logCounter = 0;

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

        public override void OnModuleStart()
        {
            ResetLogInfos();
        }

        public static LogInfoManager Current
        {
            get
            {
                return LogGuardModuleManager.LIF_Instance;
            }
        }
    }
}
