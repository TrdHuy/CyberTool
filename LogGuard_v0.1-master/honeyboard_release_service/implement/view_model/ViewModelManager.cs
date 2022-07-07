﻿using cyber_base.implement.utils;
using honeyboard_release_service.@base.module;
using honeyboard_release_service.implement.module;
using honeyboard_release_service.view_models;
using honeyboard_release_service.view_models.calendar_notebook;
using honeyboard_release_service.view_models.log_monitor;
using honeyboard_release_service.view_models.project_manager;
using honeyboard_release_service.view_models.tab_items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.view_model
{
    internal class ViewModelManager : BasePublisherModule
    {
        public ObservableDictionary<Type, object> DataContextCache { get; }

        public HoneyReleaseServiceViewModel HRSViewModel
        {
            get
            {
                return (HoneyReleaseServiceViewModel)DataContextCache[typeof(HoneyReleaseServiceViewModel)];
            }
        }

        public CalendarNotebookViewModel CNViewModel
        {
            get
            {
                return (CalendarNotebookViewModel)DataContextCache[typeof(CalendarNotebookViewModel)];
            }
        }

        public ProjectManagerViewModel PMViewModel
        {
            get
            {
                return (ProjectManagerViewModel)DataContextCache[typeof(ProjectManagerViewModel)];
            }
        }

        public LogMonitorViewModel LMViewModel
        {
            get
            {
                return (LogMonitorViewModel)DataContextCache[typeof(LogMonitorViewModel)];
            }
        }

        public ReleaseTabViewModel RTViewModel
        {
            get
            {
                return (ReleaseTabViewModel)DataContextCache[typeof(ReleaseTabViewModel)];
            }
        }

        public static ViewModelManager Current
        {
            get
            {
                return PublisherModuleManager.VMM_Instance;
            }
        }

        private ViewModelManager()
        {
            DataContextCache = new ObservableDictionary<Type, object>();
            DataContextCache.CollectionChanged -= OnContextCollectionsChanged;
            DataContextCache.CollectionChanged += OnContextCollectionsChanged;
        }

        private void OnContextCollectionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
        }

    }
}
