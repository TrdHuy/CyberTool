using cyber_base.implement.utils;
using cyber_base.view_model;
using log_guard.implement.flow.run_thread_config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.log_manager.tag_manager
{
    internal class TagManagerUCViewModel : BaseViewModel
    {
        private RangeObservableCollection<TrippleToggleItemViewModel> _tags;

        [Bindable(true)]
        public RangeObservableCollection<TrippleToggleItemViewModel> TagItems
        {
            get
            {
                return _tags;
            }
            set
            {
                _tags = value;
                InvalidateOwn();
            }
        }

        public TagManagerUCViewModel(BaseViewModel baseViewModel) : base(baseViewModel)
        {
            _tags = new RangeObservableCollection<TrippleToggleItemViewModel>();
        }

        public override void OnBegin()
        {
            base.OnBegin();
            var vos = RunThreadConfigManager.Current.TagEmployees;

            if (vos != null)
            {
                foreach (var vo in vos)
                {
                    var tagItemVM = new TagManagerItemViewModel(this, vo);
                    _tags.AddWithoutNotify(tagItemVM);
                }
                _tags.SendNotifications();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RunThreadConfigManager.Current.TagEmployees.Clear();

            if (_tags != null)
            {
                foreach (var vo in _tags)
                {
                    RunThreadConfigManager.Current.TagEmployees.Add(vo.ItemVO);
                }
            }

        }
    }
}
