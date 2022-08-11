using cyber_base.view_model;
using progtroll.definitions;
using progtroll.extensions;
using progtroll.implement.project_manager;
using progtroll.implement.view_helper;
using progtroll.models.VOs;
using progtroll.view_models.calendar_notebook.items;
using progtroll.views.elements.calendar_notebook.@base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace progtroll.view_models.calendar_notebook
{
    internal class CalendarNotebookViewModel : BaseViewModel
    {
        private ObservableCollection<ICalendarNotebookProjectItemContext> _notebookItemContexts;
        private Dictionary<string, ICalendarNotebookProjectItemContext> _notebookItemContextsMap;

        private CalendarNotebookProjectItemViewModel? _currentSelectedProjectItemContext;
        public CalendarNotebookProjectItemViewModel? CurrentSelectedProjectItemContext => _currentSelectedProjectItemContext;
        public Dictionary<string, ICalendarNotebookProjectItemContext> NotebookItemContextsMap => _notebookItemContextsMap;
        public ObservableCollection<ICalendarNotebookProjectItemContext> NotebookItemContexts => _notebookItemContexts;


        [Bindable(true)]
        public ObservableCollection<ICalendarNotebookProjectItemContext> ProjectItemContexts
        {
            get
            {
                return _notebookItemContexts;
            }
            set
            {
                _notebookItemContexts = value;
                InvalidateOwn();
            }
        }

        public CalendarNotebookViewModel(BaseViewModel parent) : base(parent)
        {
            _notebookItemContexts = new ObservableCollection<ICalendarNotebookProjectItemContext>();
            _notebookItemContextsMap = new Dictionary<string, ICalendarNotebookProjectItemContext>();

            ReleasingProjectManager.Current.UserDataImported -= HandleUserDataImported;
            ReleasingProjectManager.Current.UserDataImported += HandleUserDataImported;
            ReleasingProjectManager.Current.ImportedProjectsCollectionChanged -= HandleImportedProjectCollectionChanged;
            ReleasingProjectManager.Current.ImportedProjectsCollectionChanged += HandleImportedProjectCollectionChanged;
            ReleasingProjectManager.Current.CurrentProjectChanged -= HandleCurrentProjectChanged;
            ReleasingProjectManager.Current.CurrentProjectChanged += HandleCurrentProjectChanged;
            ReleasingProjectManager.Current.PreUpdateVersionTimelineBackground -= PreHandleUpdateVersionTimelineBackground;
            ReleasingProjectManager.Current.PreUpdateVersionTimelineBackground += PreHandleUpdateVersionTimelineBackground;
            ReleasingProjectManager.Current.VersionTimelineUpdated -= HandleVersionTimelineUpdated;
            ReleasingProjectManager.Current.VersionTimelineUpdated += HandleVersionTimelineUpdated;
            ReleasingProjectManager.Current.VersionPropertiesFound -= HandleVersionPropertiesFound;
            ReleasingProjectManager.Current.VersionPropertiesFound += HandleVersionPropertiesFound;
        }

        private void HandleVersionPropertiesFound(object sender, ReleasingProjectEventArg arg)
        {
            dynamic eventData = arg.EventData ?? throw new ArgumentNullException("EventData of VersionPropertiesFound must not be null");

            if (eventData.UpdateLevel >= Level.Hard && eventData.VersionUpCommit != null)
            {
                CurrentSelectedProjectItemContext?
                    .CommitSource
                    .Add(new CalendarNotebookCommitItemViewModel(eventData.VersionUpCommit, CurrentSelectedProjectItemContext));
            }

        }

        private void HandleVersionTimelineUpdated(object sender, ReleasingProjectEventArg arg)
        {
            dynamic eventData = arg.EventData ?? throw new ArgumentNullException("EventData of VersionTimelineUpdated must not be null");

            if (eventData.UpdateLevel >= Level.Hard)
            {
                if (CurrentSelectedProjectItemContext != null)
                {
                    CurrentSelectedProjectItemContext.IsLoadingData = false;
                }
            }
        }

        private void PreHandleUpdateVersionTimelineBackground(object sender, ReleasingProjectEventArg arg)
        {
            dynamic eventData = arg.EventData ?? throw new ArgumentNullException("EventData of PreUpdateVersionTimelineBackground must not be null");

            // Cập nhật lại commit source của project đang được import hiện tại
            if (eventData.UpdateLevel >= Level.Hard)
            {
                if (CurrentSelectedProjectItemContext != null)
                {
                    CurrentSelectedProjectItemContext.IsLoadingData = true;
                    CurrentSelectedProjectItemContext.CommitSource.Clear();
                }
            }
        }

        private void HandleCurrentProjectChanged(object sender, ProjectVO? oldProject, ProjectVO? newProject)
        {
            if (newProject != null)
                _currentSelectedProjectItemContext = _notebookItemContextsMap[newProject.Path] as CalendarNotebookProjectItemViewModel;
        }

        private void HandleImportedProjectCollectionChanged(object sender, ProjectsCollectionChangedEventArg arg)
        {
            if (arg.ChangedType == ProjectsCollectionChangedType.Add
                && arg.NewValue != null)
            {
                var context = new CalendarNotebookProjectItemViewModel(arg.NewValue);
                _notebookItemContexts.Add(context);
                _notebookItemContextsMap.Add(arg.NewValue.Path, context);
            }
            else if (arg.ChangedType == ProjectsCollectionChangedType.Modified
                && arg.NewValue != null
                && arg.OldValue != null)
            {
                var context = _notebookItemContextsMap[arg.OldValue.Path];
                _notebookItemContexts.Remove(context);
                var newContext = new CalendarNotebookProjectItemViewModel(arg.NewValue);
                _notebookItemContexts.Add(newContext);
                _notebookItemContextsMap[arg.OldValue.Path] = newContext;
            }
        }

        private void HandleUserDataImported(object sender)
        {
            var importedProjectMap = ReleasingProjectManager.Current.ImportedProjects;

            if (importedProjectMap != null)
            {
                foreach (var project in importedProjectMap.Values)
                {
                    var context = new CalendarNotebookProjectItemViewModel(project);
                    _notebookItemContexts.Add(context);
                    _notebookItemContextsMap.Add(project.Path, context);
                }
            }
        }

        public override void OnViewInstantiated()
        {
            base.OnViewInstantiated();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            ReleasingProjectManager.Current.UserDataImported -= HandleUserDataImported;
            ReleasingProjectManager.Current.ImportedProjectsCollectionChanged -= HandleImportedProjectCollectionChanged;
            ReleasingProjectManager.Current.CurrentProjectChanged -= HandleCurrentProjectChanged;
            ReleasingProjectManager.Current.PreUpdateVersionTimelineBackground -= PreHandleUpdateVersionTimelineBackground;
            ReleasingProjectManager.Current.VersionTimelineUpdated -= HandleVersionTimelineUpdated;
            ReleasingProjectManager.Current.VersionPropertiesFound -= HandleVersionPropertiesFound;
        }
    }
}
