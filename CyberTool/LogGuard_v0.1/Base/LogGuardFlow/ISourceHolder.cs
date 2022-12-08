using System;
using System.Collections.Generic;
using System.Linq;
using LogGuard_v0._1.Utils;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;

namespace LogGuard_v0._1.Base.LogGuardFlow
{
    public interface ISourceHolder
    {
        RangeObservableCollection<LogWatcherItemViewModel> ItemsSource { get; set; }

        int ItemCount { get; set; }
    }
}
