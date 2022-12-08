using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.RunThreadConfig;
using LogGuard_v0._1.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager
{
    public class TagManagerUCViewModel : BaseViewModel
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
            var vos = RunThreadConfigManager.Current.TagEmployees;
            _tags = new RangeObservableCollection<TrippleToggleItemViewModel>();


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
            foreach (var vo in _tags)
            {
                RunThreadConfigManager.Current.TagEmployees.Add(vo.ItemVO);
            }
        }
    }
}
