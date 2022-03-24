
using LogGuard_v0._1.Base.AsyncTask;
using LogGuard_v0._1.Base.Log;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Implement.AndroidLog;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        private SourceFilterManagerImpl _sourceFilter;

        public List<ISourceHolder> SourceHolders { get => _sourceHolder; }
        public RangeObservableCollection<LogWatcherItemViewModel> RawSource => _rawSource;
        public RangeObservableCollection<LogWatcherItemViewModel> DisplaySource => _displaySource;
        public ILogInfoManager LogInfoManager => LogInfoManagerImpl.Current;
        public RangeObservableCollection<string> RawLog { get => _rawLog; }

        public event SourceCollectionChangedHandler SourceCollectionChanged;
        public ISourceFilterManager SourceFilterManager => _sourceFilter;
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

        private SourceManagerImpl()
        {
            _sourceFilter = SourceFilterManagerImpl.Current;
            _rawSource = new RangeObservableCollection<LogWatcherItemViewModel>();
            _displaySource = new RangeObservableCollection<LogWatcherItemViewModel>();
            _sourceHolder = new List<ISourceHolder>();
            _logLevelCountMap = new Dictionary<object, int>();
            _rawLog = new RangeObservableCollection<string>();
            ResetLogLevelCountMap();

            _sourceFilter.FilterConditionChanged -= OnFilterConditionChanged;
            _sourceFilter.FilterConditionChanged += OnFilterConditionChanged;
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

            lock (RawSource.ThreadSafeLock)
            {
                _rawSource.Add(model);
            }

            lock (DisplaySource.ThreadSafeLock)
            {
                if (SourceFilterManager.Filter(model))
                {
                    _displaySource.Add(model);
                }
            }

            
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

            // Attach display source for the Holder
            holder.ItemsSource = DisplaySource;
        }

        public void RemoveSourceHolder(ISourceHolder holder)
        {
            _sourceHolder.Remove(holder);

            // Detach display source for the Holder
            holder.ItemsSource = null;
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

        #region Filter area
        private CancellationTokenSource SourceFilterCancellationTokenCache { get; set; }
        private AsyncTask FilterTaskCache { get; set; }
        private void OnFilterConditionChanged(object sender, ConditionChangedEventArgs e)
        {
            if (FilterTaskCache != null)
            {
                if (!FilterTaskCache.IsCompleted)
                {
                    AsyncTask.CancelAsyncExecute(FilterTaskCache);
                }
            }

            SourceFilterCancellationTokenCache = new CancellationTokenSource();
            FilterTaskCache = new AsyncTask(OnDoFilterSource
                   , null
                   , OnFinishFilterSource
                   , 0
                   , SourceFilterCancellationTokenCache);
            AsyncTask.CancelableAsyncExecute(FilterTaskCache);
        }

        private async Task<AsyncTaskResult> OnDoFilterSource(CancellationToken token)
        {
            var result = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);


            var filterLst = new List<LogWatcherItemViewModel>();

            lock (RawSource.ThreadSafeLock)
            {
                foreach (var item in RawSource)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    if (SourceFilterManager.Filter(item))
                    {
                        filterLst.Add(item);
                    }
                }
            }

            result.Result = filterLst;
            result.MesResult = MessageAsyncTaskResult.Done;
            return result;
        }

        private void OnFinishFilterSource(AsyncTaskResult result)
        {
            if (result.MesResult == MessageAsyncTaskResult.Done)
            {
                lock (DisplaySource.ThreadSafeLock)
                {
                    _displaySource.AddNewRange((IEnumerable<LogWatcherItemViewModel>)result.Result);
                }
            }
        }

        #endregion
    }
}
