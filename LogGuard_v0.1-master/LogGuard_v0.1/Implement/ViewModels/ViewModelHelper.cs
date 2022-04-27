using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Base.ViewModel.ViewModelHelper;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages;
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
        public LogGuardPageViewModel LogGuardPageViewModel { get; private set; }
        public LogManagerUCViewModel LogManagerUCViewModel { get; private set; }

        public void Init()
        {
            VMManagerMarkupExtension.DataContextGenerated -= OnDataContextGenerated;
            VMManagerMarkupExtension.DataContextGenerated += OnDataContextGenerated;
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
}
