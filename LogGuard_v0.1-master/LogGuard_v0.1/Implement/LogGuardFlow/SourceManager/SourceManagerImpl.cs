using LogGuard_v0._1.Base.LogGuardFlow;
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

        public List<ISourceHolder> SourceHolders { get => _sourceHolder; }
        public RangeObservableCollection<LogWatcherItemViewModel> RawSource => _rawSource;
        public RangeObservableCollection<LogWatcherItemViewModel> DisplaySource => _displaySource;


        public event SourceCollectionChangedHandler SourceCollectionChanged;

        private SourceManagerImpl()
        {
            _rawSource = new RangeObservableCollection<LogWatcherItemViewModel>();
            _displaySource = new RangeObservableCollection<LogWatcherItemViewModel>();
            _sourceHolder = new List<ISourceHolder>();
        }

        public void AddItem(LogWatcherItemViewModel model)
        {
            _rawSource.Add(model);
            _displaySource.Add(model);
            SourceCollectionChanged?.Invoke(this);
        }

        public void ClearSource()
        {
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
