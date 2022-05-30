using cyber_base.view_model;
using honeyboard_release_service.definitions;
using honeyboard_release_service.extensions;
using honeyboard_release_service.implement.view_helper;
using honeyboard_release_service.implement.view_manager.notebook_header;
using honeyboard_release_service.implement.view_manager.notebook_item;
using honeyboard_release_service.models.VOs;
using honeyboard_release_service.view_models.calendar_notebook.items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace honeyboard_release_service.view_models.calendar_notebook
{
    internal class CalendarNoteBookViewModel : BaseViewModel
    {

        private DateTime _startDateTime;
        private DateTime _endDateTime;
        private PublisherCalendarNotebookViewType _calendarViewType;
        private ObservableCollection<NotebookItemViewModel> _notebookItemContexts;

        [Bindable(true)]
        public ObservableCollection<NotebookItemViewModel> NotebookItemContexts
        {
            get
            {
                return _notebookItemContexts;
            }
            set
            {
                _notebookItemContexts = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public PublisherCalendarNotebookViewType CalendarViewType
        {
            get
            {
                return _calendarViewType;
            }
            set
            {
                _calendarViewType = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public DateTime StartDateTime
        {
            get
            {
                return _startDateTime;
            }
            set
            {
                _startDateTime = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public DateTime EndDateTime
        {
            get
            {
                return _endDateTime;
            }
            set
            {
                _endDateTime = value;
                InvalidateOwn();
            }
        }

        public CalendarNoteBookViewModel(BaseViewModel parent) : base(parent)
        {
            _startDateTime = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            _endDateTime = DateTime.Now.EndOfWeek(DayOfWeek.Sunday);
            _calendarViewType = PublisherCalendarNotebookViewType.DayOfWeek;
            _notebookItemContexts = new ObservableCollection<NotebookItemViewModel>();
            _notebookItemContexts.Add(new NotebookItemViewModel(this, ProjectVO.GetTestData("HoneyBoard")));
            _notebookItemContexts.Add(new NotebookItemViewModel(this, ProjectVO.GetTestData("SmartEye")));
            _notebookItemContexts.Add(new NotebookItemViewModel(this, ProjectVO.GetTestData("Clipboard")));
            _notebookItemContexts.Add(new NotebookItemViewModel(this, ProjectVO.GetTestData("HoneyBoardW")));
        }

        public override void OnViewInstantiated()
        {
            base.OnViewInstantiated();
            CalendarNotebookHeaderViewManager.Current.UpdateColumnNumber(StartDateTime, EndDateTime);
        }

    }
}
