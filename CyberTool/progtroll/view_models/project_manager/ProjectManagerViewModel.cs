using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using cyber_base.view_model;
using progtroll.implement.project_manager;
using progtroll.models.VOs;
using progtroll.view_models.command.project_manager;
using progtroll.view_models.project_manager.items;
using System;
using System.ComponentModel;
using System.Windows;

namespace progtroll.view_models.project_manager
{
    internal class ProjectManagerViewModel : BaseViewModel
    {
        private BranchItemViewModel? _selectedItem;
        private bool _isLoadingProjectVersionHistory = false;
        private Visibility _versionHistoryListTipVisibility = Visibility.Visible;
        private CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? _branchsSource;
        private ReleasingProjectManager _RPM_Instance = ReleasingProjectManager.Current;
        private string _versionFileName = "";
        private Func<bool> _isShouldOpenVersionAttrFileChooserWindow;
        
        [Bindable(true)]
        public object? SelectedVersionHistoryItem
        {
            get
            {
                return _RPM_Instance.CurrentFocusVersionCommitVM;
            }
            set
            {
                _RPM_Instance.CurrentFocusVersionCommitVM = value as VersionHistoryItemViewModel;
            }
        }
       
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
        public string VersionPropertiesFileName
        {
            get
            {
                return _versionFileName;
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

        
        [Bindable(true)]
        public Func<bool> IsShouldOpenVersionAttrFileChooserWindow
        {
            get
            {
                return _isShouldOpenVersionAttrFileChooserWindow;
            }
        }

        public ProjectManagerViewModel(BaseViewModel parents) : base(parents)
        {
            _isShouldOpenVersionAttrFileChooserWindow = () =>
            {
                if(_RPM_Instance.CurrentImportedProjectVO == null)
                {
                    HoneyboardReleaseService
                        .Current
                        .ServiceManager?
                        .App
                        .ShowWaringBox("Please import a project first!");
                    return false;
                }
                return true;
            };

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
            _RPM_Instance.CurrentProjectVersionFilePathChanged -= HandleVersionFilePathChanged;
            _RPM_Instance.CurrentProjectVersionFilePathChanged += HandleVersionFilePathChanged;
            _RPM_Instance.CurrentProjectChanged -= HandleCurrentImportProjectChanged;
            _RPM_Instance.CurrentProjectChanged += HandleCurrentImportProjectChanged;
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
            _RPM_Instance.CurrentProjectVersionFilePathChanged -= HandleVersionFilePathChanged;
            _RPM_Instance.CurrentProjectChanged -= HandleCurrentImportProjectChanged;
        }

        #region Event handler
        private void HandleCurrentImportProjectChanged(object sender, ProjectVO? oldProject, ProjectVO? newProject)
        {
            UpdateVersionPropertiesFileName();
        }

        private void HandleVersionFilePathChanged(object sender, ReleasingProjectEventArg arg)
        {
            UpdateVersionPropertiesFileName();
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
            if (_RPM_Instance.CurrentImportedProjectVO != null)
            {
                var filePath = _RPM_Instance.CurrentImportedProjectVO.VersionFilePath;
                if (filePath.IndexOf(ProjectPath) != -1)
                {
                    _versionFileName = filePath.Substring(ProjectPath.Length + 1);
                }
            }
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
        #endregion

        private void UpdateVersionPropertiesFileName()
        {
            if (_RPM_Instance.CurrentImportedProjectVO != null)
            {
                var filePath = _RPM_Instance.CurrentImportedProjectVO.VersionFilePath;
                if (filePath.IndexOf(ProjectPath) != -1)
                {
                    _versionFileName = filePath.Substring(ProjectPath.Length + 1);
                }

                Invalidate("VersionPropertiesFileName");
            }
            else
            {
                _versionFileName = "";
                Invalidate("VersionPropertiesFileName");
            }
        }
    }
}
