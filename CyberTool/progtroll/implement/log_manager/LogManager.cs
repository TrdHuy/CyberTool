using progtroll.implement.module;
using System;

namespace progtroll.implement.log_manager
{
    internal class LogManager : BasePublisherModule
    {
        private string _rtLogContent = "";

        public event LogContentChangedHandler? LogContentChanged;

        public string LogContent
        {
            get
            {
                return _rtLogContent;
            }
            private set
            {
                _rtLogContent = value;
                LogContentChanged?.Invoke(this);
            }
        }

        public static LogManager Current
        {
            get
            {
                return PublisherModuleManager.LM_Instance;
            }
        }

        public void AppendLogLine(string line, bool isTimeAppend = false)
        {
            if (!isTimeAppend)
            {
                LogContent += line + "\n";
            }
            else
            {
                var now = DateTime.Now.ToString("HH:mm:ss.fff");
                LogContent += now + ": " + line + "\n";
            }
        }

        public void AppendLog(string log)
        {
            LogContent += log;
        }

        public void ClearLog()
        {
            LogContent = "";
        }
    }

    internal delegate void LogContentChangedHandler(object sender);
}
