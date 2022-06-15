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
    public abstract class BaseActionFactory : AbstractActionFactory
    {
        public override IAction? CreateAction(string builderID, string keyID, BaseViewModel? viewModel = null, ILogger? logger = null)
        {

            //try to get the registered builder
            IActionBuilder? builder = null;
            try
            {
                builder = _builders[builderID];
            }
            catch
            {
                return null;
            }

            // Build the action
            if (!builder.Locker.IsLock)
            {
                return builder.BuildMainAction(keyID);
            }
            else
            {
                return builder.BuildAlternativeActionWhenFactoryIsLock(keyID);
            }
        }
    }
}
