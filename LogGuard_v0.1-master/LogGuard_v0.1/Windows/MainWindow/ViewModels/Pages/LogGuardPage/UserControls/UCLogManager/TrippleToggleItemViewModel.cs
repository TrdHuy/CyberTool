using LogGuard_v0._1.Base.Command;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.LogGuardFlow.SourceFilter;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Implement.ViewModels;
using LogGuard_v0._1.LogGuard.Control;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager
{
    public class TrippleToggleItemViewModel : BaseViewModel
    {
        protected TrippleToggleItemVO itemVO;
        private bool _isEditMode;

        public TrippleToggleItemViewModel(BaseViewModel parents, TrippleToggleItemVO vo)
        {
            itemVO = vo;
            
            ShowContentItemCommand = new BaseDotNetCommandImpl((s) =>
            {
                Stat = DotStatus.DotOn;
            });
            RemoveContentItemCommand = new BaseDotNetCommandImpl((s) =>
            {
                Stat = DotStatus.DotOff;
            });
            OffContentItemCommand = new BaseDotNetCommandImpl((s) =>
            {
                Stat = DotStatus.DotNormal;
            });
        }

        public TrippleToggleItemVO ItemVO
        {
            get
            {
                return itemVO;
            }
        }

        [Bindable(true)]
        public ICommand ShowContentItemCommand { get; set; }

        [Bindable(true)]
        public ICommand RemoveContentItemCommand { get; set; }

        [Bindable(true)]
        public ICommand OffContentItemCommand { get; set; }


        [Bindable(true)]
        public ICommand DeleteContentItemCommand { get; set; }

        [Bindable(true)]
        public ICommand EditContentItemCommand { get; set; }

        [Bindable(true)]
        public bool IsEditMode
        {
            get
            {
                return _isEditMode;
            }
            set
            {
                _isEditMode = value;
                InvalidateOwn();
            }
        }


        [Bindable(true)]
        public string Content
        {
            get
            {
                return itemVO.Content;
            }
            set
            {
                itemVO.Content = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public DotStatus Stat
        {
            get
            {
                switch (itemVO.Stat)
                {
                    case TrippleToggleItemVO.Status.None:
                        return DotStatus.DotNormal;
                    case TrippleToggleItemVO.Status.Show:
                        return DotStatus.DotOn;
                    case TrippleToggleItemVO.Status.Remove:
                        return DotStatus.DotOff;
                    default:
                        return DotStatus.DotNormal;
                }
            }
            set
            {
                var oldValue = itemVO.Stat;
                switch (value)
                {
                    case DotStatus.DotOn:
                        itemVO.Stat = TrippleToggleItemVO.Status.Show;
                        break;
                    case DotStatus.DotNormal:
                        itemVO.Stat = TrippleToggleItemVO.Status.None;
                        break;
                    case DotStatus.DotOff:
                        itemVO.Stat = TrippleToggleItemVO.Status.Remove;
                        break;
                    default:
                        itemVO.Stat = TrippleToggleItemVO.Status.None;
                        break;
                }
                if(oldValue != itemVO.Stat)
                {
                    OnTagItemStatChanged(oldValue, itemVO.Stat);
                    InvalidateOwn();
                }
            }
        }

     
        protected virtual void OnTagItemStatChanged(TrippleToggleItemVO.Status oldStat, TrippleToggleItemVO.Status newStat)
        {
            
        }
    }
}
