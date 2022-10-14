using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.utils;
using progtroll.implement.project_manager;
using progtroll.implement.ui_event_handler.async_tasks.git_tasks;
using progtroll.implement.view_model;
using progtroll.view_models.project_manager.items;
using progtroll.view_models.version_comparator;
using progtroll.view_models.version_comparator.item;
using progtroll.views.elements.commit_data_grid;
using progtroll.views.elements.commit_data_grid.@base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace progtroll.implement.ui_event_handler.actions.version_manager
{
    internal class PRT_VM_ShowCommitDataGridAction : BaseCommandExecuter
    {
        private VersionHistoryItemViewModel? _versionHistoryItemVM;

        public PRT_VM_ShowCommitDataGridAction(string actionID, string builderID, object? dataTransfer, ILogger? logger)
            : base(actionID, builderID, dataTransfer, logger)
        {

        }

        protected override bool CanExecute(object? dataTransfer)
        {
            if (DataTransfer == null || DataTransfer.Count == 0) return false;

            _versionHistoryItemVM = DataTransfer[0] as VersionHistoryItemViewModel;

            if (_versionHistoryItemVM == null) return false;

            return true;
        }
        protected override void ExecuteCommand()
        {
            var commitItemsSource = new ObservableCollection<ICommitDataGridItemContext>();

            var fromCommitId = _versionHistoryItemVM?
                                    .VersionCommitVO
                                    .CommitId ?? throw new ArgumentNullException("Commit id must not be null!");

            var compareVersion = _versionHistoryItemVM?
                                    .VersionCommitVO
                                    .Properties ?? throw new ArgumentNullException("Version properties must not be null!");

            var toCommitId = ViewModelManager
                                .Current
                                .VMTViewModel
                                .CurrentFocusVersionCommitVM?
                                .VersionCommitVO
                                .CommitId ?? throw new ArgumentNullException("Commit id must not be null!");

            var selectedVersion = ViewModelManager
                                    .Current
                                    .VMTViewModel
                                    .CurrentFocusVersionCommitVM?
                                    .VersionCommitVO
                                    .Properties ?? throw new ArgumentNullException("Version properties must not be null!");

            if (compareVersion == selectedVersion)
            {
                ProgTroll.Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("Compare version must be difference with Selected version"!);

                return;
            }

            if (compareVersion > selectedVersion)
            {
                var tempCommitId = fromCommitId;
                fromCommitId = toCommitId;
                toCommitId = tempCommitId;
            }

            var getCommitHistoryTask = new GetVersionHistoryTask(
                new string[]
                {
                    ReleasingProjectManager.Current.ProjectPath,
                    ReleasingProjectManager.Current.VersionPropertiesPath,
                    fromCommitId,
                    toCommitId,
                }
                , versionPropertiesFoundCallback: (prop, task) =>
                {
                    dynamic data = prop;
                    var commitItemVM = new CommitDataGridItemViewModel(
                            commitId: data.HashId,
                            taskId: data.SubjectID,
                            title: data.Title,
                            author: data.Email,
                            dateTime: data.DateTime
                        );
                    commitItemsSource.Add(commitItemVM);
                });

            List<BaseAsyncTask> listTask = new List<BaseAsyncTask>();
            listTask.Add(getCommitHistoryTask);

            MultiAsyncTask multiTask = new MultiAsyncTask(listTask
               , new CancellationTokenSource()
               , null
               , name: "Getting commit history"
               , delayTime: 0
               , reportDelay: 100);

            var message = ProgTroll.Current.ServiceManager?.App.OpenMultiTaskBox("Getting commit history"
                , multiTask);

            if (message == cyber_base.definition.CyberContactMessage.Done)
            {
                var comparatorUC = new VersionComparator();
                var comparatorUCContext = comparatorUC.DataContext as VersionComparatorViewModel;

                if (comparatorUCContext != null)
                {
                    comparatorUCContext.CommitItemsSource = commitItemsSource;
                    ProgTroll.Current
                        .ServiceManager?
                        .App
                        .ShowUserControlWindow(comparatorUC
                            , cyber_base.definition.CyberOwner.ServiceManager
                            , width: 1100
                            , height: 550);
                }

            }

        }
    }
}
