using cyber_base.view_model;
using progtroll.views.elements.commit_data_grid.@base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.view_models.version_comparator
{
    internal class VersionComparatorViewModel : BaseViewModel
    {
        private ObservableCollection<ICommitDataGridItemContext> _commitItemsSource { get; set; }

        [Bindable(true)]
        public ObservableCollection<ICommitDataGridItemContext> CommitItemsSource
        {
            get
            {
                return _commitItemsSource;

            }
            set
            {
                _commitItemsSource = value;
                InvalidateOwn();
            }
        }

        public VersionComparatorViewModel()
        {
            _commitItemsSource = new ObservableCollection<ICommitDataGridItemContext>();
        }
    }
}
