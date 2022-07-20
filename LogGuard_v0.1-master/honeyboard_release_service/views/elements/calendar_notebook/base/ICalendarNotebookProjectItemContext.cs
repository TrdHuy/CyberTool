using cyber_base.utils;
using honeyboard_release_service.views.elements.calendar_notebook.data_structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.views.elements.calendar_notebook.@base
{
    public interface ICalendarNotebookProjectItemContext : IBeginable, INotifyPropertyChanged
    {
        CalendarNotebookItemCollection<ICalendarNotebookCommitItemContext> CommitSource { get; }

        [Bindable(true)]
        string ProjectName { get; set; }

        [Bindable(true)]
        object? ItemView { get; set; }

        [Bindable(true)]
        bool IsLoadingData { get; set; }
    }
}
