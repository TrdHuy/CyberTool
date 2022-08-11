using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.models.info
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
        static LogInfo()
        {
            COLOR_VERBOSE = ColorTranslator.FromHtml("#AA52ABE8");
            COLOR_DEBUG = ColorTranslator.FromHtml("#AAADFF55");
            COLOR_WARN = ColorTranslator.FromHtml("#AAFFB855");
            COLOR_ERROR = ColorTranslator.FromHtml("#AAFF6255");
            COLOR_INFO = ColorTranslator.FromHtml("#AACAF1E1");
            COLOR_FATAL = ColorTranslator.FromHtml("#AAF0ADF1");
            COLOR_DEFAULT = ColorTranslator.FromHtml("#AA7E7D7E");
        }

        public const string LEVEL_ALL = "VDFIEW";
        public const string LEVEL_VERBOSE = "V";
        public const string LEVEL_DEBUG = "D";
        public const string LEVEL_FATAL = "F";
        public const string LEVEL_INFO = "I";
        public const string LEVEL_ERROR = "E";
        public const string LEVEL_WARNING = "W";

        public const int LOG_LEVEL_COUNT = 6;
        public const int LEVEL_VERBOSE_INDEX = 0;
        public const int LEVEL_DEBUG_INDEX = 1;
        public const int LEVEL_FATAL_INDEX = 2;
        public const int LEVEL_INFO_INDEX = 3;
        public const int LEVEL_ERROR_INDEX = 4;
        public const int LEVEL_WARNING_INDEX = 5;

        public static Color COLOR_VERBOSE;
        public static Color COLOR_DEBUG;
        public static Color COLOR_INFO;
        public static Color COLOR_WARN;
        public static Color COLOR_ERROR;
        public static Color COLOR_FATAL;
        public static Color COLOR_DEFAULT;

        public const string KEY_DATE = "Date";
        public const string KEY_TIME = "Time";
        public const string KEY_DATE_TIME = "DateTime";
        public const string KEY_DATE_TIME_S = "DateTimeStr";
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
