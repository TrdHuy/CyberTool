using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.RunThreadConfig;
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
        private MessageManagerUCViewModel _messageManagerUCViewModel;


        [Bindable(true)]
        public MSW_LMUC_ControlButtonCommandVM CommandViewModel { get; set; }

        [Bindable(true)]
        public MessageManagerUCViewModel MessageManagerContent
        {
            get
            {
                return _messageManagerUCViewModel;
            }
            set
            {
                _messageManagerUCViewModel = value;
            }
        }

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
            MessageManagerContent = new MessageManagerUCViewModel(this);
            CommandViewModel = new MSW_LMUC_ControlButtonCommandVM(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RunThreadConfigManager.Current.ExportConfig();
        }
    }
}
