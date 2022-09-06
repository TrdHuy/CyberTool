using cyber_base.view_model;
using System.ComponentModel;

namespace progtroll.view_models.tab_items
{
    internal class ReleaseTemplateItemViewModel : BaseViewModel
    {
        private string _displayName = "";
        private string _taskID = "";
        private string _commitTitle = "";

        [Bindable(true)]
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        public string TaskID
        {
            get
            {
                return _taskID;
            }
            set { _taskID = value; }
        }

        public string CommitTitle
        {
            get
            {
                return _commitTitle;
            }
            set { _commitTitle = value; }
        }

        public ReleaseTemplateItemViewModel()
        {
        }
    }
}
