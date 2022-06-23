using cyber_base.async_task;
using honeyboard_release_service.implement.module;
using honeyboard_release_service.implement.ui_event_handler.async_tasks.git_tasks;
using honeyboard_release_service.implement.view_model;
using honeyboard_release_service.models.VOs;
using honeyboard_release_service.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace honeyboard_release_service.implement.project_manager
{
    internal class ReleasingProjectManager : BasePublisherModule
    {
        private ProjectVO? _currentProjectVO;
        private CommitVO? _latestCommitVO;
        private BaseAsyncTask? _getVersionHistoryTaskCache;
        private bool _isLatestVersionSet = false;

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

        public void CreateNewProjectVO(string proPath)
        {
            _currentProjectVO = new ProjectVO(proPath);
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
                        Name = data.Title,
                        ReleaseDateTime = DateTime.ParseExact(data.DateTime, "HH:mm:ss yyyy-MM-dd",
                                   System.Globalization.CultureInfo.InvariantCulture),
                        AuthorEmail = data.Email,
                        CommitId = data.HashId,
                    };
                    if (!_isLatestVersionSet)
                    {
                        ReleasingProjectManager
                            .Current
                            .SetLatestCommitVO(vVO);
                        _isLatestVersionSet = true;
                    }
                    ReleasingProjectManager
                        .Current
                        .CurrentProjectVO?
                        .AddCommitVOToCurrentBranch(vVO);

                    pMViewmodel.VersionHistoryItemContexts.Add(
                        new VersionHistoryItemViewModel(vVO));
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
    }
}
