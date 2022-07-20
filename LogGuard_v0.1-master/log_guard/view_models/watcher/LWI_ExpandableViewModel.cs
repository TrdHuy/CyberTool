using cyber_base.view_model;
using log_guard.@base.watcher;
using log_guard.definitions;
using System.Collections.Generic;
using System.Windows.Input;

namespace log_guard.view_models.watcher
{
    internal class LWI_ExpandableViewModel : LogWatcherItemViewModel, IExpandableElements
    {
        private ICommand _expandButtonCommand;
        private ICommand _deleteButtonCommand;
        private List<ILogWatcherElements> _childs;

        public ICommand ExpandButtonCommand { get => _expandButtonCommand; set => _expandButtonCommand = value; }
        public ICommand DeleteButtonCommand { get => _deleteButtonCommand; set => _deleteButtonCommand = value; }
        public List<ILogWatcherElements> Childs { get => _childs; set => _childs = value; }

        public LWI_ExpandableViewModel(BaseViewModel parent)
        {
            ViewType = ElementViewType.ExpandableRowView;
            _childs = new List<ILogWatcherElements>();
        }
    }
}
