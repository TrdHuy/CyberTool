using cyber_base.implement.async_task;
using cyber_base.implement.models.cyber_treeview;
using cyber_base.implement.utils;
using cyber_base.implement.view_models.cyber_treeview;
using cyber_base.implement.views.cyber_treeview;
using cyber_base.view_model;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.models.VOs;
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
        private BranchItemViewModel? _selectedItem;
        private bool _isLoadingProjectVersionHistory = false;
        private Visibility _versionHistoryListTipVisibility = Visibility.Visible;
        private CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? _branchsSource;
        private ReleasingProjectManager _RPM_Instance = ReleasingProjectManager.Current;

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
                return VersionHistoryItemContexts.Count > 20;
            }
        }

        [Bindable(true)]
        public string SelectedBranch
        {
            get
            {
                return _RPM_Instance.CurrentImportedProjectVO?.OnBranch?.BranchPath ?? "";
            }
            set
            {
                var isShouldExecuteBranchChanged = !string.IsNullOrEmpty(
                    _RPM_Instance.CurrentImportedProjectVO?.OnBranch?.BranchPath);
                if (_RPM_Instance.CurrentImportedProjectVO?.OnBranch?.BranchPath != value)
                {
                    if (_RPM_Instance.CurrentImportedProjectVO != null)
                    {
                        _RPM_Instance.SetCurrentProjectOnBranch(value);
                        InvalidateOwn();

                        if (isShouldExecuteBranchChanged)
                            GestureCommandVM.SelectedBranchChangedCommand.Execute(this);
                    }
                }
            }
        }

        [Bindable(true)]
        public BranchItemViewModel? SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                var branchPath = value?.Branch.BranchPath;
                if (value != null
                    && !string.IsNullOrEmpty(branchPath)
                    && value != _selectedItem
                    && SelectedBranch != branchPath)
                {
                    HandlePreSelectedItemChange(branchPath);
                    SelectedBranch = branchPath;
                }
                _selectedItem = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string VersionPropertiesPath
        {
            get
            {
                return _RPM_Instance.CurrentImportedProjectVO?.VersionFilePath ?? "";
            }
            set
            {
                if (_RPM_Instance.CurrentImportedProjectVO != null)
                {
                    _RPM_Instance.CurrentImportedProjectVO.VersionFilePath = value;
                    if (value.IndexOf(ProjectPath) != -1)
                    {
                        _RPM_Instance.CurrentImportedProjectVO.VersionFilePath =
                            value.Substring(ProjectPath.Length + 1);
                    }

                    InvalidateOwn();
                }
            }
        }

        [Bindable(true)]
        public string ProjectPath
        {
            get
            {
                return _RPM_Instance.CurrentImportedProjectVO?.Path ?? "";
            }

            // Không được gọi hàm set này trong code behind để set project path
            // mục đích của hàm set này chỉ phục vụ việc binding
            // Khi người dùng chọn import project từ path textbox
            // Nó sẽ tạo 1 project mới từ path mà người dùng đã select
            set
            {
                if (value != null)
                {
                    _RPM_Instance.CreateNewProjectForCurrentProjectVO(value);
                    InvalidateOwn();
                }
            }
        }

        [Bindable(true)]
        public PM_GestureCommandVM GestureCommandVM { get; set; }
        [Bindable(true)]
        public PM_ButtonCommandVM ButtonCommandVM { get; set; }

        [Bindable(true)]
        public FirstLastObservableCollection<VersionHistoryItemViewModel> VersionHistoryItemContexts
        {
            get
            {
                return ReleasingProjectManager.Current.VersionHistoryItemContexts;
            }
        }

        [Bindable(true)]
        public CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? BranchsSource
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

        public ProjectManagerViewModel(BaseViewModel parents) : base(parents)
        {
            _branchsSource = new CyberTreeViewObservableCollection<ICyberTreeViewItemContext>();
            GestureCommandVM = new PM_GestureCommandVM(this);
            ButtonCommandVM = new PM_ButtonCommandVM(this);
            VersionHistoryItemContexts.CollectionChanged += (s, e) =>
            {
                Invalidate("IsVirtualizingVersionHistoryList");
            };

            _RPM_Instance.UserDataImported -= HandleUserDataImported;
            _RPM_Instance.UserDataImported += HandleUserDataImported;
            _RPM_Instance.CurrentProjectBranchContextSourceChanged -= HandleProjectBranchContextSourceChanged;
            _RPM_Instance.CurrentProjectBranchContextSourceChanged += HandleProjectBranchContextSourceChanged;
            _RPM_Instance.PreUpdateVersionTimelineBackground -= PreHandleUpdateVersionTimelineBackground;
            _RPM_Instance.PreUpdateVersionTimelineBackground += PreHandleUpdateVersionTimelineBackground;
            _RPM_Instance.VersionTimelineUpdated -= HandleVersionTimelineUpdated;
            _RPM_Instance.VersionTimelineUpdated += HandleVersionTimelineUpdated;
        }

        private void HandleVersionTimelineUpdated(object sender, ReleasingProjectEventArg arg)
        {
            IsLoadingProjectVersionHistory = false;
        }

        private void PreHandleUpdateVersionTimelineBackground(object sender, ReleasingProjectEventArg arg)
        {
            VersionHistoryListTipVisibility = Visibility.Collapsed;
            IsLoadingProjectVersionHistory = true;
        }

        private void HandleProjectBranchContextSourceChanged(object sender
            , CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? oldSource
            , CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? newSource)
        {
            BranchsSource = newSource;
        }

        private void HandleUserDataImported(object sender)
        {
            RefreshViewModel();
        }

        private bool HandlePreSelectedItemChange(string newBranchPath)
        {
            var res = HoneyboardReleaseService.Current
                .ServiceManager?
                .App
                .ShowWaringBox("You are about to checkout \"" + newBranchPath + "\"");
            return res == cyber_base.definition.CyberContactMessage.Yes;
        }

        public void ForceSetSelectedBranch(BranchItemViewModel parents)
        {
            HoneyboardReleaseService
                .Current
                .ServiceManager?
                .App
                .CyberApp
                .Dispatcher
                .Invoke(() =>
                {
                    _selectedItem = parents;
                    var branchPath = parents.Branch.BranchPath;
                    if (!string.IsNullOrEmpty(branchPath))
                    {
                        SelectedBranch = branchPath;
                    }
                    Invalidate("SelectedItem");
                });
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _RPM_Instance.UserDataImported -= HandleUserDataImported;
            _RPM_Instance.CurrentProjectBranchContextSourceChanged -= HandleProjectBranchContextSourceChanged;
            _RPM_Instance.VersionTimelineUpdated -= HandleVersionTimelineUpdated;
            _RPM_Instance.PreUpdateVersionTimelineBackground -= PreHandleUpdateVersionTimelineBackground;
        }
    }
}
