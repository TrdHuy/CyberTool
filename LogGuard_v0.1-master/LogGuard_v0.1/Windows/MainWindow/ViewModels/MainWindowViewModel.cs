﻿using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceManager;
using LogGuard_v0._1.Utils;
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
    public class MainWindowViewModel : BaseViewModel, ISourceHolder
    {

        private RangeObservableCollection<LogWatcherItemViewModel> _logItemVMs;
        private int _logCount;
        private LogGuardState _currentLogGuardState = LogGuardState.NONE;

        [Bindable(true)]
        public MSW_LogWatcherControlButtonCommandVM CommandViewModel { get; set; }

        [Bindable(true)]
        public RangeObservableCollection<LogWatcherItemViewModel> LogItemVMs
        {
            get
            {
                return _logItemVMs;
            }
            set
            {
                _logItemVMs = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public LogGuardState CurrentLogGuardState
        {
            get
            {
                return _currentLogGuardState;
            }
            set
            {
                _currentLogGuardState = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public RangeObservableCollection<LogWatcherItemViewModel> ItemsSource
        {
            get
            {
                return _logItemVMs;
            }
            set
            {
                _logItemVMs = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public int ItemCount
        {
            get
            {
                return _logCount;
            }
            set
            {
                _logCount = value;
                InvalidateOwn();
            }
        }

        public MainWindowViewModel()
        {
            CommandViewModel = new MSW_LogWatcherControlButtonCommandVM(this);
            LogItemVMs = new RangeObservableCollection<LogWatcherItemViewModel>();
            SourceManagerImpl.Current.AddSourceHolder(this);

        }
    }
}
