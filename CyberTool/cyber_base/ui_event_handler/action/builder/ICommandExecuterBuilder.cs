using cyber_base.ui_event_handler.action.executer;
using cyber_base.utils;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.action.builder
{
    public interface ICommandExecuterBuilder : IActionBuilder
    {
        ICommandExecuter? BuildCommandExecuter(string keyTag, ILogger? logger = null);

        IViewModelCommandExecuter? BuildViewModelCommandExecuter(string keyTag, BaseViewModel viewModel, ILogger? logger = null);


        ICommandExecuter? BuildAlternativeCommandExecuterWhenBuilderIsLock(string keyTag, ILogger? logger = null);

        IViewModelCommandExecuter? BuildAlternativeViewModelCommandExecuterWhenBuilderIsLock(string keyTag, BaseViewModel viewModel, ILogger? logger = null);

    }
}
