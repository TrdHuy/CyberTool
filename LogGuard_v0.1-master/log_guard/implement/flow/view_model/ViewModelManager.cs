using cyber_base.implement.utils;
using log_guard.@base.module;
using log_guard.implement.module;
using log_guard.view_models;
using log_guard.view_models.advance_filter;
using log_guard.view_models.log_manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.flow.view_model
{
    internal class ViewModelManager : ILogGuardModule
    {
        public ObservableDictionary<Type, object> DataContextCache { get; }

        public static ViewModelManager Current
        {
            get
            {
                return LogGuardModuleManager.VMM_Instance;
            }
        }

        public LogGuardViewModel LogGuardViewModel
        {
            get
            {
                return (LogGuardViewModel)DataContextCache[typeof(LogGuardViewModel)];
            }
        }
        public AdvanceFilterUCViewModel AdvanceFilterUCViewModel
        {
            get
            {
                return (AdvanceFilterUCViewModel)DataContextCache[typeof(AdvanceFilterUCViewModel)];
            }
        }
        public LogManagerUCViewModel LogManagerUCViewModel
        {
            get
            {
                return (LogManagerUCViewModel)DataContextCache[typeof(LogManagerUCViewModel)];
            }
        }

        public ViewModelManager()
        {
            DataContextCache = new ObservableDictionary<Type, object>();
            DataContextCache.CollectionChanged -= OnContextCollectionsChanged;
            DataContextCache.CollectionChanged += OnContextCollectionsChanged;
        }

        public void OnModuleStart()
        {
        }

        private void OnContextCollectionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
        }


    }
}
