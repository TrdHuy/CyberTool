using cyber_base.view_model;
using progtroll.implement.project_manager;
using progtroll.models.VOs;
using progtroll.utils;
using progtroll.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.view_models.tab_items
{
    internal class VersionManagerTabViewModel : BaseViewModel
    {
        private const string VERSION_MANAGER_PAGE_TITLE_1 = "Latest version";
        private const string VERSION_MANAGER_PAGE_TITLE_2 = "Selected version";
        private const string VERSION_MANAGER_PAGE_TITLE_3 = "Import a project";
        private string _currentFocusVersionTitle = VERSION_MANAGER_PAGE_TITLE_1;
        private VersionHistoryItemViewModel? _currentFocusVersionCommitVM;

        [Bindable(true)]
        public string CurrentFocusProjectBranch
        {
            get
            {
                return ReleasingProjectManager.Current.CurrentImportedProjectVO?.OnBranch?.BranchPath ?? string.Empty;
            }
        }

        [Bindable(true)]
        public VersionHistoryItemViewModel? CurrentFocusVersionCommitVM
        {
            get
            {
                return _currentFocusVersionCommitVM;
            }
            set
            {
                _currentFocusVersionCommitVM = value;
                
                if(_currentFocusVersionCommitVM == null)
                {
                    CurrentFocusVersionTitle = VERSION_MANAGER_PAGE_TITLE_3;
                }
                else if (_currentFocusVersionCommitVM?.VersionCommitVO.CommitId == ReleasingProjectManager.Current.LatestCommitVO?.CommitId)
                {
                    CurrentFocusVersionTitle = VERSION_MANAGER_PAGE_TITLE_1;
                }
                else 
                {
                    CurrentFocusVersionTitle = VERSION_MANAGER_PAGE_TITLE_2;
                }
                Invalidate("CurrentFocusCommitReleaseDate");
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string CurrentFocusVersionTitle
        {
            get
            {
                return _currentFocusVersionTitle;
            }
            set
            {
                _currentFocusVersionTitle = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string CurrentFocusCommitReleaseDate
        {
            get
            {
                return CurrentFocusVersionCommitVM?.VersionCommitVO.ReleaseDateTime.ToString("dd-MM-yyyy") ?? "NA";
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
        public FirstLastObservableCollection<VersionHistoryItemViewModel> VersionHistoryItemContexts
        {
            get
            {
                return ReleasingProjectManager.Current.VersionHistoryItemContexts;
            }
        }

        public VersionManagerTabViewModel(BaseViewModel parents) : base(parents)
        {
            VersionHistoryItemContexts.CollectionChanged += (s, e) =>
            {
                Invalidate("IsVirtualizingVersionHistoryList");
            };

            ReleasingProjectManager.Current.LatestVersionUpCommitChanged += CurrentLatestVersionCommitChanged;
            ReleasingProjectManager.Current.CurrentProjectChanged += CurrentProjectChanged;
            ReleasingProjectManager.Current.CurrentFocusVersionCommitChanged += CurrentFocusVersionCommitChanged;
        }

        private void CurrentLatestVersionCommitChanged(object sender)
        {
            CurrentFocusVersionCommitVM = ReleasingProjectManager.Current.LatestCommitVM;
            Invalidate("CurrentFocusProjectBranch");
        }

        private void CurrentProjectChanged(object sender, ProjectVO? oldProject, ProjectVO? newProject)
        {
            // cur_project is deleted 
            if (ReleasingProjectManager.Current.CurrentImportedProjectVO == null)
            {
                CurrentFocusVersionCommitVM = null;
                Invalidate("CurrentFocusProjectBranch");
            }
        }

        private void CurrentFocusVersionCommitChanged(object sender)
        {
            CurrentFocusVersionCommitVM = ReleasingProjectManager.Current.CurrentFocusVersionCommitVM;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            ReleasingProjectManager.Current.LatestVersionUpCommitChanged -= CurrentLatestVersionCommitChanged;
            ReleasingProjectManager.Current.CurrentProjectChanged -= CurrentProjectChanged;
            ReleasingProjectManager.Current.CurrentFocusVersionCommitChanged -= CurrentFocusVersionCommitChanged;
        }
    }
}
