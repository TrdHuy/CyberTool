using LogGuard_v0._1.Base.UIEventHandler.Action;
using LogGuard_v0._1.Base.UIEventHandler.Action.Factory;
using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.UIEventHandler
{
    public class LogGuardCommandExecuterFactory : BaseCommandExecuterFactory
    {
        private static Logger logger = new Logger("LogGuardCommandExecuterFactory");

        private static LogGuardCommandExecuterFactory _instance;

        public static LogGuardCommandExecuterFactory Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogGuardCommandExecuterFactory();
                }
                return _instance;
            }
        }

        public override ILogger Logger => logger;

        private LogGuardCommandExecuterFactory()
        {
            RegisterBuilder(WindowTag.WINDOW_TAG_MAIN_SCREEN, new MSW_CommandExecuterBuilder());
        }


        public override IAction CreateAction(string builderID, string keyID, BaseViewModel viewModel = null, ILogger logger = null)
        {
            IAction action = base.CreateAction(builderID, keyID, viewModel, logger);

            return action;
        }


    }
}