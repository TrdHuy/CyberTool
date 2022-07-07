using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.views.elements.calendar_notebook.@base
{
    public interface ICalendarNotebookCommitItemContext
    {
        DateTime TimeId { get; }

        string MainContent { get; }

        ICalendarNotebookProjectItemContext Project { get; }
    }
}
