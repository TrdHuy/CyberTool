using cyber_base.implement.utils;
using honeyboard_release_service.@base.module;
using honeyboard_release_service.implement.module;
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

        public static ViewModelManager Current
        {
            get
            {
                return PublisherModuleManager.VMM_Instance;
            }
        }

        public ViewModelManager()
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
