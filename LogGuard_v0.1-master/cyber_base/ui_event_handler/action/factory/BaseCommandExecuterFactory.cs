using cyber_base.ui_event_handler.action.builder;
using cyber_base.utils;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.action.factory
{
    public abstract class BaseCommandExecuterFactory : BaseActionFactory
    {
        public override IAction? CreateAction(string builderID, string keyID, BaseViewModel? viewModel = null, ILogger? logger = null)
        {
            IAction? action = base.CreateAction(builderID, keyID, viewModel, logger);

            if (action == null)
            {

                IActionBuilder? builder = null;
                try
                {
                    builder = _builders[builderID];
                }
                catch
                {
                    return null;
                }

                if (builder is ICommandExecuterBuilder)
                {
                    var commandExecuterBuilder = builder as ICommandExecuterBuilder;

                    if (!commandExecuterBuilder?.Locker.IsLock ?? false)
                    {
                        if (viewModel == null)
                        {
                            action = commandExecuterBuilder?.BuildCommandExecuter(keyID, logger);
                        }
                        else
                        {
                            action = commandExecuterBuilder?.BuildViewModelCommandExecuter(keyID, viewModel, logger);
                        }
                    }
                    else
                    {
                        if (viewModel == null)
                        {
                            action = commandExecuterBuilder?.BuildAlternativeCommandExecuterWhenBuilderIsLock(keyID, logger);
                        }
                        else
                        {
                            action = commandExecuterBuilder?.BuildAlternativeViewModelCommandExecuterWhenBuilderIsLock(keyID, viewModel, logger);
                        }
                    }
                }
            }
            return action;
        }
    }
}
