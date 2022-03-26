using LogGuard_v0._1.Base.AsyncTask;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.FileHelper;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using LogGuard_v0._1.Implement.LogGuardFlow.StateController;
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
        public int SelectedPageIndex
        {
            get
            {
                return _selectedPageIndex;
            }
            set
            {
                if (IsShouldChangePage(_selectedPageIndex, value))
                {
                    _selectedPageIndex = value;
                    _pageHost.UpdateCurrentPageSource(_lstItemPageSourceMap[value]);
                    InvalidateOwn();
                }
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

        private bool IsShouldChangePage(int oleValue, int newValue)
        {
            if (_lstItemPageSourceMap[oleValue] == PageSource.LOG_GUARD_PAGE)
            {
                if (SourceManagerImpl.Current.RawItemsCount() > 0)
                {
                    var mesResult = App.Current.ShowEscapeCaptureLogWarningBox();
                    if (mesResult == MessageWindow.LogGuardMesBoxResult.No)
                    {
                        return false;
                    }
                    else if (mesResult == MessageWindow.LogGuardMesBoxResult.Continue)
                    {
                        StateControllerImpl.Current.Stop();
                        return true;
                    }
                    else if (mesResult == MessageWindow.LogGuardMesBoxResult.Yes)
                    {
                        StateControllerImpl.Current.Stop();
                        var savePath = App.Current.OpenSaveFileDialogWindow();

                        var resMes = App.Current.OpenWaitingTaskBox("Saving!"
                            , "Please wait!"
                            , async (param, token) =>
                                {
                                    var result = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);
                                    FileHelperImpl.Current.ExportLinesToFile(savePath, SourceManagerImpl.Current.RawLog);
                                    await Task.Delay(1000);
                                    return result;
                                }
                            , null
                            , null
                            , 3000);

                        if (resMes == WaitingWindow.LogGuardWaitingBoxResult.cancel)
                        {
                            FileHelperImpl.Current.DeleteLogFile(savePath);
                        }
                    }
                }
            }

            return true;
        }

    }
}
