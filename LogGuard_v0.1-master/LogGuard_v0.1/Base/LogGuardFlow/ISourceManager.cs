﻿using LogGuard_v0._1.Utils;
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

        int RawItemsCount();
        int DisplayItemsCount();
        void AddSourceHolder(ISourceHolder holder);
        void AddItem(LogWatcherItemViewModel model);
        void RemoveItem(LogWatcherItemViewModel model);
        void ClearSource();
    }

    public delegate void SourceCollectionChangedHandler(object sender);
}