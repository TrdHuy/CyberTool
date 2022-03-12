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
    public abstract class AbstractCommandExecuterBuilder : AbstractActionBuilder, ICommandExecuterBuilder
    {
        public override IAction BuildMainAction(string keyTag)
        {
            return BuildCommandExecuter(keyTag);
        }

        public override IAction BuildAlternativeActionWhenFactoryIsLock(string keyTag)
        {
            return BuildAlternativeCommandExecuterWhenBuilderIsLock(keyTag);
        }

        public abstract ICommandExecuter BuildAlternativeCommandExecuterWhenBuilderIsLock(string keyTag, ILogger logger = null);
        public abstract IViewModelCommandExecuter BuildAlternativeViewModelCommandExecuterWhenBuilderIsLock(string keyTag, BaseViewModel viewModel, ILogger logger = null);

        public abstract ICommandExecuter BuildCommandExecuter(string keyTag, ILogger logger = null);
        public abstract IViewModelCommandExecuter BuildViewModelCommandExecuter(string keyTag, BaseViewModel viewModel, ILogger logger = null);


    }
}
