using LogGuard_v0._1.Base.Log;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Implement.AndroidLog;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.LogGuardFlow.SourceManager
{
    public class SourceManagerImpl : ISourceManager
    {
        private static SourceManagerImpl _instance;
        private RangeObservableCollection<LogWatcherItemViewModel> _rawSource;
        private RangeObservableCollection<LogWatcherItemViewModel> _displaySource;
        private List<ISourceHolder> _sourceHolder;
        private Dictionary<object, int> _logLevelCountMap;
        private RangeObservableCollection<string> _rawLog;

        public List<ISourceHolder> SourceHolders { get => _sourceHolder; }
        public RangeObservableCollection<LogWatcherItemViewModel> RawSource => _rawSource;
        public RangeObservableCollection<LogWatcherItemViewModel> DisplaySource => _displaySource;
        public ILogInfoManager LogInfoManager => LogInfoManagerImpl.Current;
        public RangeObservableCollection<string> RawLog { get => _rawLog; }

        public event SourceCollectionChangedHandler SourceCollectionChanged;

        private SourceManagerImpl()
        {
            _rawSource = new RangeObservableCollection<LogWatcherItemViewModel>();
            _displaySource = new RangeObservableCollection<LogWatcherItemViewModel>();
            _sourceHolder = new List<ISourceHolder>();
            _logLevelCountMap = new Dictionary<object, int>();
            _rawLog = new RangeObservableCollection<string>();
            ResetLogLevelCountMap();
        }

        private void ResetLogLevelCountMap()
        {
            _logLevelCountMap.Clear();
            _logLevelCountMap.Add("V", 0);
            _logLevelCountMap.Add("D", 0);
            _logLevelCountMap.Add("I", 0);
            _logLevelCountMap.Add("F", 0);
            _logLevelCountMap.Add("W", 0);
            _logLevelCountMap.Add("E", 0);
        }

        public void AddItem(string line)
        {
            _rawLog.Add(line);
            var item = LogInfoManager.ParseLogInfos(line, false, false);
            if (item != null)
            {
                LogWatcherItemViewModel livm = new LogWatcherItemViewModel(item);
                AddItem(livm);
            }
        }
        public void AddItem(LogWatcherItemViewModel model)
        {
            if (model == null)
            {
                return;
            }
            if (model.Level != null)
            {
                _logLevelCountMap[model.Level]++;
            }

            _rawSource.Add(model);
            _displaySource.Add(model);
            SourceCollectionChanged?.Invoke(this);
        }

        public void ClearSource()
        {
            LogInfoManager.ResetLogInfos();
            RawLog.Clear();
            ResetLogLevelCountMap();
            RawSource.Clear();
            DisplaySource.Clear();
            foreach (var holder in SourceHolders)
            {
                holder.ItemCount = 0;
            }

            SourceCollectionChanged?.Invoke(this);
        }

        public int DisplayItemsCount()
        {
            return DisplaySource.Count();
        }

        public int RawItemsCount()
        {
            return RawSource.Count();
        }

        public void RemoveItem(LogWatcherItemViewModel model)
        {
            RawSource.Remove(model);
            DisplaySource.Remove(model);
            SourceCollectionChanged?.Invoke(this);
        }

        public void AddSourceHolder(ISourceHolder holder)
        {
            _sourceHolder.Add(holder);
            holder.ItemsSource = DisplaySource;

        }

        public int ErrorItemsCount()
        {
            return _logLevelCountMap["E"];
        }

        public int InfoItemsCount()
        {
            return _logLevelCountMap["I"];
        }

        public int DebugItemsCount()
        {
            return _logLevelCountMap["D"];
        }

        public int WarningItemsCount()
        {
            return _logLevelCountMap["W"];
        }

        public int FatalItemsCount()
        {
            return _logLevelCountMap["F"];
        }

        public int VerboseItemsCount()
        {
            return _logLevelCountMap["V"];
        }

        public void UpdateLogParser(IRunThreadConfig runThreadConfig)
        {
            LogInfoManager.UpdateLogParser(runThreadConfig);
        }

        public static SourceManagerImpl Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SourceManagerImpl();
                }
                return _instance;
            }
        }

    }
}
