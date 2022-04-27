using LogGuard_v0._1.Base.Command;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Implement.ViewModels;
using LogGuard_v0._1.LogGuard.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager
{
    public class TagItemViewModel : BaseViewModel
    {
        private LogTagVO tagVO;
        private bool _isEditMode;

        public TagItemViewModel(BaseViewModel parents, LogTagVO vo)
        {
            tagVO = vo;
            ViewModelHelper.Current.LogManagerUCViewModelGenerated -= LMUCViewModelGenerated;
            ViewModelHelper.Current.LogManagerUCViewModelGenerated += LMUCViewModelGenerated;
            ShowTagItemCommand = new BaseDotNetCommandImpl((s) =>
            {
                Stat = DotStatus.DotOn;
            });
            RemoveTagItemCommand = new BaseDotNetCommandImpl((s) =>
            {
                Stat = DotStatus.DotOff;
            });
            OffTagItemCommand = new BaseDotNetCommandImpl((s) =>
            {
                Stat = DotStatus.DotNormal;
            });
        }

        private void LMUCViewModelGenerated(object sender, LogManagerUCViewModel vm)
        {
            EditTagItemCommand = vm.CommandViewModel.EditTagItemButtonCommand;
            DeleteTagItemCommand = vm.CommandViewModel.DeleteTagItemButtonCommand;
            
            ViewModelHelper.Current.LogManagerUCViewModelGenerated -= LMUCViewModelGenerated;
        }

        public LogTagVO TagVO
        {
            get
            {
                return tagVO;
            }
        }

        [Bindable(true)]
        public ICommand ShowTagItemCommand { get; set; }

        [Bindable(true)]
        public ICommand RemoveTagItemCommand { get; set; }

        [Bindable(true)]
        public ICommand OffTagItemCommand { get; set; }


        [Bindable(true)]
        public ICommand DeleteTagItemCommand { get; set; }

        [Bindable(true)]
        public ICommand EditTagItemCommand { get; set; }

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
        public string Tag
        {
            get
            {
                return tagVO.Tag;
            }
            set
            {
                tagVO.Tag = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public DotStatus Stat
        {
            get
            {
                switch (tagVO.Stat)
                {
                    case LogTagVO.Status.None:
                        return DotStatus.DotNormal;
                    case LogTagVO.Status.Show:
                        return DotStatus.DotOn;
                    case LogTagVO.Status.Remove:
                        return DotStatus.DotOff;
                    default:
                        return DotStatus.DotNormal;
                }
            }
            set
            {
                switch (value)
                {
                    case DotStatus.DotOn:
                        tagVO.Stat = LogTagVO.Status.Show;
                        break;
                    case DotStatus.DotNormal:
                        tagVO.Stat = LogTagVO.Status.None;
                        break;
                    case DotStatus.DotOff:
                        tagVO.Stat = LogTagVO.Status.Remove;
                        break;
                    default:
                        tagVO.Stat = LogTagVO.Status.None;
                        break;
                }
                InvalidateOwn();
            }
        }

    }
}
