using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.action.executer
{
    public interface IViewModelCommandExecuter : ICommandExecuter
    {
        BaseViewModel ViewModel { get; }
    }
}
