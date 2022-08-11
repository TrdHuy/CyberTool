using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Base.ViewModel.ViewModelHelper;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.ViewModels
{
    public class ViewModelHelper
    {
        private static ViewModelHelper _instance;
        public event OnLogGuardPageViewModelGeneratedHandler LogGuardPageViewModelGenerated;
        public event OnLogManagerUCViewModelGeneratedHandler LogManagerUCViewModelGenerated;
        public event OnAdvanceFilterUCViewModelGeneratedHandler AdvanceFilterUCViewModelGenerated;

        public LogGuardPageViewModel LogGuardPageViewModel { get; private set; }
        public LogManagerUCViewModel LogManagerUCViewModel { get; private set; }
        public AdvanceFilterUCViewModel AdvanceFilterUCViewModel { get; private set; }

        public void Init()
        {
            VMManagerMarkupExtension.DataContextGenerated -= OnDataContextGenerated;
            VMManagerMarkupExtension.DataContextGenerated += OnDataContextGenerated;
            VMManagerMarkupExtension.DataContextDestroyed -= OnDataContextDestroyed;
            VMManagerMarkupExtension.DataContextDestroyed += OnDataContextDestroyed;
        }

        private void OnDataContextDestroyed(object sender, DataContextDestroyedArgs e)
        {
            switch (e.DataContext)
            {
                case LogGuardPageViewModel vm:
                    LogGuardPageViewModel = null;
                    break;
                case LogManagerUCViewModel vm:
                    LogManagerUCViewModel = null;
                    break;
                case AdvanceFilterUCViewModel vm:
                    AdvanceFilterUCViewModel = null;
                    break;
            }
        }

        private void OnDataContextGenerated(object sender, DataContextGeneratedArgs e)
        {
            switch (e.DataContext)
            {
                case LogGuardPageViewModel vm:
                    LogGuardPageViewModel = vm;
                    LogGuardPageViewModelGenerated?.Invoke(this, vm);
                    break;
                case LogManagerUCViewModel vm:
                    LogManagerUCViewModel = vm;
                    LogManagerUCViewModelGenerated?.Invoke(this, vm);
                    break;
                case AdvanceFilterUCViewModel vm:
                    AdvanceFilterUCViewModel = vm;
                    AdvanceFilterUCViewModelGenerated?.Invoke(this, vm);
                    break;
            }
        }

        public static ViewModelHelper Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelHelper();
                }
                return _instance;
            }
        }

    }

    public delegate void OnLogManagerUCViewModelGeneratedHandler(object sender, LogManagerUCViewModel vm);
    public delegate void OnLogGuardPageViewModelGeneratedHandler(object sender, LogGuardPageViewModel vm);
    public delegate void OnAdvanceFilterUCViewModelGeneratedHandler(object sender, AdvanceFilterUCViewModel vm);
}
