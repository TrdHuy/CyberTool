using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.RunThreadConfig;
using LogGuard_v0._1.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager
{
    public class MessageManagerUCViewModel : BaseViewModel
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
            var vos = RunThreadConfigManager.Current.MessageEmployees;
            _messages = new RangeObservableCollection<TrippleToggleItemViewModel>();

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
