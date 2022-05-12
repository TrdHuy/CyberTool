using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using cyber_base.implement.utils;
using log_guard.@base.watcher;

namespace log_guard.@base.flow
{
    internal interface ISourceHolder
    {
        RangeObservableCollection<ILogWatcherElements> ItemsSource { get; set; }

        int ItemCount { get; set; }
    }
}
