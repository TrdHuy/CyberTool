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
    interface IActionFactory
    {
        ILogger Logger { get; }

        Dictionary<string, IActionBuilder> Builders { get; }

        IAction CreateAction(string builderID, string keyID, BaseViewModel viewModel = null, ILogger logger = null);

        void RegisterBuilder(string builderID, IActionBuilder builder);

        void LockBuilder(string builderID, bool key, BuilderStatus status);
    }
}
