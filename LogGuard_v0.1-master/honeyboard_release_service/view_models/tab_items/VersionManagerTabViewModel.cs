using cyber_base.view_model;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.utils;
using honeyboard_release_service.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models.tab_items
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
                return ReleasingProjectManager.Current.CurrentProjectVO?.OnBranch?.BranchPath ?? string.Empty;
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
                if (_currentFocusVersionCommitVM?.VersionCommitVO.Properties == ReleasingProjectManager.Current.LatestCommitVO?.Properties)
                {
                    CurrentFocusVersionTitle = VERSION_MANAGER_PAGE_TITLE_1;
                }
                else if (_currentFocusVersionCommitVM != null)
                {
                    CurrentFocusVersionTitle = VERSION_MANAGER_PAGE_TITLE_2;
                }
                else
                {
                    CurrentFocusVersionTitle = VERSION_MANAGER_PAGE_TITLE_3;
                }
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

            CurrentFocusVersionCommitVM = ReleasingProjectManager.Current.LatestCommitVM;
            ReleasingProjectManager.Current.LatestVersionUpCommitChanged += CurrentLatestVersionCommitChanged;
        }

        private void CurrentLatestVersionCommitChanged(object sender)
        {
            CurrentFocusVersionCommitVM = ReleasingProjectManager.Current.LatestCommitVM;
            Invalidate("CurrentFocusProjectBranch");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            ReleasingProjectManager.Current.LatestVersionUpCommitChanged -= CurrentLatestVersionCommitChanged;
        }
    }
}
