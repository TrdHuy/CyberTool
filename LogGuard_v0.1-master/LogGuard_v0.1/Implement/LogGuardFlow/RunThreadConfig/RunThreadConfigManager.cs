using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.LogGuardFlow.RunThreadConfig
{
    public class RunThreadConfigManager
    {
        private static Logger logger = new Logger("RunThreadConfigManager");
        private static RunThreadConfigManager _instance;
        private const string _configPath = @"\config.json";
        private RunThreadConfigImpl _config;

        public event OnConfigChangedHandler ConfigChanged;
        public event OnTagsCollectionChangedHandler TagsCollectionChanged;

        public static RunThreadConfigManager Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RunThreadConfigManager();
                }
                return _instance;
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

        public List<LogTagVO> TagEmployees
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
        public IRunThreadConfig CurrentConfig { get => _config; }


        private RunThreadConfigManager()
        {
            _config = new RunThreadConfigImpl();
        }

        public void Init()
        {
            var watch = Stopwatch.StartNew();

            _config.Init();

            watch.Stop();

#if DEBUG
            Console.WriteLine("Total load config from file time = " + watch.ElapsedMilliseconds + "(ms)");
#endif
            logger.I("Total load config from file time = " + watch.ElapsedMilliseconds + "(ms)");
            if (_config.LogTags == null)
            {
                TagEmployees = new List<LogTagVO>();
            }
        }


        public void ExportConfig()
        {
            FileIOManager.Current.ExportJsonToDataFile(_configPath, _config);
        }


        private void OnTagEmployeesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TagsCollectionChanged?.Invoke(sender, e);
        }

        private class RunThreadConfigImpl : IRunThreadConfig
        {
            private const string _configPath = @"\config.json";
            private LogParserVO _logParserFormat;
            private List<LogTagVO> _tags;
            public List<LogTagVO> LogTags { get => _tags; set => _tags = value; }
            public LogParserVO LogParserFormat { get => _logParserFormat; set => _logParserFormat = value; }

            public RunThreadConfigImpl()
            {
            }

            public void Init()
            {
                var config = FileIOManager.Current.LoadJsonFromDataFile<RunThreadConfigImpl>(_configPath);
                LogTags = new List<LogTagVO>();
                if (config != null)
                {
                    LogTags = config.LogTags;
                    LogParserFormat = config.LogParserFormat;
                }

            }

        }
    }

    public delegate void OnConfigChangedHandler(object sender, OnConfigChangedArgs args);
    public delegate void OnTagsCollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs args);

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
