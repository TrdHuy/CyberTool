using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.Models
{
    enum LogLevel
    {
        NONE = 0,
        LEVEL_VERBOSE = 1,
        LEVEL_DEBUG = 2,
        LEVEL_INFO = 3,
        LEVEL_WARN = 4,
        LEVEL_ERROR = 5,
        LEVEL_FATAL = 6
    }

    public class LogInfo
    {
        public const string LEVEL_ALL = "VDFIEW";
        public const string LEVEL_VERBOSE = "V";
        public const string LEVEL_DEBUG = "D";
        public const string LEVEL_FATAL = "F";
        public const string LEVEL_INFO = "I";
        public const string LEVEL_ERROR = "E";
        public const string LEVEL_WARNING = "W";

        public const string COLOR_VERBOSE = "#000000";
        public const string COLOR_DEBUG = "#0063B1";
        public const string COLOR_INFO = "#00B294";
        public const string COLOR_WARN = "#FF8C00";
        public const string COLOR_ERROR = "#E81123";
        public const string COLOR_FATAL = "#D13438";

        public const string KEY_DATE = "Date";
        public const string KEY_TIME = "Time";
        public const string KEY_PID = "Pid";
        public const string KEY_TID = "Tid";
        public const string KEY_PACKAGE = "Package";
        public const string KEY_TAG = "Tag";
        public const string KEY_LEVEL = "Level";
        public const string KEY_MESSAGE = "Message";
        public const string KEY_LINE = "Line";
        public const string KEY_COLOR = "Color";
        public const string KEY_RAW_TEXT = "RawText";

        private Dictionary<string, object> _parts = new Dictionary<string, object>();

        public LogInfo()
        {
        }

        public object this[string key]
        {
            get
            {
                try
                {
                    return _parts[key];
                }
                catch (Exception e)
                {
                    return null;
                }

                ;
            }

            set { _parts[key] = value; }
        }

    }
}
