using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.views.elements.commit_data_grid.@base
{
    public interface ICommitDataGridItemContext
    {
        string CommitId { get; }
        string TaskId { get; }
        string Title { get; }
        string Author { get; }
        string DateTime { get; }

        List<IMatchedWord>? CommitIdHighlightSource { get; set; }
        List<IMatchedWord>? TaskIdHighlightSource { get; set; }
        List<IMatchedWord>? TitleHighlightSource { get; set; }
        List<IMatchedWord>? AuthorHighlightSource { get; set; }
        List<IMatchedWord>? DateTimeHighlightSource { get; set; }
    }

}
