using cyber_base.view_model;
using progtroll.definitions;
using progtroll.implement.project_manager;
using progtroll.models.VOs;
using progtroll.view_models.command.tab_items.release_tab;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace progtroll.view_models.tab_items
{
    internal class ReleaseTabViewModel : BaseViewModel
    {
        private static readonly Regex PLMCaseCodeRegex = new Regex(PublisherDefinition.PLM_CASE_CODE_REGEX);
        private string _taskID = "";
        private string _commitTitle = "";
        private string _commitDescription = "";
        private VersionPropertiesVO _modifiedVersionPropVO;

        private Visibility _commitButtonVisibility = Visibility.Visible;
        private Visibility _pushButtonVisibility = Visibility.Hidden;
        private ProjectGitStatus _releaseTabGitStatus;

        private ReleaseTemplateItemViewModel? _selectedReleaseTemplateItem;

        public ProjectGitStatus ReleaseTabGitStatus
        {
            get
            {
                return _releaseTabGitStatus;
            }
            set
            {
                _releaseTabGitStatus = value;
                switch (value)
                {
                    case ProjectGitStatus.None:
                        _commitButtonVisibility = Visibility.Visible;
                        _pushButtonVisibility = Visibility.Hidden;
                        break;
                    case ProjectGitStatus.HavingCommit:
                        _commitButtonVisibility = Visibility.Hidden;
                        _pushButtonVisibility = Visibility.Visible;
                        break;
                }
                Invalidate("CommitButtonVisibility");
                Invalidate("PushButtonVisibility");
            }
        }

        public VersionPropertiesVO ModifiedVersionPropVO
        {
            get
            {
                return _modifiedVersionPropVO;
            }
        }

        [Bindable(true)]
        public RT_ButtonCommandVM ButtonCommandVM { get; set; }

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
        public string Major
        {
            get
            {
                return _modifiedVersionPropVO.Major;
            }
            set
            {
                _modifiedVersionPropVO.Major = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Minor
        {
            get
            {
                return _modifiedVersionPropVO.Minor;
            }
            set
            {
                _modifiedVersionPropVO.Minor = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Patch
        {
            get
            {
                return _modifiedVersionPropVO.Patch;
            }
            set
            {
                _modifiedVersionPropVO.Patch = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Revision
        {
            get
            {
                return _modifiedVersionPropVO.Revision;
            }
            set
            {
                _modifiedVersionPropVO.Revision = value;
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
        public ObservableCollection<ReleaseTemplateItemViewModel> ReleaseTemplateItemSource
        {
            get
            {
                return ReleasingProjectManager.Current.ReleaseTemplateItemSource;
            }
        }

        [Bindable(true)]
        public ReleaseTemplateItemViewModel? SelectedReleaseTemplateItem
        {
            get
            {
                return _selectedReleaseTemplateItem;
            }
            set
            {
                _selectedReleaseTemplateItem = value;

                if (_selectedReleaseTemplateItem != null)
                {
                    _taskID = _selectedReleaseTemplateItem.TaskID;
                    _commitTitle = _selectedReleaseTemplateItem.CommitTitle;
                }

                Invalidate("TaskID");
                Invalidate("CommitTitle");
                InvalidateOwn();
            }
        }

        public ReleaseTabViewModel(BaseViewModel parents) : base(parents)
        {
            ButtonCommandVM = new RT_ButtonCommandVM(this);
            ReleaseTabGitStatus = ProjectGitStatus.None;
            _modifiedVersionPropVO = new VersionPropertiesVO();
        }
    }
}
