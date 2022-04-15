using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.Models.Builder
{
    public class LogBuilder
    {
        private static int _currentYear = DateTime.Now.Year;
        private LogInfo _logInfo;

        public LogInfo LogInfo
        {
            get { return _logInfo; }
        }

        // A fresh builder instance should contain a blank log object, which
        // is used in further assembly.
        public LogBuilder()
        {
            _logInfo = new LogInfo();
        }

        public void Reset()
        {
            this._logInfo = new LogInfo();
        }


        // All log val with the same log line instance.
        public LogBuilder BuildLine(object val)
        {
            _logInfo[LogInfo.KEY_LINE] = val;
            return this;
        }

        public LogBuilder BuildDate(object val)
        {
            _logInfo[LogInfo.KEY_DATE] = val;
            return this;
        }

        public LogBuilder BuildTime(object val)
        {
            _logInfo[LogInfo.KEY_TIME] = val;
            return this;
        }

        public LogBuilder BuildTID(object val)
        {
            try
            {
                _logInfo[LogInfo.KEY_TID] = Int32.Parse(val.ToString());
            }
            catch (Exception e)
            {
                _logInfo[LogInfo.KEY_TID] = "-";
            }
            return this;
        }

        public LogBuilder BuildPID(object val)
        {
            try
            {
                _logInfo[LogInfo.KEY_PID] = Int32.Parse(val.ToString());
            }
            catch (Exception e)
            {
                _logInfo[LogInfo.KEY_PID] = "-";
            }
            return this;
        }

        public LogBuilder BuildLevel(object val)
        {
            _logInfo[LogInfo.KEY_LEVEL] = val;
            return this;
        }

        public LogBuilder BuildPackage(object val)
        {
            _logInfo[LogInfo.KEY_PACKAGE] = val;
            return this;
        }

        public LogBuilder BuildTag(object val)
        {
            _logInfo[LogInfo.KEY_TAG] = val;
            return this;
        }

        public LogBuilder BuildMessage(object val)
        {
            _logInfo[LogInfo.KEY_MESSAGE] = val;
            return this;
        }

        public LogBuilder BuildColor(object val)
        {
            _logInfo[LogInfo.KEY_COLOR] = val;
            return this;
        }

        public LogBuilder BuildRawText(object val)
        {
            _logInfo[LogInfo.KEY_RAW_TEXT] = val;
            return this;
        }

        public LogBuilder BuildDateTime()
        {
            var date = _logInfo[LogInfo.KEY_DATE];
            var time = _logInfo[LogInfo.KEY_TIME];
            var dateTime = date + "-" + _currentYear + " " + time;

            _logInfo[LogInfo.KEY_DATE_TIME_S] = dateTime;
            _logInfo[LogInfo.KEY_DATE_TIME] = DateTime.ParseExact(dateTime
                , "MM-dd-yyyy HH:mm:ss.fff"
                , System.Globalization.CultureInfo.CurrentCulture);
            return this;
        }

        public LogInfo Build()
        {
            return _logInfo;
        }

        public LogBuilder BuildColorByLevel(object level)
        {
            switch (level.ToString())
            {
                case LogInfo.LEVEL_DEBUG:
                    {
                        BuildColor(LogInfo.COLOR_DEBUG);
                        BuildLevel(LogInfo.LEVEL_DEBUG);
                        break;
                    }
                case LogInfo.LEVEL_ERROR:
                    {
                        BuildColor(LogInfo.COLOR_ERROR);
                        BuildLevel(LogInfo.LEVEL_ERROR);
                        break;
                    }
                case LogInfo.LEVEL_FATAL:
                    {
                        BuildColor(LogInfo.COLOR_FATAL);
                        BuildLevel(LogInfo.LEVEL_FATAL);
                        break;
                    }
                case LogInfo.LEVEL_WARNING:
                    {
                        BuildColor(LogInfo.COLOR_WARN);
                        BuildLevel(LogInfo.LEVEL_WARNING);
                        break;
                    }
                case LogInfo.LEVEL_INFO:
                    {
                        BuildColor(LogInfo.COLOR_INFO);
                        BuildLevel(LogInfo.LEVEL_INFO);
                        break;
                    }
                case LogInfo.LEVEL_VERBOSE:
                    {
                        BuildColor(LogInfo.COLOR_VERBOSE);
                        BuildLevel(LogInfo.LEVEL_VERBOSE);
                        break;
                    }
                default:
                    {
                        BuildColor(LogInfo.COLOR_VERBOSE);
                        BuildLevel(LogInfo.LEVEL_VERBOSE);
                        break;
                    }
            }
            return this;
        }


    }
}
