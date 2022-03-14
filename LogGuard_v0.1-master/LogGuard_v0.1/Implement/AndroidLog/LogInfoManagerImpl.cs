using LogGuard_v0._1.Base.AndroidLog.LogParser;
using LogGuard_v0._1.Base.Log;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.AndroidLog.LogParser;
using LogGuard_v0._1.Windows.MainWindow.Models;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.AndroidLog
{
    public class LogInfoManagerImpl : ILogInfoManager
    {
        private static LogInfoManagerImpl _instance;

        private Dictionary<string, List<LogInfo>> _logInfos = new Dictionary<string, List<LogInfo>>();
        private List<BaseViewModel> _logItems = new List<BaseViewModel>();

        private AbstractLogParser _logParser;

        private int _logCounter = 0;

        private LogInfoManagerImpl()
        {
            ResetLogInfos();
            UpdateLogParser();
        }

        public void ResetLogInfos()
        {
            _logInfos.Clear();
            _logItems.Clear();
            _logCounter = 0;

            _logInfos.Add(LogInfo.LEVEL_VERBOSE, new List<LogInfo>());
            _logInfos.Add(LogInfo.LEVEL_ERROR, new List<LogInfo>());
            _logInfos.Add(LogInfo.LEVEL_FATAL, new List<LogInfo>());
            _logInfos.Add(LogInfo.LEVEL_INFO, new List<LogInfo>());
            _logInfos.Add(LogInfo.LEVEL_WARNING, new List<LogInfo>());
            _logInfos.Add(LogInfo.LEVEL_DEBUG, new List<LogInfo>());
            _logInfos.Add(LogInfo.LEVEL_ALL, new List<LogInfo>());
        }

        public LogInfo ParseLogInfos(string line, bool isReadFromFile, bool isReadFromDumpstateFile)
        {

            LogInfo logInfo = _logParser.ParseLogInfos(line, _logCounter);

            if (_logParser.IsMatch(line))
            {
                //_logInfos[(string)logInfo[LogInfo.KEY_LEVEL]].Add(logInfo);

                //_logInfos[LogInfo.LEVEL_ALL].Add(logInfo);

                //_logItems.Add(new LogWatcherItemViewModel(logInfo));

                _logCounter++;

                return logInfo;
            }
            else
            {
                return null;
            }
        }

        public void UpdateLogParser()
        {

            _logParser = new AdbCmdLogParser();

            //switch (_runThreadConfig.LogParserFormat)
            //{
            //    case LogParserFormatContact.NORMAL_ADB_COMMAND:
            //        _logParser = new AdbCmdLogParser();
            //        break;
            //    case LogParserFormatContact.TIME_ADB_COMMAND:
            //        _logParser = new TimeAdbCmdLogParser();
            //        break;
            //    case LogParserFormatContact.NONE:
            //        break;
            //    default:
            //        _logParser = new AdbCmdLogParser();
            //        break;
            //}
        }

        public List<LogInfo> GetLogInfosByLevel(string level)
        {
            return _logInfos[level];
        }

        public List<BaseViewModel> GetLogItemVMs()
        {
            return _logItems;
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
