﻿using cyber_base.implement.utils;
using progtroll.@base.module;
using progtroll.implement.module;
using progtroll.view_models;
using progtroll.view_models.calendar_notebook;
using progtroll.view_models.log_monitor;
using progtroll.view_models.project_manager;
using progtroll.view_models.tab_items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.implement.view_model
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

        public MergeTabViewModel MTViewModel
        {
            get
            {
                return (MergeTabViewModel)DataContextCache[typeof(MergeTabViewModel)];
            }
        }

        public VersionManagerTabViewModel VMTViewModel
        {
            get
            {
                return (VersionManagerTabViewModel)DataContextCache[typeof(VersionManagerTabViewModel)];
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
