using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.implement.command;
using cyber_base.view_model;
using progtroll.definitions;
using progtroll.view_models.calendar_notebook;
using progtroll.view_models.command;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace progtroll.view_models
{
    internal class HoneyReleaseServiceViewModel : BaseViewModel
    {
        private Visibility _calendarNoteBookVisibility;
        private Visibility _logMonitorVisibility;
        private BaseSwPublisherCommandVM _commandVM;

        [Bindable(true)]
        public CommandExecuterImpl CalendarButtonCommand { get; set; }

        [Bindable(true)]
        public CommandExecuterImpl LogMonitorButtonCommand { get; set; }

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
            _commandVM = new BaseSwPublisherCommandVM(this);
            CalendarButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return _commandVM.GetCommandExecuter(PublisherKeyFeatureTag.KEY_TAG_PRT_SWITCH_CALENDAR_FEATURE
                    , paramaters);
            });

            LogMonitorButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return _commandVM.GetCommandExecuter(PublisherKeyFeatureTag.KEY_TAG_PRT_SWITCH_LOG_MONITOR_FEATURE
                    , paramaters);
            });
        }

        

    }
}
