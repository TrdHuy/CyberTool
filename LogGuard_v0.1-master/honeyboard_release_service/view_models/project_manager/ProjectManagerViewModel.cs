using cyber_base.implement.async_task;
using cyber_base.implement.models.cyber_treeview;
using cyber_base.implement.utils;
using cyber_base.implement.view_models.cyber_treeview;
using cyber_base.implement.views.cyber_treeview;
using cyber_base.view_model;
using honeyboard_release_service.utils;
using honeyboard_release_service.view_models.command.project_manager;
using honeyboard_release_service.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace honeyboard_release_service.view_models.project_manager
{
    internal class ProjectManagerViewModel : BaseViewModel
    {
        private FirstLastObservableCollection<VersionHistoryItemViewModel> _versionHistoryItemContexts;
        private string _projectPath = "";
        private string _versionPropertiesPath = "";
        private CyberTreeViewObservableCollection<ICyberTreeViewItem> _branchsSource = new CyberTreeViewObservableCollection<ICyberTreeViewItem>();
        private object? _selectedItem;
        private string _selectedBranch = "";
        private bool _isLoadingProjectVersionHistory = false;
        private Visibility _versionHistoryListTipVisibility = Visibility.Visible;

        [Bindable(true)]
        public Visibility VersionHistoryListTipVisibility
        {
            get
            {
                return _versionHistoryListTipVisibility;
            }
            set
            {
                _versionHistoryListTipVisibility = value;
                InvalidateOwn();
            }


        }
        [Bindable(true)]
        public bool IsLoadingProjectVersionHistory
        {
            get
            {
                return _isLoadingProjectVersionHistory;
            }
            set
            {
                _isLoadingProjectVersionHistory = value;
                InvalidateOwn();
            }

        }

        [Bindable(true)]
        public bool IsVirtualizingVersionHistoryList
        {
            get
            {
                return _versionHistoryItemContexts.Count > 20;
            }
        }

        [Bindable(true)]
        public string SelectedBranch
        {
            get
            {
                return _selectedBranch;
            }
            set
            {
                _selectedBranch = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object? SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                if (value != null)
                {
                    SelectedBranch = value.ToString() ?? "";
                }
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string VersionPropertiesPath
        {
            get
            {
                return _versionPropertiesPath;
            }
            set
            {
                _versionPropertiesPath = value;
                if (value.IndexOf(ProjectPath) != -1)
                {
                    _versionPropertiesPath = value.Substring(ProjectPath.Length + 1);
                }

                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string ProjectPath
        {
            get
            {
                return _projectPath;
            }
            set
            {
                _projectPath = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public PM_GestureCommandVM GestureCommandVM { get; set; }

        [Bindable(true)]
        public FirstLastObservableCollection<VersionHistoryItemViewModel> VersionHistoryItemContexts
        {
            get
            {
                return _versionHistoryItemContexts;
            }
            set
            {
                _versionHistoryItemContexts = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public CyberTreeViewObservableCollection<ICyberTreeViewItem> BranchsSource
        {
            get
            {
                return _branchsSource;
            }

            set
            {
                if (_branchsSource == value)
                {
                    return;
                }
                _branchsSource = value;
                InvalidateOwn();
            }
        }

        public ProjectManagerViewModel(BaseViewModel parents)
        {
            _versionHistoryItemContexts = new FirstLastObservableCollection<VersionHistoryItemViewModel>();

            GestureCommandVM = new PM_GestureCommandVM(this);

            LoadData();
            _versionHistoryItemContexts.CollectionChanged += (s, e) =>
            {
                Invalidate("IsVirtualizingVersionHistoryList");
            };
        }


        private void LoadData()
        {
            var perForItem = new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("P4"));

            perForItem.AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("Setting")));
            perForItem.AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("Config")));

            for (int i = 0; i < 2; i++)
            {
                perForItem.AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("" + i)));
            }

            var item1 = new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("SIP team"));

            item1.AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("1")));

            item1.AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("Performance"))
                    .AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("1")))
                    .AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("2")))
                    .AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("3"))));

            item1.AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("View"))
                    .AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("1")))
                    .AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("2"))));
            item1.AddItem(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("4")));

            item1.IsSelected = true;
            BranchsSource.Add(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("1")));
            BranchsSource.Add(item1);
            BranchsSource.Add(perForItem);
            BranchsSource.Add(new BaseCyberTreeItemViewModel(new BaseCyberTreeItemVO("4")));

        }

    }
}
