using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.BaseWindow.Models;
using LogGuard_v0._1.Windows.BaseWindow.Utils;
using LogGuard_v0._1.Windows.MainWindow.Utils;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private MSW_PageController _pageHost = MSW_PageController.Current;
        private PageSourceWatcher _pageSourceWatcher;
        private int _selectedPageIndex = 1;
        private PageSource[] _lstItemPageSourceMap = new PageSource[]
        {
            PageSource.UNDER_CONSTRUCTION_PAGE,
            PageSource.LOG_GUARD_PAGE,
            PageSource.UNDER_CONSTRUCTION_PAGE,
            PageSource.UNDER_CONSTRUCTION_PAGE,
            PageSource.UNDER_CONSTRUCTION_PAGE,
            PageSource.UNDER_CONSTRUCTION_PAGE,
        };

        [Bindable(true)]
        public int SelectedPageIndex {
            get
            {
                return _selectedPageIndex;
            }
            set
            {
                _selectedPageIndex = value;
                _pageHost.UpdateCurrentPageSource(_lstItemPageSourceMap[value]);
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public Uri CurrentPageSource
        {
            get
            {
                return _pageHost.CurrentPageOV.PageUri;
            }
            set
            {
                _pageHost.UpdatePageOVUri(value);
                InvalidateOwn();
            }
        }

        public MainWindowViewModel()
        {
            _pageSourceWatcher = new PageSourceWatcher(OnPageSourceChange);
            _pageHost.Subcribe(_pageSourceWatcher);
            _pageHost.UpdateCurrentPageSource(_lstItemPageSourceMap[SelectedPageIndex]);
        }

        private void OnPageSourceChange(PageVO obj)
        {
            Invalidate("CurrentPageSource");
        }
    }
}
