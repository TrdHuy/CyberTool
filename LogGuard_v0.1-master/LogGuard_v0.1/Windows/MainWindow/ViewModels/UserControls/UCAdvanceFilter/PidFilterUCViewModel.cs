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
    public class PidFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        [Bindable(true)]
        public CommandExecuterModel PidFilterLeftClickCommand { get; set; }


        protected override bool IsUseFilterEngine => false;

        public PidFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            PidFilterLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsFilterEnable = !IsFilterEnable;
                return null;
            });

            UpdateHelperContent();
        }

        public override bool Filter(object obj)
        {
            var data = obj as LogWatcherItemViewModel;
            if (IsFilterEnable && data?.Pid != null)
            {
                return data
                    .Tid
                    .ToString()
                    .IndexOf(FilterContent, StringComparison.InvariantCultureIgnoreCase) != -1;
            }

            return true;
        }

    }
}
