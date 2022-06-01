using cyber_base.view_model;
using honeyboard_release_service.utils;
using honeyboard_release_service.view_models.command.project_manager;
using honeyboard_release_service.view_models.project_manager.items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models.project_manager
{
    internal class ProjectManagerViewModel : BaseViewModel
    {
        private FirstLastObservableCollection<VersionHistoryItemViewModel> _versionHistoryItemContexts;
        
        [Bindable(true)]
        public PM_GestureCommandVM GestureCommandVM { get; set; }
        
        [Bindable(true)]
        public FirstLastObservableCollection<VersionHistoryItemViewModel> VersionHistoryItemContexts
        {
            get
            {
                return _versionHistoryItemContexts;
            }
            set
            {
                _versionHistoryItemContexts = value;
                InvalidateOwn();
            }
        }

        public ProjectManagerViewModel(BaseViewModel parents)
        {
            _versionHistoryItemContexts = new FirstLastObservableCollection<VersionHistoryItemViewModel>();
            _versionHistoryItemContexts.FirstChanged -= HandleHistoryItemFirstChanged;
            _versionHistoryItemContexts.FirstChanged += HandleHistoryItemFirstChanged;
            _versionHistoryItemContexts.LastChanged -= HandleHistoryItemLastChanged;
            _versionHistoryItemContexts.LastChanged += HandleHistoryItemLastChanged;

            GestureCommandVM = new PM_GestureCommandVM(this);

            //Test data
            _versionHistoryItemContexts.Add(new VersionHistoryItemViewModel());
            _versionHistoryItemContexts.Add(new VersionHistoryItemViewModel());
            _versionHistoryItemContexts.Add(new VersionHistoryItemViewModel());
            _versionHistoryItemContexts.Add(new VersionHistoryItemViewModel());
        }

        private void HandleHistoryItemLastChanged(object sender, VersionHistoryItemViewModel? oldLast, VersionHistoryItemViewModel? newLast)
        {
            if (oldLast != null)
            {
                oldLast.IsLast = false;
            }

            if (newLast != null)
            {
                newLast.IsLast = true;
            }
        }

        private void HandleHistoryItemFirstChanged(object sender, VersionHistoryItemViewModel? oldFirst, VersionHistoryItemViewModel? newFirst)
        {
            if (oldFirst != null)
            {
                oldFirst.IsFirst = false;
            }

            if (newFirst != null)
            {
                newFirst.IsFirst = true;
            }
        }
    }
}
