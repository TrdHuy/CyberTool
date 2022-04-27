using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.LogGuard.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher
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
