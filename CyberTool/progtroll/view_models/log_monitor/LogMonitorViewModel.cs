using cyber_base.view_model;
using progtroll.implement.log_manager;
using progtroll.view_models.command.log_monitor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.view_models.log_monitor
{
    internal class LogMonitorViewModel : BaseViewModel
    {
        [Bindable(true)]
        public LM_ButtonCommandVM ButtonCommandVM { get; set; }

        [Bindable(true)]
        public string LogContent
        {
            get
            {
                return LogManager.Current.LogContent;
            }
        }

        public LogMonitorViewModel(BaseViewModel parents) : base(parents)
        {
            ButtonCommandVM = new LM_ButtonCommandVM(this);
            LogManager.Current.LogContentChanged -= OnLogContentChanged;
            LogManager.Current.LogContentChanged += OnLogContentChanged;
        }

        private void OnLogContentChanged(object sender)
        {
            Invalidate("LogContent");
        }
    }
}
