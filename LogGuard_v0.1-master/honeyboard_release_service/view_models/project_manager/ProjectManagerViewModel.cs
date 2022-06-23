﻿using cyber_base.implement.async_task;
using cyber_base.implement.models.cyber_treeview;
using cyber_base.implement.utils;
using cyber_base.implement.view_models.cyber_treeview;
using cyber_base.implement.views.cyber_treeview;
using cyber_base.view_model;
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
        private FirstLastObservableCollection<VersionHistoryItemViewModel> _versionHistoryItemContexts;
        private object? _selectedItem;
        private bool _isLoadingProjectVersionHistory = false;
        private Visibility _versionHistoryListTipVisibility = Visibility.Visible;
        private ProjectVO? _currentProjectVO;
        private CommitVO? _latestCommitVO;
        private CyberTreeViewObservableCollection<ICyberTreeViewItem> _branchsSource;

        public ProjectVO? CurrentProjectVO { get => _currentProjectVO; }
        public CommitVO? LatestCommitVO { get => _latestCommitVO; set => _latestCommitVO = value; }

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
                return _currentProjectVO?.OnBranch?.BranchPath ?? "";
            }
            set
            {
                var isShouldExecuteBranchChanged = !string.IsNullOrEmpty(_currentProjectVO?.OnBranch?.BranchPath);
                if (_currentProjectVO?.OnBranch?.BranchPath != value)
                {
                    if (_currentProjectVO != null)
                    {
                        _currentProjectVO.SetOnBranch(value);
                        InvalidateOwn();

                        if (isShouldExecuteBranchChanged)
                            GestureCommandVM.SelectedBranchChangedCommand.Execute(this);
                    }
                }
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
                var branchPath = value?.ToString();
                if (value != null 
                    && !string.IsNullOrEmpty(branchPath)
                    && value != _selectedItem)
                {
                    HandlePreSelectedItemChange(branchPath);
                    _selectedItem = value;
                    SelectedBranch = branchPath;
                }
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string VersionPropertiesPath
        {
            get
            {
                return _currentProjectVO?.VersionFilePath ?? "";
            }
            set
            {
                if (_currentProjectVO != null)
                {
                    _currentProjectVO.VersionFilePath = value;
                    if (value.IndexOf(ProjectPath) != -1)
                    {
                        _currentProjectVO.VersionFilePath = value.Substring(ProjectPath.Length + 1);
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
                return _currentProjectVO?.Path ?? "";
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _currentProjectVO = new ProjectVO(value);
                    InvalidateOwn();
                }
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

        public ProjectManagerViewModel(BaseViewModel parents) : base(parents)
        {
            _versionHistoryItemContexts = new FirstLastObservableCollection<VersionHistoryItemViewModel>();
            _branchsSource = new CyberTreeViewObservableCollection<ICyberTreeViewItem>();
            GestureCommandVM = new PM_GestureCommandVM(this);
            _versionHistoryItemContexts.CollectionChanged += (s, e) =>
            {
                Invalidate("IsVirtualizingVersionHistoryList");
            };
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
            _selectedItem = parents;
            var branchPath = parents.ToString();
            if (!string.IsNullOrEmpty(branchPath))
            {
                SelectedBranch = branchPath;
            }
            Invalidate("SelectedItem");
        }
    }
}
