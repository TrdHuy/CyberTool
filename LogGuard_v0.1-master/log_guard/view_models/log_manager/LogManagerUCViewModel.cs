using cyber_base.view_model;
using log_guard.implement.flow.run_thread_config;
using log_guard.view_models.command.log_manager;
using log_guard.view_models.log_manager.message_manager;
using log_guard.view_models.log_manager.tag_manager;
using System.ComponentModel;

namespace log_guard.view_models.log_manager
{
    internal class LogManagerUCViewModel : BaseViewModel
    {
        private TagManagerUCViewModel _tagManagerUCViewModel;
        private MessageManagerUCViewModel _messageManagerUCViewModel;


        [Bindable(true)]
        public LMUC_ButtonCommandVM CommandViewModel { get; set; }

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
            CommandViewModel = new LMUC_ButtonCommandVM(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RunThreadConfigManager.Current.ExportConfig();
        }
    }
}
