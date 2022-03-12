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
    public abstract class AbstractActionFactory : IActionFactory
    {
        protected Dictionary<string, IActionBuilder> _builders { get; set; }

        public AbstractActionFactory()
        {
            _builders = new Dictionary<string, IActionBuilder>();
        }

        public Dictionary<string, IActionBuilder> Builders { get => _builders; }

        public abstract ILogger Logger { get; }

        public abstract IAction CreateAction(string builderID, string keyID, BaseViewModel viewModel = null, ILogger logger = null);

        public void LockBuilder(string builderID, bool key, BuilderStatus status)
        {
            try
            {
                if (key)
                    _builders[builderID].LockBuilder(status);
                else
                    _builders[builderID].UnlockBuilder(status);
            }
            catch (Exception e)
            {
                Logger.E(e.Message);
                return;
            }
        }

        public void RegisterBuilder(string builderID, IActionBuilder builder)
        {
            if (!_builders.ContainsKey(builderID))
            {
                _builders.Add(builderID, builder);
            }
        }
    }
}
