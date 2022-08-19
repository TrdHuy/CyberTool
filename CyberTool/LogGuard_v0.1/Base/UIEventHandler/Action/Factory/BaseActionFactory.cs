using LogGuard_v0._1.Base.UIEventHandler.Action.Builder;
using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.UIEventHandler.Action.Factory
{
    public abstract class BaseActionFactory : AbstractActionFactory
    {
        public override IAction CreateAction(string builderID, string keyID, BaseViewModel viewModel = null, ILogger logger = null)
        {

            //try to get the registered builder
            IActionBuilder builder = null;
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
