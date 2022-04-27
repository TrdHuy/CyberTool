using LogGuard_v0._1.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager
{
    public class LogManagerUCViewModel : BaseViewModel
    {
        private TagManagerUCViewModel _tagManagerUCViewModel;


        [Bindable(true)]
        public MSW_LMUC_ControlButtonCommandVM CommandViewModel { get; set; }

        [Bindable(true)]
        public TagManagerUCViewModel TagManagerContent
        {
            get
            {
                return _tagManagerUCViewModel;
            }
            set
            {
                _tagManagerUCViewModel = value;
            }
        }

        public LogManagerUCViewModel()
        {
        }

        public LogManagerUCViewModel(BaseViewModel baseViewModel) : base(baseViewModel)
        {
            TagManagerContent = new TagManagerUCViewModel(this);
            CommandViewModel = new MSW_LMUC_ControlButtonCommandVM(this);
        }
    }
}
