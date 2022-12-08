using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using cyber_base.view_model;
using progtroll.definitions;
using progtroll.implement.project_manager;
using progtroll.view_models.command.tab_items.merge_tab;
using progtroll.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace progtroll.view_models.tab_items
{
    internal class MergeTabViewModel : BaseViewModel
    {
        private static readonly Regex PLMCaseCodeRegex = new Regex(PublisherDefinition.PLM_CASE_CODE_REGEX);
        private string _taskID = "";
        private string _commitTitle = "";
        private string _commitDescription = "";
        private CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? _branchsSource;
        private ReleasingProjectManager _RPM_Instance = ReleasingProjectManager.Current;
        private string _selectedInceptionBranch = "";
        private BranchItemViewModel? _selectedInceptionItem;
        private Visibility _commitButtonVisibility = Visibility.Visible;
        private Visibility _pushButtonVisibility = Visibility.Hidden;
        private Visibility _checkMergeButtonVisibility = Visibility.Hidden;
        private ProjectGitStatus _mergeTabGitStatus;

        public ProjectGitStatus MergeTabGitStatus
        {
            get
            {
                return _mergeTabGitStatus;
            }
            set
            {
                _mergeTabGitStatus = value;
                switch (value)
                {
                    case ProjectGitStatus.None:
                        _commitButtonVisibility = Visibility.Visible;
                        _pushButtonVisibility = Visibility.Hidden;
                        _checkMergeButtonVisibility = Visibility.Hidden;
                        break;
                    case ProjectGitStatus.HavingCommit:
                        _commitButtonVisibility = Visibility.Hidden;
                        _pushButtonVisibility = Visibility.Visible;
                        _checkMergeButtonVisibility = Visibility.Hidden;
                        break;
                    case ProjectGitStatus.HavingUnmergeFile:
                        _commitButtonVisibility = Visibility.Hidden;
                        _pushButtonVisibility = Visibility.Hidden;
                        _checkMergeButtonVisibility = Visibility.Visible;
                        break;

                }
                Invalidate("CheckMergeButtonVisibility");
                Invalidate("CommitButtonVisibility");
                Invalidate("PushButtonVisibility");
            }
        }


        [Bindable(true)]
        public Visibility CheckMergeButtonVisibility
        {
            get
            {
                return _checkMergeButtonVisibility;
            }
            set
            {
                _checkMergeButtonVisibility = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public Visibility CommitButtonVisibility
        {
            get
            {
                return _commitButtonVisibility;
            }
            set
            {
                _commitButtonVisibility = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public Visibility PushButtonVisibility
        {
            get
            {
                return _pushButtonVisibility;
            }
            set
            {
                _pushButtonVisibility = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public MT_ButtonCommandVM ButtonCommandVM { get; set; }

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
        public string TaskIDUri
        {
            get
            {
                if (PLMCaseCodeRegex.IsMatch(_taskID))
                {
                    return "http://splm.sec.samsung.net/wl/com/ozSearch/searchMain.do?searchWord=" + _taskID;
                }
                else
                {
                    return "https://mobilerndhub.sec.samsung.net/its/browse/" + _taskID;
                }
            }

        }

        [Bindable(true)]
        public string TaskID
        {
            get
            {
                return _taskID;
            }
            set
            {
                _taskID = value;
                InvalidateOwn();
                Invalidate("TaskIDUri");
            }
        }

        [Bindable(true)]
        public string CommitTitle
        {
            get
            {
                return _commitTitle;
            }
            set
            {
                _commitTitle = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string CommitDescription
        {
            get
            {
                return _commitDescription;
            }
            set
            {
                _commitDescription = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string SelectedInceptionBranch
        {
            get
            {
                return _selectedInceptionBranch;
            }
            set
            {
                _selectedInceptionBranch = value;
                InvalidateOwn();
            }
        }


        [Bindable(true)]
        public BranchItemViewModel? SelectedInceptionItem
        {
            get
            {
                return _selectedInceptionItem;
            }
            set
            {
                var branchPath = value?.Branch.BranchPath ?? "";
                if (value != null
                    && !string.IsNullOrEmpty(branchPath)
                    && value != _selectedInceptionItem
                    && SelectedInceptionBranch != branchPath)
                {
                    SelectedInceptionBranch = branchPath;
                }
                _selectedInceptionItem = value;
                InvalidateOwn();
            }
        }

        public MergeTabViewModel(BaseViewModel parents) : base(parents)
        {
            ButtonCommandVM = new MT_ButtonCommandVM(this);
            MergeTabGitStatus = ProjectGitStatus.None;
            _RPM_Instance.CurrentProjectBranchContextSourceChanged -= HandleProjectBranchContextSourceChanged;
            _RPM_Instance.CurrentProjectBranchContextSourceChanged += HandleProjectBranchContextSourceChanged;
        }

        private void HandleProjectBranchContextSourceChanged(object sender
            , CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? oldSource
            , CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? newSource)
        {
            BranchsSource = newSource;
        }
    }
}
