using LogGuard_v0._1.Base.Log;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.LogGuardFlow
{
    public interface ISourceManager
    {
        event SourceCollectionChangedHandler SourceCollectionChanged;

        List<ISourceHolder> SourceHolders { get;}
        RangeObservableCollection<LogWatcherItemViewModel> RawSource { get; }
        RangeObservableCollection<LogWatcherItemViewModel> DisplaySource { get; }
        ILogInfoManager LogInfoManager { get; }

        int RawItemsCount();
        int DisplayItemsCount();
        int ErrorItemsCount();
        int InfoItemsCount();
        int DebugItemsCount();
        int WarningItemsCount();
        int FatalItemsCount();
        int VerboseItemsCount();

        void AddSourceHolder(ISourceHolder holder);
        void AddItem(LogWatcherItemViewModel model);
        void RemoveItem(LogWatcherItemViewModel model);
        void ClearSource();
    }

    public delegate void SourceCollectionChangedHandler(object sender);
}
