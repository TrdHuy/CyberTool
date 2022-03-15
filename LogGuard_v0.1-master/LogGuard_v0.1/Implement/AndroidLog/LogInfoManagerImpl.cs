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

        private AbstractLogParser _logParser;

        private int _logCounter = 0;

        private LogInfoManagerImpl()
        {
            ResetLogInfos();
            UpdateLogParser();
        }

        public void ResetLogInfos()
        {
            _logCounter = 0;
        }

        public LogInfo ParseLogInfos(string line, bool isReadFromFile, bool isReadFromDumpstateFile)
        {

            LogInfo logInfo = _logParser.ParseLogInfos(line, _logCounter);

            if (_logParser.IsMatch(line))
            {
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
