using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.UserControls.UCAdvanceFilter
{
    public class TagFilterUCViewModel : BaseViewModel, ISourceFilter
    {
        private string _tagFilterContent = "";

        [Bindable(true)]
        public string TagFilterContent
        {
            get
            {
                return _tagFilterContent;
            }
            set
            {
                _tagFilterContent = value;
                var watcher = Stopwatch.StartNew();
                SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, value);
                watcher.Stop();
                Console.WriteLine("Time validate filter = " + watcher.ElapsedMilliseconds);
                InvalidateOwn();
            }
        }

        public TagFilterUCViewModel(BaseViewModel parent) : base(parent) { }

        public bool Filter(object obj)
        {
            var itemVM = obj as LogWatcherItemViewModel;
            if (itemVM != null)
            {
                return itemVM
                    .Tag
                    .ToString()
                    .IndexOf(TagFilterContent, StringComparison.InvariantCultureIgnoreCase) != -1;
            }
            return true;
        }
    }
}
