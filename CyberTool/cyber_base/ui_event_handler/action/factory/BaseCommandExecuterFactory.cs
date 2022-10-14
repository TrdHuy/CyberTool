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
        public override IAction? CreateAction(string builderID, string keyID, object? dataTransfer, BaseViewModel? viewModel = null, ILogger? logger = null)
        {
            IAction? action = base.CreateAction(builderID, keyID, dataTransfer, viewModel, logger);

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
                            action = commandExecuterBuilder?.BuildCommandExecuter(keyID, dataTransfer, logger);
                        }
                        else
                        {
                            action = commandExecuterBuilder?.BuildViewModelCommandExecuter(keyID, dataTransfer, viewModel, logger);
                        }
                    }
                    else
                    {
                        if (viewModel == null)
                        {
                            action = commandExecuterBuilder?.BuildAlternativeCommandExecuterWhenBuilderIsLock(keyID, dataTransfer, logger);
                        }
                        else
                        {
                            action = commandExecuterBuilder?.BuildAlternativeViewModelCommandExecuterWhenBuilderIsLock(keyID, dataTransfer, viewModel, logger);
                        }
                    }
                }
            }
            return action;
        }
    }
}
