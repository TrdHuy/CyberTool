using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using honeyboard_release_service.implement.module;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.others;
using honeyboard_release_service.implement.user_data_manager;
using honeyboard_release_service.implement.view_model;
using honeyboard_release_service.models.VOs;
using honeyboard_release_service.view_models.project_manager.items;
using System;
using System.Collections.Generic;
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
        private Dictionary<string, ProjectVO>? _importedProjects;
        private CommitVO? _latestCommitVO;
        private BaseAsyncTask? _getVersionHistoryTaskCache;
        private bool _isLatestVersionSet = false;

        public event UserDataImportedHandler? UserDataImported;

        public static ReleasingProjectManager Current
        {
            get
            {
                return PublisherModuleManager.RPM_Instance;
            }
        }

        public ProjectVO? CurrentProjectVO { get => _currentProjectVO; }

        public CommitVO? LatestCommitVO
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

        public void CreateNewProjectForCurrentProjectVO(string proPath)
        {
            _currentProjectVO = new ProjectVO(proPath);
            UserDataManager.Current.AddImportedProject(proPath, _currentProjectVO);
            UserDataManager.Current.SetCurrentImportedProject(_currentProjectVO);
        }

        public void SetLatestCommitVO(CommitVO commitVO)
        {
            _latestCommitVO = commitVO;
        }

        public async void UpdateVersionHistoryTimeline()
        {
            _isLatestVersionSet = false;
            var pMViewmodel = ViewModelManager.Current.PMViewModel;

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
                    CommitVO vVO = new CommitVO()
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
                    var vOInCurrentBranch = CurrentProjectVO?
                        .AddCommitVOToCurrentBranch(vVO);

                    if (vOInCurrentBranch != null)
                    {
                        pMViewmodel.VersionHistoryItemContexts.Add(
                            new VersionHistoryItemViewModel(vOInCurrentBranch));
                    }
                }
                , taskFinishedCallback: (s) =>
                {
                    pMViewmodel.IsLoadingProjectVersionHistory = false;
                });
            _getVersionHistoryTaskCache = getVersionHistory;
            pMViewmodel.VersionHistoryItemContexts.Clear();
            pMViewmodel.VersionHistoryListTipVisibility = Visibility.Collapsed;
            pMViewmodel.IsLoadingProjectVersionHistory = true;
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
            _currentProjectVO = currentProject;
            _importedProjects = importedProjects;

            if (currentProject != null && currentProject.OnBranch != null)
            {
                BaseAsyncTask importProjectBranchTask = new ParseProjectBranchsFromVOTask(
                   new object[] { currentProject.Branchs, currentProject.OnBranch }
                   , (result) =>
                   {
                       var source = result.Result
                        as CyberTreeViewObservableCollection<ICyberTreeViewItemContext>;
                       if (source != null)
                       {
                           pMViewmodel.BranchsSource = source;
                       }
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
                UserDataImported?.Invoke(this);
                UpdateVersionHistoryTimeline();
            }
        }
    }

    internal delegate void UserDataImportedHandler(object sender);

}
