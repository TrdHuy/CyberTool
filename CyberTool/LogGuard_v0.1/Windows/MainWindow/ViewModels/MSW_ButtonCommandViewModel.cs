using LogGuard_v0._1.Base.UIEventHandler.Action.Builder;
using LogGuard_v0._1.Base.UIEventHandler.Action.Executer;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Implement.ViewModels;
using LogGuard_v0._1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels
{
    public class MSW_ButtonCommandViewModel : ButtonCommandViewModel
    {
        private static Logger _logger = new Logger("MSW_ButtonCommandViewModel");

        protected override Logger logger => _logger;

        public MSW_ButtonCommandViewModel(BaseViewModel parentsModel) : base(parentsModel) { }

        protected override ICommandExecuter OnKey(string keyTag, object paramaters, bool isViewModelOnKey = true, string windowTag = WindowTag.WINDOW_TAG_MAIN_SCREEN)
        {
            return base.OnKey(keyTag, paramaters, isViewModelOnKey, windowTag);
        }

        protected override ICommandExecuter OnKey(string keyTag, object paramaters, BuilderLocker locker, bool isViewModelOnKey = true, string windowTag = WindowTag.WINDOW_TAG_MAIN_SCREEN)
        {
            return base.OnKey(keyTag, paramaters, locker, isViewModelOnKey, windowTag);
        }
    }
}
