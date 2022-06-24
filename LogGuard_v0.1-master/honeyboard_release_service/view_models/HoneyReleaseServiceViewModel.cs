using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.implement.command;
using cyber_base.view_model;
using honeyboard_release_service.definitions;
using honeyboard_release_service.view_models.calendar_notebook;
using honeyboard_release_service.view_models.command;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace honeyboard_release_service.view_models
{
    internal class HoneyReleaseServiceViewModel : BaseViewModel
    {
        private CalendarNoteBookViewModel _calendarNoteBookContext;
        private Visibility _calendarNoteBookVisibility;
        private Visibility _logMonitorVisibility;
        private BaseSwPublisherCommandVM _commandVM;

        [Bindable(true)]
        public CommandExecuterModel CalendarButtonCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterModel LogMonitorButtonCommand { get; set; }

        [Bindable(true)]
        public CalendarNoteBookViewModel CalendarNoteBookContext
        {
            get
            {
                return _calendarNoteBookContext;
            }
            set
            {
                _calendarNoteBookContext = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public Visibility LogMonitorVisibility
        {
            get
            {
                return _logMonitorVisibility;
            }
            set
            {
                _logMonitorVisibility = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public Visibility CalendarNoteBookVisibility
        {
            get
            {
                return _calendarNoteBookVisibility;
            }
            set
            {
                _calendarNoteBookVisibility = value;
                InvalidateOwn();
            }
        }

        public HoneyReleaseServiceViewModel()
        {
            _calendarNoteBookVisibility = Visibility.Visible;
            _logMonitorVisibility= Visibility.Hidden;
            _calendarNoteBookContext = new CalendarNoteBookViewModel(this);
            _commandVM = new BaseSwPublisherCommandVM(this);
            CalendarButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return _commandVM.GetCommandExecuter(PublisherKeyFeatureTag.KEY_TAG_PRT_SWITCH_CALENDAR_FEATURE
                    , paramaters);
            });

            LogMonitorButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return _commandVM.GetCommandExecuter(PublisherKeyFeatureTag.KEY_TAG_PRT_SWITCH_LOG_MONITOR_FEATURE
                    , paramaters);
            });
        }

        

    }
}
