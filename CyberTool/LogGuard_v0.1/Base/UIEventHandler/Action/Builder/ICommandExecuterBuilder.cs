using LogGuard_v0._1.Base.UIEventHandler.Action.Executer;
using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.UIEventHandler.Action.Builder
{
    public interface ICommandExecuterBuilder : IActionBuilder
    {
        ICommandExecuter BuildCommandExecuter(string keyTag, ILogger logger = null);

        IViewModelCommandExecuter BuildViewModelCommandExecuter(string keyTag, BaseViewModel viewModel, ILogger logger = null);


        ICommandExecuter BuildAlternativeCommandExecuterWhenBuilderIsLock(string keyTag, ILogger logger = null);

        IViewModelCommandExecuter BuildAlternativeViewModelCommandExecuterWhenBuilderIsLock(string keyTag, BaseViewModel viewModel, ILogger logger = null);

    }
}
