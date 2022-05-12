using cyber_base.implement.utils;
using log_guard.@base.flow.highlight;
using log_guard.@base.flow.source_filter;
using log_guard.@base.watcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.flow
{
    internal interface ISourceManager
    {
        event SourceCollectionChangedHandler SourceCollectionChanged;
        event SourceFilteredAndDisplayedHandler SourceFilteredAndDisplayed;

        List<ISourceHolder> SourceHolders { get;}
        RangeObservableCollection<ILogWatcherElements> RawSource { get; }
        RangeObservableCollection<ILogWatcherElements> DisplaySource { get; }
        ILogInfoManager LogInfoManager { get; }
        ISourceFilterManager SourceFilterManager { get; }
        ISourceHighlightManager SourceHighlightManager { get; }
        RangeObservableCollection<string> RawLog { get; }

        int RawItemsCount();
        int DisplayItemsCount();
        int ErrorItemsCount();
        int InfoItemsCount();
        int DebugItemsCount();
        int WarningItemsCount();
        int FatalItemsCount();
        int VerboseItemsCount();

        void AddSourceHolder(ISourceHolder holder);
        void RemoveSourceHolder(ISourceHolder holder);
        void AddItem(ILogWatcherElements model);
        void AddItem(string line);
        void RemoveItem(ILogWatcherElements model);
        void ClearSource();
        void UpdateLogParser(IRunThreadConfig runThreadConfig);
    }

    public delegate void SourceCollectionChangedHandler(object sender);
    public delegate void SourceFilteredAndDisplayedHandler(object sender);
}
