using LogGuard_v0._1.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MSW_LogWatcherControlButtonCommandVM CommandViewModel { get; set; }

        public MainWindowViewModel()
        {
            CommandViewModel = new MSW_LogWatcherControlButtonCommandVM(this);
        }
    }
}
