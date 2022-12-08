using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceFilterManager;
using LogGuard_v0._1.Implement.UIEventHandler;
using System;
using System.ComponentModel;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter.TimeFilter
{
    public abstract class TimeFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        private bool _isCalendarOpen;

        [Bindable(true)]
        public bool IsCalendarOpen
        {
            get
            {
                return _isCalendarOpen;
            }
            set
            {

                _isCalendarOpen = value;
                InvalidateOwn();
            }
        }

        public override bool IsUseFilterEngine => false;

        protected DateTime CurrentFilterTime { get; set; }
        public TimeFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            FilterLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                IsCalendarOpen = !IsCalendarOpen;
                return null;
            });
        }

        protected override void OnFilterContentChanged(string value)
        {
            try
            {
                CurrentFilterTime = DateTime.ParseExact(FilterContent
                               , "dd-MM-yyyy HH:mm:ss:ffffff"
                               , System.Globalization.CultureInfo.CurrentCulture);
            }
            catch
            {

            }

            NotifyFilterContentChanged(value);
        }

        protected override void OnFilterEnableChanged(bool value)
        {
            if (FilterContent != "")
            {
                SourceFilterManagerImpl.Current.NotifyFilterPropertyChanged(this, value);
            }
        }

    }
}
