using cyber_base.implement.command;
using cyber_base.view_model;
using log_guard.implement.flow.source_filter_manager;
using System;
using System.ComponentModel;

namespace log_guard.view_models.advance_filter.time_filter
{
    internal abstract class TimeFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
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
                SourceFilterManager.Current.NotifyFilterPropertyChanged(this, value);
            }
        }

    }
}
