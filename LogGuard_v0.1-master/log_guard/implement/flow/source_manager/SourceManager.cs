
using cyber_base.implement.command;
using cyber_base.implement.utils;
using cyber_base.utils.async_task;
using log_guard.@base.flow;
using log_guard.@base.flow.highlight;
using log_guard.@base.flow.source_filter;
using log_guard.@base.module;
using log_guard.@base.watcher;
using log_guard.definitions;
using log_guard.implement.flow.log_manager;
using log_guard.implement.flow.source_filter_manager;
using log_guard.implement.flow.source_highlight_manager;
using log_guard.implement.flow.view_model;
using log_guard.implement.module;
using log_guard.view_models.watcher;
using log_guard.views.others.log_watcher._item;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace log_guard.implement.flow.source_manager
{
    internal class SourceManager : ISourceManager, ILogGuardModule
    {
        private static Logger Logger { get; } = new Logger("SourceManager");
        private object stateLockObject = new Object();

        private RangeObservableCollection<ILogWatcherElements> _rawSource;
        private RangeObservableCollection<ILogWatcherElements> _displaySource;
        private List<ISourceHolder> _sourceHolder;
        private Dictionary<object, int> _logLevelCountMap;
        private RangeObservableCollection<string> _rawLog;
        private SourceFilterManager? _sourceFilter;
        private SourceHighlightManager? _sourceHighlighter;

        public List<ISourceHolder> SourceHolders { get => _sourceHolder; }
        public RangeObservableCollection<ILogWatcherElements> RawSource => _rawSource;
        public RangeObservableCollection<ILogWatcherElements> DisplaySource => _displaySource;
        public ILogInfoManager LogManager => LogInfoManager.Current;

        public RangeObservableCollection<string> RawLog { get => _rawLog; }

        public event SourceCollectionChangedHandler? SourceCollectionChanged;
        public event SourceFilteredAndDisplayedHandler? SourceFilteredAndDisplayed;

        public ISourceFilterManager? SrcFilterManager => _sourceFilter;
        public ISourceHighlightManager? SrcHighlightManager => _sourceHighlighter;
        public static SourceManager Current
        {
            get
            {
                return LogGuardModuleManager.SM_Instance;
            }
        }

        public SourceManager()
        {
            _rawSource = new RangeObservableCollection<ILogWatcherElements>();
            _displaySource = new RangeObservableCollection<ILogWatcherElements>();
            _sourceHolder = new List<ISourceHolder>();
            _logLevelCountMap = new Dictionary<object, int>();
            _rawLog = new RangeObservableCollection<string>();
            ResetLogLevelCountMap();
        }

        public void OnModuleStart()
        {
            _sourceFilter = SourceFilterManager.Current;
            _sourceHighlighter = SourceHighlightManager.Current;

            _sourceFilter.FilterConditionChanged -= OnFilterConditionChanged;
            _sourceFilter.FilterConditionChanged += OnFilterConditionChanged;
            _sourceHighlighter.HighlightConditionChanged -= OnHighlightConditionChanged;
            _sourceHighlighter.HighlightConditionChanged += OnHighlightConditionChanged;
        }

        public void AddItem(string line)
        {
            _rawLog.Add(line);
            var item = LogManager.ParseLogInfos(line, false, false);
            if (item != null)
            {
                LWI_ParseableViewModel livm = new LWI_ParseableViewModel(
                    ViewModelManager.Current.LogGuardViewModel
                    , item);
                AddItem(livm);
            }
        }

        public void AddItem(ILogWatcherElements model)
        {

            if (model == null)
            {
                return;
            }

            if (model is LWI_ParseableViewModel)
            {
                _logLevelCountMap[(model as LWI_ParseableViewModel).Level]++;
            }

            lock (stateLockObject)
            {
                lock (RawSource.ThreadSafeLock)
                {
                    _rawSource.Add(model);
                }

                lock (DisplaySource.ThreadSafeLock)
                {
                    if (SrcFilterManager.Filter(model))
                    {
                        SrcHighlightManager.FilterHighlight(model);
                        SrcHighlightManager.FinderHighlight(model);

                        model.LineNumber = _displaySource.Count;
                        _displaySource.Add(model);

                    }
                }
                SourceCollectionChanged?.Invoke(this);
            }

        }

        public void ClearSource()
        {
            lock (stateLockObject)
            {
                LogManager.ResetLogInfos();
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
        }

        public int DisplayItemsCount()
        {
            return DisplaySource.Count;
        }

        public int RawItemsCount()
        {
            return RawSource.Count;
        }

        public void RemoveItem(ILogWatcherElements model)
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
            LogManager.UpdateLogParser(runThreadConfig);
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
        private CancellationTokenSource? SourceFilterCancellationTokenCache { get; set; }
        private AsyncTask? FilterTaskCache { get; set; }

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


            var filterLst = new List<ILogWatcherElements>();

            lock (RawSource.ThreadSafeLock)
            {
                foreach (var item in RawSource)
                {
                    // Reset line number id (thứ tự của line hiển thị trên logwatcher)
                    item.LineNumber = -1;

                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    if (SrcFilterManager.Filter(item))
                    {
                        SrcHighlightManager.FilterHighlight(item);
                        SrcHighlightManager.FinderHighlight(item);

                        item.LineNumber = filterLst.Count;

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
                    SourceFilteredAndDisplayed?.Invoke(this);
                }
            }
        }

        #endregion

        #region Highlight area
        private CancellationTokenSource? SourceHighlightCancellationTokenCache { get; set; }
        private AsyncTask? HighlightTaskCache { get; set; }

        private void OnHighlightConditionChanged(object sender, ConditionChangedEventArgs e)
        {
            if (HighlightTaskCache != null)
            {
                if (!HighlightTaskCache.IsCompleted)
                {
                    AsyncTask.CancelAsyncExecute(HighlightTaskCache);
                }
            }

            SourceHighlightCancellationTokenCache = new CancellationTokenSource();
            HighlightTaskCache = new AsyncTask(OnDoHighlight
                   , null
                   , OnFinishHighlightSource
                   , 0
                   , SourceHighlightCancellationTokenCache);
            AsyncTask.CancelableAsyncExecute(HighlightTaskCache);
        }

        private void OnFinishHighlightSource(AsyncTaskResult result)
        {

        }

        private async Task<AsyncTaskResult> OnDoHighlight(CancellationToken token)
        {
            var result = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);



            lock (DisplaySource.ThreadSafeLock)
            {
                foreach (var item in DisplaySource)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    SrcHighlightManager.FinderHighlight(item);
                }
            }

            result.MesResult = MessageAsyncTaskResult.Done;
            return result;
        }
        #endregion

        /// <summary>
        /// This area for method delete of
        /// LogWatcher elements
        /// Including:
        ///     +) DeleteTaskCache: Task run for delete log view and expandable
        ///     +) SourceDeleteCancellationTokenCache: Cancellation token for DeleteTaskCache
        ///     +) SelectedItemNotifierCache: Notifier when source change
        ///     
        /// Instead of using Remove(element) method of a IList
        /// this method will re-create the IList
        /// this save more time when using Remove methods
        /// After calculating: if use Remove: O(n^2)
        /// but re-create only: O(n)
        /// </summary>
        #region Delete display log area
        private CancellationTokenSource? SourceDeleteCancellationTokenCache { get; set; }
        private AsyncTask? DeleteTaskCache { get; set; }
        private INotifyCollectionChanged? SelectedItemNotifierCache { get; set; }
        public void DeleteSeletedLogLine(IEnumerable<LogWatcherItemViewModel> selectedItem, INotifyCollectionChanged selectedItemNotifier)
        {
            if (selectedItemNotifier == null)
            {
                return;
            }
            else
            {
                SelectedItemNotifierCache = selectedItemNotifier;
                selectedItemNotifier.CollectionChanged -= OnSelectedItemChanged;
                selectedItemNotifier.CollectionChanged += OnSelectedItemChanged;
            }

            if (DeleteTaskCache != null)
            {
                if (!DeleteTaskCache.IsCompleted)
                {
                    AsyncTask.CancelAsyncExecute(DeleteTaskCache);
                }
            }

            SourceDeleteCancellationTokenCache = new CancellationTokenSource();
            DeleteTaskCache = new AsyncTask(DeleteSelectedItems
                   , null
                   , OnFinishDeleteSource
                   , 0
                   , SourceDeleteCancellationTokenCache);
            AsyncTask.ParamAsyncExecute(DeleteTaskCache
                , param: selectedItem
                , isAsyncCallback: true);

        }

        private void OnSelectedItemChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
#if DEBUG
            Console.WriteLine("Selected collection changed!");
#endif
            if (DeleteTaskCache != null)
            {
                if (!DeleteTaskCache.IsCompleted)
                {

                    AsyncTask.CancelAsyncExecute(DeleteTaskCache);
                    if (SelectedItemNotifierCache != null)
                    {
                        SelectedItemNotifierCache.CollectionChanged -= OnSelectedItemChanged;
                        SelectedItemNotifierCache = null;
                    }
#if DEBUG
                    Console.WriteLine("Selected collection changed during deleting, abort the process!");
#endif
                }
            }

            if (SelectedItemNotifierCache != null)
            {
                SelectedItemNotifierCache.CollectionChanged -= OnSelectedItemChanged;
                SelectedItemNotifierCache = null;
            }
        }

        private async Task<AsyncTaskResult> DeleteSelectedItems(object? data, CancellationToken token)
        {
            var result = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);
            var selectedItem = data as IEnumerable<LogWatcherItemViewModel>;
            GetNewExpandableList(selectedItem, token, result);

            return result;
        }

        private void GetNewExpandableList(
            IEnumerable<LogWatcherItemViewModel> selectedItem
            , CancellationToken token
            , AsyncTaskResult result)
        {
#if DEBUG
            var watch = Stopwatch.StartNew();
#endif
            var items = selectedItem;
            var itemsCount = 0;
            lock (DisplaySource.ThreadSafeLock)
            {
                items = selectedItem
                   .OrderBy((e) =>
                   {
                       itemsCount++;
                       return e.LineNumber;
                   })
                   .ToArray();

            }

#if DEBUG
            watch.Stop();
            Console.WriteLine("Order time = " + watch.ElapsedMilliseconds + "(ms)");
            Logger.D("Order time = " + watch.ElapsedMilliseconds + "(ms)");
#endif

            LogWatcherItemViewModel cur = null;
            var newExpandableLst = new List<LWI_ExpandableViewModel>();
            var preCount = 0;

            var curExpandbleView = new LWI_ExpandableViewModel(
                ViewModelManager.Current.LogGuardViewModel);
            curExpandbleView.ExpandButtonCommand = GetExpandButtonCommand(DisplaySource);
            curExpandbleView.DeleteButtonCommand = GetDeleteButtonCommand(DisplaySource);

            // In case user selected only one item, don't need to re-create all the
            // display source, only need to replace deleted item with expandable view
            if (itemsCount == 1)
            {
#if DEBUG
                watch = Stopwatch.StartNew();
#endif

                cur = items.ElementAt(0);
                curExpandbleView.Childs.Add(cur);
                curExpandbleView.LineNumber = cur.LineNumber;
                lock (DisplaySource.ThreadSafeLock)
                {
                    DisplaySource[cur.LineNumber] = curExpandbleView;
                }
                result.MesResult = MessageAsyncTaskResult.Finished;
                result.Result = null;
#if DEBUG
                watch.Stop();
                Console.WriteLine("Delete one item time = " + watch.ElapsedMilliseconds + "(ms)");
                Logger.D("Delete one item time = " + watch.ElapsedMilliseconds + "(ms)");
#endif
                return;
            }



#if DEBUG
            watch = Stopwatch.StartNew();
#endif

            var isShouldRecreatDisplaySource = true;
            foreach (var item in items)
            {
                if (token.IsCancellationRequested)
                {
#if DEBUG
                    watch.Stop();
                    Console.WriteLine("Index process is aborted: time = " + watch.ElapsedMilliseconds + "(ms)");
                    Logger.D("Index process is aborted: time = " + watch.ElapsedMilliseconds + "(ms)");
#endif
                    throw new OperationCanceledException();
                }

                if (cur == null)
                {
                    cur = item;
                    curExpandbleView.Childs.Add(cur);
                    curExpandbleView.LineNumber = cur.LineNumber;
                }
                else if (item.LineNumber == cur.LineNumber + 1)
                {
                    curExpandbleView.Childs.Add(item);
                    cur = item;
                    isShouldRecreatDisplaySource = false;
                }
                else
                {
                    newExpandableLst.Add(curExpandbleView);

                    preCount += curExpandbleView.Childs.Count - 1;

                    curExpandbleView = new LWI_ExpandableViewModel(
                        ViewModelManager.Current.LogGuardViewModel);
                    curExpandbleView.ExpandButtonCommand = GetExpandButtonCommand(DisplaySource);
                    curExpandbleView.DeleteButtonCommand = GetDeleteButtonCommand(DisplaySource);

                    curExpandbleView.LineNumber = item.LineNumber - preCount;
                    curExpandbleView.Childs.Add(item);
                    cur = item;
                }
            }
            newExpandableLst.Add(curExpandbleView);
#if DEBUG
            watch.Stop();
            Console.WriteLine("Index time = " + watch.ElapsedMilliseconds + "(ms)");
#endif

            // In case user selected many item, but those items not adjacent
            // For example: 2 items A & B
            //      A [index = 0] & B [index = 1]
            //          => Should recreate display source due to A is adjacent with B
            //
            //      A [index = 0] & B [index = 2]
            //          => Should not recreate display source due to A is not adjacent with B
            if (isShouldRecreatDisplaySource &&
                itemsCount > 1)
            {
                lock (DisplaySource.ThreadSafeLock)
                {
                    foreach (var item in newExpandableLst)
                    {
                        DisplaySource[item.LineNumber] = item;
                    }
                }

                result.MesResult = MessageAsyncTaskResult.Finished;
                result.Result = null;
                return;
            }

#if DEBUG
            watch = Stopwatch.StartNew();
#endif
            var newDisplayList = new List<ILogWatcherElements>();
            lock (DisplaySource.ThreadSafeLock)
            {
                int oldDisplaySourceCount = DisplaySource.Count;
                int j = 0;
                int expandViewCount = newExpandableLst.Count;
                for (int i = 0; i < oldDisplaySourceCount; i++)
                {
                    if (j < expandViewCount
                        && DisplaySource[i].LineNumber == newExpandableLst[j].LineNumber)
                    {
                        var newItem = newExpandableLst[j];
                        newItem.LineNumber = newDisplayList.Count;

                        newDisplayList.Add(newItem);
                        i += newExpandableLst[j].Childs.Count - 1;
                        j++;
                    }
                    else
                    {
                        var newItem = DisplaySource[i];
                        newItem.LineNumber = newDisplayList.Count;

                        newDisplayList.Add(newItem);
                    }
                }
            }


#if DEBUG
            watch.Stop();
            Console.WriteLine("Create new = " + watch.ElapsedMilliseconds + "(ms)");
#endif
            result.Result = newDisplayList;
            result.MesResult = MessageAsyncTaskResult.Done;
        }

        private void OnFinishDeleteSource(object data, AsyncTaskResult obj)
        {
            if (obj.MesResult == MessageAsyncTaskResult.Done)
            {
                lock (DisplaySource.ThreadSafeLock)
                {
                    _displaySource.AddNewRange((IEnumerable<LogWatcherItemViewModel>)obj.Result);
                    SourceCollectionChanged?.Invoke(this);
                }
            }
        }

        private BaseDotNetCommandImpl GetExpandButtonCommand(RangeObservableCollection<ILogWatcherElements> displaySource)
        {
            return new BaseDotNetCommandImpl((s) =>
            {
                var context = (s as LogWatcherItem)?.DataContext as LogWatcherItemViewModel;
                if (context != null)
                {
                    RedoDeleteLogLine(context);
                }
            });
        }

        private BaseDotNetCommandImpl GetDeleteButtonCommand(RangeObservableCollection<ILogWatcherElements> displaySource)
        {
            return new BaseDotNetCommandImpl((s) =>
            {
                var context = (s as LogWatcherItem)?.DataContext as LogWatcherItemViewModel;
                if (context != null)
                {
                    CompletelyDeleteExpandableLogLine(context);
                }
            });
        }

        #endregion

        /// <summary>
        /// This area for redo method delete of
        /// LogWatcher elements
        /// Including:
        ///     +) RedoDeleteTaskCache: Task run for re-add log view type from expandable parents view
        ///     +) SourceRedoDeleteCancellationTokenCache: Cancellation token for RedoDeleteTaskCache
        /// </summary>
        #region Redo delete source
        private CancellationTokenSource? SourceRedoDeleteCancellationTokenCache { get; set; }
        private AsyncTask? RedoDeleteTaskCache { get; set; }

        public void RedoDeleteLogLine(LogWatcherItemViewModel expandableViewModel)
        {
            if (expandableViewModel == null
                || expandableViewModel.ViewType != ElementViewType.ExpandableRowView)
            {
                return;
            }

            if (RedoDeleteTaskCache != null)
            {
                if (!RedoDeleteTaskCache.IsCompleted)
                {
                    AsyncTask.CancelAsyncExecute(RedoDeleteTaskCache);
                }
            }

            SourceRedoDeleteCancellationTokenCache = new CancellationTokenSource();
            RedoDeleteTaskCache = new AsyncTask(OnRedoDeleteSource
                  , null
                  , OnFinishRedoDeleteSource
                  , 0
                  , SourceRedoDeleteCancellationTokenCache);

            AsyncTask.ParamAsyncExecute(RedoDeleteTaskCache
                , param: expandableViewModel
                , isAsyncCallback: true);

        }

        private void OnFinishRedoDeleteSource(object data, AsyncTaskResult obj)
        {
            if (obj.MesResult == MessageAsyncTaskResult.Done)
            {
                lock (DisplaySource.ThreadSafeLock)
                {
                    _displaySource.AddNewRange((IEnumerable<LogWatcherItemViewModel>)obj.Result);
                    SourceCollectionChanged?.Invoke(this);
                }
            }
        }

        private async Task<AsyncTaskResult> OnRedoDeleteSource(object data, CancellationToken token)
        {
            var vmodel = data as LWI_ExpandableViewModel;
            var newExpandedList = new List<ILogWatcherElements>();
            var result = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);
            lock (DisplaySource.ThreadSafeLock)
            {
                int oldDisplaySourceCount = DisplaySource.Count;
                for (int i = 0; i < oldDisplaySourceCount; i++)
                {
                    if (DisplaySource[i].LineNumber == vmodel.LineNumber)
                    {
                        foreach (var child in vmodel.Childs)
                        {
                            var newItem = child as LogWatcherItemViewModel;
                            newItem.LineNumber = newExpandedList.Count;

                            newExpandedList.Add(newItem);
                        }
                    }
                    else
                    {
                        var newItem = DisplaySource[i];
                        newItem.LineNumber = newExpandedList.Count;

                        newExpandedList.Add(newItem);
                    }
                }
            }
            result.MesResult = MessageAsyncTaskResult.Done;
            result.Result = newExpandedList;

            return result;
        }

        #endregion

        /// <summary>
        /// This area for completely delete method of
        /// LogWatcher elements
        /// Including:
        ///     +) CompletelyDeleteTaskCache: Task run for completely delete log expandable view from log watcher
        ///     +) SourceCompletelyDeleteCancellationTokenCache: Cancellation token for CompletelyDeleteTaskCache
        /// </summary>
        #region Completely delete expandble log view
        private CancellationTokenSource? SourceCompletelyDeleteCancellationTokenCache { get; set; }
        private AsyncTask? CompletelyDeleteTaskCache { get; set; }

        public void CompletelyDeleteExpandableLogLine(LogWatcherItemViewModel expandableViewModel)
        {
            if (expandableViewModel == null
                || expandableViewModel.ViewType != ElementViewType.ExpandableRowView)
            {
                return;
            }

            if (CompletelyDeleteTaskCache != null)
            {
                if (!CompletelyDeleteTaskCache.IsCompleted)
                {
                    AsyncTask.CancelAsyncExecute(CompletelyDeleteTaskCache);
                }
            }

            SourceCompletelyDeleteCancellationTokenCache = new CancellationTokenSource();
            CompletelyDeleteTaskCache = new AsyncTask(OnCompletelyDeleteSource
                  , null
                  , OnFinishCompletelyDeleteSource
                  , 0
                  , SourceCompletelyDeleteCancellationTokenCache);

            AsyncTask.ParamAsyncExecute(CompletelyDeleteTaskCache
                , param: expandableViewModel
                , isAsyncCallback: true);

        }

        private void OnFinishCompletelyDeleteSource(object data, AsyncTaskResult obj)
        {
            if (obj.MesResult == MessageAsyncTaskResult.Done)
            {
                lock (DisplaySource.ThreadSafeLock)
                {
                    _displaySource.AddNewRange((IEnumerable<LogWatcherItemViewModel>)obj.Result);
                    SourceCollectionChanged?.Invoke(this);
                }
            }
        }

        private async Task<AsyncTaskResult> OnCompletelyDeleteSource(object data, CancellationToken token)
        {
            var vmodel = data as LogWatcherItemViewModel;
            var newExpandedList = new List<ILogWatcherElements>();
            var result = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);
            lock (DisplaySource.ThreadSafeLock)
            {
                int oldDisplaySourceCount = DisplaySource.Count;
                for (int i = 0; i < oldDisplaySourceCount; i++)
                {
                    if (DisplaySource[i].LineNumber == vmodel.LineNumber)
                    {

                    }
                    else
                    {
                        var newItem = DisplaySource[i];
                        newItem.LineNumber = newExpandedList.Count;

                        newExpandedList.Add(newItem);
                    }
                }
            }
            result.MesResult = MessageAsyncTaskResult.Done;
            result.Result = newExpandedList;

            return result;
        }

        #endregion


    }
}