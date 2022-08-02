using cyber_base.view_model;
using honeyboard_release_service.views.elements.commit_data_grid.@base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models.version_comparator.item
{
    internal class CommitDataGridItemViewModel : BaseViewModel, ICommitDataGridItemContext
    {
        private string _commitId = "";
        private string _taskId = "";
        private string _title = "";
        private string _author = "";
        private string _dateTime = "";
        private List<IMatchedWord>? _commitIdSource;
        private List<IMatchedWord>? _taskIdSource;
        private List<IMatchedWord>? _titleSource;
        private List<IMatchedWord>? _authorSource;
        private List<IMatchedWord>? _dateTimeSource;

        [Bindable(true)]
        public string CommitId => _commitId;

        [Bindable(true)]
        public string TaskId => _taskId;

        [Bindable(true)]
        public string Title => _title;

        [Bindable(true)]
        public string Author => _author;

        [Bindable(true)]
        public string DateTime => _dateTime;

        [Bindable(true)]
        public List<IMatchedWord>? CommitIdHighlightSource
        {
            get => _commitIdSource;
            set
            {
                _commitIdSource = value;
                Invalidate("CommitIdHighlightSource");
            }
        }

        [Bindable(true)]
        public List<IMatchedWord>? TaskIdHighlightSource
        {
            get => _taskIdSource;
            set
            {
                _taskIdSource = value;
                Invalidate("TaskIdHighlightSource");
            }
        }

        [Bindable(true)]
        public List<IMatchedWord>? TitleHighlightSource
        {
            get => _titleSource;
            set
            {
                _titleSource = value;
                Invalidate("TitleHighlightSource");
            }
        }

        [Bindable(true)]
        public List<IMatchedWord>? AuthorHighlightSource
        {
            get => _authorSource;
            set
            {
                _authorSource = value;
                Invalidate("AuthorHighlightSource");
            }
        }

        [Bindable(true)]
        public List<IMatchedWord>? DateTimeHighlightSource
        {
            get => _dateTimeSource;
            set
            {
                _dateTimeSource = value;
                Invalidate("DateTimeHighlightSource");
            }
        }

        public CommitDataGridItemViewModel(string commitId
            , string taskId
            , string title
            , string author
            , string dateTime)
        {
            _commitId = commitId;
            _taskId = taskId;
            _title = title;
            _author = author;
            _dateTime = dateTime;
        }
    }
}
