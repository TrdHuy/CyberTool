using cyber_base.implement.utils;
using log_guard.@base.flow;
using log_guard.@base.module;
using log_guard.implement.module;
using log_guard.models.vo;
using log_guard.utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.flow.run_thread_config
{
    internal class RunThreadConfigManager : BaseLogGuardModule, IRunThreadConfigManager
    {
        private static Logger logger = new Logger("RunThreadConfigManager");
        private const string _configPath = @"\config.json";
        private RunThreadConfigImpl _config;

        public event OnConfigChangedHandler? ConfigChanged;

        public static RunThreadConfigManager Current
        {
            get
            {
                return LogGuardModuleManager.RTCM_Instance;
            }
        }

        public LogParserVO CurrentParser
        {
            get
            {
                return _config.LogParserFormat;
            }
            set
            {
                var oldValue = _config.LogParserFormat;
                _config.LogParserFormat = value;
                ConfigChanged?.Invoke(this, new OnConfigChangedArgs(oldValue, value));
            }
        }

        public List<TrippleToggleItemVO> TagEmployees
        {
            get
            {
                return _config.LogTags;
            }
            set
            {
                var oldValue = _config.LogTags;
                _config.LogTags = value;
                ConfigChanged?.Invoke(this, new OnConfigChangedArgs(oldValue, value));
            }
        }

        public List<TrippleToggleItemVO> MessageEmployees
        {
            get
            {
                return _config.LogMessages;
            }
            set
            {
                var oldValue = _config.LogMessages;
                _config.LogMessages = value;
                ConfigChanged?.Invoke(this, new OnConfigChangedArgs(oldValue, value));
            }
        }


        public IRunThreadConfig CurrentConfig { get => _config; }

        public void ExportConfig()
        {
            FileIOManager.ExportJsonToDataFile(_configPath, _config);
        }

        public RunThreadConfigManager()
        {
            _config = new RunThreadConfigImpl();

            var watch = Stopwatch.StartNew();

            _config.Init();

            watch.Stop();

#if DEBUG
            Console.WriteLine("Total load config from file time = " + watch.ElapsedMilliseconds + "(ms)");
#endif
            logger.I("Total load config from file time = " + watch.ElapsedMilliseconds + "(ms)");
            if (_config.LogTags == null)
            {
                TagEmployees = new List<TrippleToggleItemVO>();
            }

            if (_config.LogMessages == null)
            {
                MessageEmployees = new List<TrippleToggleItemVO>();
            }
        }

        private class RunThreadConfigImpl : IRunThreadConfig
        {
            private const string _configPath = @"\config.json";
            private LogParserVO _logParserFormat;
            private List<TrippleToggleItemVO> _tags;
            private List<TrippleToggleItemVO> _messages;
            public List<TrippleToggleItemVO> LogTags { get => _tags; set => _tags = value; }
            public List<TrippleToggleItemVO> LogMessages { get => _messages; set => _messages = value; }
            public LogParserVO LogParserFormat { get => _logParserFormat; set => _logParserFormat = value; }

            public RunThreadConfigImpl()
            {
                _logParserFormat = new LogParserVO();
                _tags = new List<TrippleToggleItemVO>();
                _messages = new List<TrippleToggleItemVO>();
            }

            public void Init()
            {
                var config = FileIOManager.LoadJsonFromDataFile<RunThreadConfigImpl>(_configPath);

                if (config != null)
                {
                    LogTags = config.LogTags;
                    LogMessages = config.LogMessages;
                    LogParserFormat = config.LogParserFormat;
                }

            }

        }
    }

    public delegate void OnConfigChangedHandler(object sender, OnConfigChangedArgs args);

    public class OnConfigChangedArgs : EventArgs
    {
        public object OldValue { get; }
        public object NewValue { get; }

        public OnConfigChangedArgs(object oldV, object newV)
        {
            OldValue = oldV;
            NewValue = newV;
        }
    }
}
