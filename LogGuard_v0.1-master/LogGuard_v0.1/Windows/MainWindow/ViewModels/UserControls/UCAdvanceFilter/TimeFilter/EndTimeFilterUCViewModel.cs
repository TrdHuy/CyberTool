using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.UserControls.UCAdvanceFilter
{
    public class EndTimeFilterUCViewModel : TimeFilterUCViewModel
    {
        public EndTimeFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
        }

        public override bool Filter(object obj)
        {
            var data = obj as LogWatcherItemViewModel;
            if (!IsFilterEnable || FilterContent == "")
            {
                return true;
            }

            if (data != null)
            {
                return data.LogDateTime <= CurrentFilterTime;
            }

            return true;
        }
    }
}
