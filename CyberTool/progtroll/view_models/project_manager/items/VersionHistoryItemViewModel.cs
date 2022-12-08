using cyber_base.async_task;
using cyber_base.implement.command;
using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.view_model;
using progtroll.@base.view_model;
using progtroll.definitions;
using progtroll.implement.async_task_execute_helper;
using progtroll.implement.project_manager;
using progtroll.implement.ui_event_handler;
using progtroll.implement.ui_event_handler.async_tasks.git_tasks;
using progtroll.implement.view_model;
using progtroll.models.VOs;
using System.ComponentModel;

namespace progtroll.view_models.project_manager.items
{
    internal class VersionHistoryItemViewModel : BaseViewModel, IFirstLastElement, IVirtualizingViewModel
    {
        private bool _isFirst;
        private bool _isLast;
        private string _version = "5.5.5.5";
        private string _email = "huy.td1@samsung.com";
        private string _hour = "10:20:30";
        private string _dayOfWeek = "MON";
        private string _dayOfMonth = "23";
        private VersionUpCommitVO _versionVO;
        private bool _isVersionTitleLoaded = false;
        private bool _isLoadingVersionTitle;
        private BaseAsyncTask? _loadingTaskCache;

        [Bindable(true)]
        public CommandExecuterModel ShowCommitDataGridCommand { get; set; }

        [Bindable(true)]
        public BaseDotNetCommandImpl SyncVersionCommand { get; set; }

        [Bindable(true)]
        public bool IsLoadingVersionTitle
        {
            get
            {
                return _isLoadingVersionTitle;
            }
            set
            {
                _isLoadingVersionTitle = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string DayOfMonth
        {
            get
            {
                return _dayOfMonth;
            }
            set
            {
                _dayOfMonth = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string DayOfWeek
        {
            get
            {
                return _dayOfWeek;
            }
            set
            {
                _dayOfWeek = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Hour
        {
            get
            {
                return _hour;
            }
            set
            {
                _hour = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsFirst
        {
            get
            {
                return _isFirst;
            }
            set
            {
                _isFirst = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsLast
        {
            get
            {
                return _isLast;
            }
            set
            {
                _isLast = value;
                InvalidateOwn();
            }
        }

        public VersionUpCommitVO VersionCommitVO
        {
            get
            {
                return _versionVO;
            }
        }

        public VersionHistoryItemViewModel(VersionUpCommitVO vo)
        {
            _dayOfMonth = vo.ReleaseDateTime.ToString("dd");
            _dayOfWeek = vo.ReleaseDateTime.ToString("ddd").ToUpper();
            _hour = vo.ReleaseDateTime.ToString("hh:mm tt");
            _version = vo.CommitTitle;
            _email = vo.AuthorEmail;
            _versionVO = vo;

            SyncVersionCommand = new BaseDotNetCommandImpl((arg) =>
            {
                if (!_isVersionTitleLoaded)
                {
                    if (_loadingTaskCache != null)
                    {
                        _loadingTaskCache.Dispose();
                    }
                    _loadingTaskCache = GetUpdateVersionTitleTask(true);
                    if (_loadingTaskCache != null)
                    {
                        IsLoadingVersionTitle = true;
                        AsyncTaskManager.Current?.ForceAddVersionPropertiesLoadingTask(_loadingTaskCache);
                    }
                }
            });

            ShowCommitDataGridCommand = new CommandExecuterModel((paramaters) =>
            {
                if (paramaters != null)
                {
                    return PublisherKeyActionListener.Current
                        .OnKey(PublisherDefinition.PUBLISHER_PLUGIN_TAG,
                        PublisherKeyFeatureTag.KEY_TAG_PRT_VM_SHOW_COMMIT_DATA_GRID_FEATURE, paramaters) as ICommandExecuter;
                }
                return null;
            });
        }

        public override string ToString()
        {
            return Version;
        }

        public BaseAsyncTask? GetUpdateVersionTitleTask(bool force = false)
        {
            if (_versionVO != null
                && (_versionVO.Properties?.IsEmpty() ?? true)
                && !_isVersionTitleLoaded
                || force && _versionVO != null)
            {
                _versionVO.Properties = new VersionPropertiesVO();

                BaseAsyncTask getVersionPropFromCommit = new GetVersionPropertiesFromCommitIDTask(
                new string[] {
                    ViewModelManager.Current.PMViewModel.ProjectPath
                    , ViewModelManager.Current.PMViewModel.VersionPropertiesFileName
                    , _versionVO.CommitId
                }
                , callback: (result) =>
                {
                    if (result.MesResult != MessageAsyncTaskResult.Aborted)
                    {
                        var fileContent = result.Result?.ToString();
                        if (!string.IsNullOrEmpty(fileContent))
                        {
                            _versionVO.Properties = ReleasingProjectManager
                               .Current
                               .VAParsingManager
                               .GetVersionPropertiesFromVersionFileContent(fileContent);
                            if (_versionVO.Properties.Major != "")
                            {
                                var versionToString = _versionVO.Properties.ToString();
                                _isVersionTitleLoaded = true;
                                IsLoadingVersionTitle = false;
                                _versionVO.Version = versionToString;
                                Version = versionToString;
                            }
                        }

                    }
                });

                getVersionPropFromCommit.Name = Version;
                getVersionPropFromCommit.OnExecutingChanged += (s, o, n) =>
                {
                    IsLoadingVersionTitle = n;
                };

                return getVersionPropFromCommit;
            }

            return null;
        }

        public void OnVirtualizingViewModelLoaded()
        {
            if (string.IsNullOrEmpty(_versionVO.Version))
            {
                _loadingTaskCache = GetUpdateVersionTitleTask();
                if (_loadingTaskCache != null)
                {
                    AsyncTaskManager.Current?.AddVersionPropertiesLoadingTask(_loadingTaskCache);
                }
            }
            else
            {
                IsLoadingVersionTitle = false;
                Version = _versionVO.Version;
            }
        }
    }
}
