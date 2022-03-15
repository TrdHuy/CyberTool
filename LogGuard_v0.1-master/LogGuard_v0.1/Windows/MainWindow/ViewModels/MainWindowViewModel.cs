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

        }

        private void OnPageSourceChange(PageVO obj)
        {
            Invalidate("CurrentPageSource");
        }
    }
}
