﻿using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using honeyboard_release_service.implement.module;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.others;
using honeyboard_release_service.implement.user_data_manager;
using honeyboard_release_service.implement.view_model;
using honeyboard_release_service.models.VOs;
using honeyboard_release_service.utils;
using honeyboard_release_service.view_models.calendar_notebook.items;
using honeyboard_release_service.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace honeyboard_release_service.implement.project_manager
{
    internal class ReleasingProjectManager : BasePublisherModule
    {
        private ProjectVO? _currentProjectVO;
        private Dictionary<string, ProjectVO> _importedProjects = new Dictionary<string, ProjectVO>();
        private VersionUpCommitVO? _latestCommitVO;
        private BaseAsyncTask? _getVersionHistoryTaskCache;
        private bool _isLatestVersionSet = false;
        private CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? _currentProjectBranchContextSource;
        private FirstLastObservableCollection<VersionHistoryItemViewModel> _versionHistoryItemContexts;

        private event UserDataImportedHandler? _userDataImported;
        private event ImportedProjectsCollectionChangedHandler? _importedProjectsCollectionChanged;
        private event CurrentProjectChangedHandler? _currentProjectChanged;
        private event CurrentProjectBranchContextSourceChangedHandler? _currentProjectBranchContextSourceChanged;
        private event LatestVersionUpCommitChangedHandler? _latestVersionUpCommitChanged;
        
        public static ReleasingProjectManager Current
        {
            get
            {
                return PublisherModuleManager.RPM_Instance;
            }
        }

        public event LatestVersionUpCommitChangedHandler LatestVersionUpCommitChanged
        {
            add
            {
                _latestVersionUpCommitChanged += value;
            }
            remove
            {
                _latestVersionUpCommitChanged -= value;
            }
        }

        public event CurrentProjectBranchContextSourceChangedHandler CurrentProjectBranchContextSourceChanged
        {
            add
            {
                _currentProjectBranchContextSourceChanged += value;
            }
            remove
            {
                _currentProjectBranchContextSourceChanged -= value;
            }
        }

        public event CurrentProjectChangedHandler CurrentProjectChanged
        {
            add
            {
                _currentProjectChanged += value;
            }
            remove
            {
                _currentProjectChanged -= value;
            }
        }

        public event UserDataImportedHandler UserDataImported
        {
            add
            {
                _userDataImported += value;
            }
            remove
            {
                _userDataImported -= value;
            }
        }

        public event ImportedProjectsCollectionChangedHandler ImportedProjectsCollectionChanged
        {
            add
            {
                _importedProjectsCollectionChanged += value;
            }
            remove
            {
                _importedProjectsCollectionChanged -= value;
            }
        }

        public CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? CurrentProjectBranchContextSource
        {
            get
            {
                return _currentProjectBranchContextSource;
            }
            private set
            {
                var oldValue = _currentProjectBranchContextSource;
                _currentProjectBranchContextSource = value;
                if (oldValue != value)
                {
                    _currentProjectBranchContextSourceChanged?.Invoke(this, oldValue, value);
                }
            }
        }

        public ProjectVO? CurrentProjectVO { get => _currentProjectVO; }

        public Dictionary<string, ProjectVO> ImportedProjects
        {
            get => _importedProjects;
            private set
            {
                _importedProjects = value;
            }
        }

        public VersionHistoryItemViewModel? LatestCommitVM
        {
            get
            {
                return _versionHistoryItemContexts.First;
            }

        }

        public VersionUpCommitVO? LatestCommitVO
        {
            get
            {
                return _latestCommitVO;
            }

        }

        public string SelectedBranchPath
        {
            get
            {
                return _currentProjectVO?.OnBranch?.BranchPath ?? "";
            }
        }

        public string ProjectPath
        {
            get
            {
                return _currentProjectVO?.Path ?? "";
            }
        }

        public string VersionPropertiesPath
        {
            get
            {
                return _currentProjectVO?.VersionFilePath ?? "";
            }
        }

        public FirstLastObservableCollection<VersionHistoryItemViewModel> VersionHistoryItemContexts
        {
            get
            {
                return _versionHistoryItemContexts;
            }
        }

        private ReleasingProjectManager()
        {
            _versionHistoryItemContexts = new FirstLastObservableCollection<VersionHistoryItemViewModel>();
            _versionHistoryItemContexts.FirstChanged -= OnVersionHistoryItemContextsFirstChanged;
            _versionHistoryItemContexts.FirstChanged += OnVersionHistoryItemContextsFirstChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            // Memory leak issue
            // Ref: https://michaelscodingspot.com/5-techniques-to-avoid-memory-leaks-by-events-in-c-net-you-should-know/
            _versionHistoryItemContexts.FirstChanged -= OnVersionHistoryItemContextsFirstChanged;
        }

        private void OnVersionHistoryItemContextsFirstChanged(object sender, VersionHistoryItemViewModel? oldFirst, VersionHistoryItemViewModel? newFirst)
        {
            _latestVersionUpCommitChanged?.Invoke(this);
        }

        public BranchVO? GetBranchOfCurrentProjectFromPath(string path)
        {
            return CurrentProjectVO?.Branchs[path];
        }



        public void CreateNewProjectForCurrentProjectVO(string proPath)
        {
            var oldProject = _currentProjectVO;
            _currentProjectVO = new ProjectVO(proPath);
            if (string.IsNullOrEmpty(proPath))
            {
                _currentProjectVO = null;
            }
            else
            {
                if (!ImportedProjects.ContainsKey(proPath))
                {
                    ImportedProjects[proPath] = _currentProjectVO;
                    _importedProjectsCollectionChanged?.Invoke(this
                        , new ProjectsCollectionChangedEventArg(_currentProjectVO
                            , null
                            , ProjectsCollectionChangedType.Add));
                }
                else
                {
                    var oldProjectVO = ImportedProjects[proPath];
                    ImportedProjects[proPath] = _currentProjectVO;
                    _importedProjectsCollectionChanged?.Invoke(this
                        , new ProjectsCollectionChangedEventArg(_currentProjectVO
                            , oldProjectVO
                            , ProjectsCollectionChangedType.Modified));
                }
                UserDataManager.Current.AddImportedProject(proPath, _currentProjectVO);
                UserDataManager.Current.SetCurrentImportedProject(_currentProjectVO);
            }
            _currentProjectChanged?.Invoke(this, oldProject, _currentProjectVO);

        }

        public void SetLatestCommitVO(VersionUpCommitVO commitVO)
        {
            _latestCommitVO = commitVO;
        }

        public void SetCurrentProjectBranchContextSource(
            CyberTreeViewObservableCollection<ICyberTreeViewItemContext> source)
        {
            CurrentProjectBranchContextSource = source;
        }

        public void SetCurrentProjectOnBranch(string branch)
        {
            if (CurrentProjectVO?.OnBranch?.BranchPath != branch)
            {
                if (CurrentProjectVO != null)
                {
                    CurrentProjectVO.SetOnBranch(branch);
                }
            }
        }

        public VersionUpCommitVO? AddCommitToCurrentBranch(VersionUpCommitVO vVO)
        {
            return CurrentProjectVO?.AddCommitVOToCurrentBranch(vVO);
        }

        public void AddProjectBranch(BranchVO bVO)
        {
            CurrentProjectVO?.AddProjectBranch(bVO);
        }

        public async void UpdateVersionHistoryTimelineInBackground(bool isNeedToUpdateCurrentPorjectOnCalendarNotebook = true)
        {
            _isLatestVersionSet = false;
            var pMViewmodel = ViewModelManager.Current.PMViewModel;
            var cnViewmodel = ViewModelManager.Current.CNViewModel;

            if (_getVersionHistoryTaskCache != null
                && !_getVersionHistoryTaskCache.IsCompleted
                && !_getVersionHistoryTaskCache.IsCanceled
                && !_getVersionHistoryTaskCache.IsFaulted)
            {
                _getVersionHistoryTaskCache.Cancel();
            }
            BaseAsyncTask getVersionHistory = new GetVersionHistoryTask(
                new string[] { ProjectPath
                    , VersionPropertiesPath }
                , versionPropertiesFoundCallback: (prop, task) =>
                {
                    dynamic data = prop;
                    VersionUpCommitVO vVO = new VersionUpCommitVO()
                    {
                        CommitTitle = data.Title,
                        ReleaseDateTime = DateTime.ParseExact(data.DateTime, "HH:mm:ss yyyy-MM-dd",
                                   System.Globalization.CultureInfo.InvariantCulture),
                        AuthorEmail = data.Email,
                        CommitId = data.HashId,
                    };
                    if (!_isLatestVersionSet)
                    {
                        SetLatestCommitVO(vVO);
                        _isLatestVersionSet = true;
                    }

                    // Xử lý trên model
                    var vOInCurrentBranch = AddCommitToCurrentBranch(vVO);

                    // Xử lý trên viewmodel
                    if (vOInCurrentBranch != null)
                    {
                        _versionHistoryItemContexts.Add(
                            new VersionHistoryItemViewModel(vOInCurrentBranch));

                        if (isNeedToUpdateCurrentPorjectOnCalendarNotebook)
                        {
                            cnViewmodel.CurrentSelectedProjectItemContext?
                                .CommitSource.Add(new CalendarNotebookCommitItemViewModel(
                                    vOInCurrentBranch
                                    , cnViewmodel.CurrentSelectedProjectItemContext));
                        }
                    }
                }
                , taskFinishedCallback: (s) =>
                {
                    pMViewmodel.IsLoadingProjectVersionHistory = false;
                    if (isNeedToUpdateCurrentPorjectOnCalendarNotebook
                        && cnViewmodel.CurrentSelectedProjectItemContext != null)
                    {
                        cnViewmodel.CurrentSelectedProjectItemContext.IsLoadingData = false;
                    }
                });
            _getVersionHistoryTaskCache = getVersionHistory;
            _versionHistoryItemContexts.Clear();

            // Xử lý project manager viewmodel trước khi thực hiện task
            pMViewmodel.VersionHistoryListTipVisibility = Visibility.Collapsed;
            pMViewmodel.IsLoadingProjectVersionHistory = true;

            // Xử lý calendar notebook viewmodel trước khi thực hiện task
            if (isNeedToUpdateCurrentPorjectOnCalendarNotebook
                && cnViewmodel.CurrentSelectedProjectItemContext != null)
            {
                cnViewmodel.CurrentSelectedProjectItemContext.IsLoadingData = true;
                cnViewmodel.CurrentSelectedProjectItemContext.CommitSource.Clear();
            }
            await getVersionHistory.Execute();
        }

        /// <summary>
        /// Chỉ gọi hàm này khi user data đã được load thành công
        /// </summary>
        /// <param name="currentProject"></param>
        /// <param name="importedProjects"></param>
        public void UpdateWorkingProjectsAfterLoadedFromUserData(ProjectVO? currentProject
            , Dictionary<string, ProjectVO> importedProjects)
        {
            if (_getVersionHistoryTaskCache != null
                && !_getVersionHistoryTaskCache.IsCompleted
                && !_getVersionHistoryTaskCache.IsCanceled
                && !_getVersionHistoryTaskCache.IsFaulted)
            {
                _getVersionHistoryTaskCache.Cancel();
            }
            var pMViewmodel = ViewModelManager.Current.PMViewModel;
            var oldProject = _currentProjectVO;
            _currentProjectVO = currentProject;
            ImportedProjects = importedProjects;

            if (currentProject != null && currentProject.OnBranch != null)
            {
                BaseAsyncTask importProjectBranchTask = new ParseProjectBranchsFromVOTask(
                   new object[] { currentProject.Branchs, currentProject.OnBranch }
                   , (result) =>
                   {
                       var source = result.Result
                        as CyberTreeViewObservableCollection<ICyberTreeViewItemContext>;
                       CurrentProjectBranchContextSource = source;
                   });
                List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
                tasks.Add(importProjectBranchTask);

                MultiAsyncTask multiTask = new MultiAsyncTask(tasks
                  , new CancellationTokenSource()
                  , null
                  , name: "Importing branchs from user data"
                  , delayTime: 0
                  , reportDelay: 100);
                HoneyboardReleaseService.Current.ServiceManager?.App.OpenMultiTaskBox(
                    title: "Importing branchs from user data"
                   , task: multiTask
                   , isCancelable: false);
            }
            _userDataImported?.Invoke(this);
            UpdateVersionHistoryTimelineInBackground(isNeedToUpdateCurrentPorjectOnCalendarNotebook: false);
            // Nên gọi sự kiện này sau khi toàn bộ user data đã được imported thành công
            _currentProjectChanged?.Invoke(this, oldProject, _currentProjectVO);
        }
    }

    internal delegate void UserDataImportedHandler(object sender);
    internal delegate void LatestVersionUpCommitChangedHandler(object sender);
    internal delegate void ImportedProjectsCollectionChangedHandler(object sender, ProjectsCollectionChangedEventArg arg);
    internal delegate void CurrentProjectChangedHandler(object sender, ProjectVO? oldProject, ProjectVO? newProject);
    internal delegate void CurrentProjectBranchContextSourceChangedHandler(object sender
        , CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? oldSource
        , CyberTreeViewObservableCollection<ICyberTreeViewItemContext>? newSource);

    internal class ProjectsCollectionChangedEventArg
    {
        public ProjectVO? OldValue { get; private set; }
        public ProjectVO? NewValue { get; private set; }
        public ProjectsCollectionChangedType ChangedType { get; private set; }
        public ProjectsCollectionChangedEventArg(
            ProjectVO? newValue
            , ProjectVO? oldValue
            , ProjectsCollectionChangedType changedType)
        {
            OldValue = oldValue;
            NewValue = newValue;
            ChangedType = changedType;
        }
    }
    internal enum ProjectsCollectionChangedType
    {
        Add = 1,
        Remove = 2,
        Modified = 3
    }
}