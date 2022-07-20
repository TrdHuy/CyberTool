using cyber_base.implement.utils;
using cyber_base.view_model;
using log_guard.implement.flow.run_thread_config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.log_manager.message_manager
{
    internal class MessageManagerUCViewModel : BaseViewModel
    {
        private RangeObservableCollection<TrippleToggleItemViewModel> _messages;

        [Bindable(true)]
        public RangeObservableCollection<TrippleToggleItemViewModel> Messagetems
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                InvalidateOwn();
            }
        }

        public MessageManagerUCViewModel(BaseViewModel baseViewModel) : base(baseViewModel)
        {
            _messages = new RangeObservableCollection<TrippleToggleItemViewModel>();
        }

        public override void OnBegin()
        {
            base.OnBegin();
            var vos = RunThreadConfigManager.Current.MessageEmployees;

            if (vos != null)
            {
                foreach (var vo in vos)
                {
                    var messageItemVM = new MessageManagerItemViewModel(this, vo);
                    _messages.AddWithoutNotify(messageItemVM);
                }
                _messages.SendNotifications();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RunThreadConfigManager.Current.MessageEmployees.Clear();
            foreach (var vo in _messages)
            {
                RunThreadConfigManager.Current.MessageEmployees.Add(vo.ItemVO);
            }
        }
    }
}
