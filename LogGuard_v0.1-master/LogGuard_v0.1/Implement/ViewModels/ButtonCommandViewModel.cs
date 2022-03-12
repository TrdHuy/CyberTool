using LogGuard_v0._1.Base.UIEventHandler.Action.Builder;
using LogGuard_v0._1.Base.UIEventHandler.Action.Executer;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.ViewModels
{


    public abstract class ButtonCommandViewModel : BaseViewModel
    {
        protected abstract Logger logger { get; }

        protected LogGuardKeyActionListener _keyActionListener = LogGuardKeyActionListener.Current;

        protected ButtonCommandViewModel(BaseViewModel parentsModel) : base(parentsModel) { }

        /// <summary>
        /// Get command executer
        /// </summary>
        /// <param name="keyTag"></param>
        /// <param name="paramaters"></param>
        /// <param name="isViewModelOnKey"> True if create a command executer with a view model</param>
        /// <param name="windowTag"></param>
        /// <returns></returns>
        protected virtual ICommandExecuter OnKey(string keyTag, object paramaters, bool isViewModelOnKey, string windowTag)
        {
            logger.I("OnKey: keyTag = " + keyTag + " windowTag = " + windowTag);

#if DEBUG
            Stopwatch onKeyWatcher = Stopwatch.StartNew();
#endif

            var action = _keyActionListener.OnKey(isViewModelOnKey ? ParentsModel : null
                                , logger
                                , windowTag
                                , keyTag
                                , paramaters);

#if DEBUG
            onKeyWatcher.Stop();
            var timeExecuted = onKeyWatcher.ElapsedMilliseconds;
            logger.D("Time executed on key = " + timeExecuted + "(ms)");
#endif
            logger.I("Done: keyTag = " + keyTag + " windowTag = " + windowTag);

            return action as ICommandExecuter;
        }

        /// <summary>
        /// Get command executer and lock builder
        /// </summary>
        /// <param name="keyTag"></param>
        /// <param name="paramaters"></param>
        /// <param name="locker"> Use to lock builder after get command executer</param>
        /// <param name="isViewModelOnKey"> True if create a command executer with a view model</param>
        /// <param name="windowTag"></param>
        /// <returns></returns>
        protected virtual ICommandExecuter OnKey(string keyTag, object paramaters, BuilderLocker locker, bool isViewModelOnKey, string windowTag)
        {
            logger.I("OnKey: keyTag = " + keyTag + " windowTag = " + windowTag);

#if DEBUG
            Stopwatch onKeyWatcher = Stopwatch.StartNew();
#endif

            var action = _keyActionListener.OnKey(isViewModelOnKey ? ParentsModel : null
                                , logger
                                , windowTag
                                , keyTag
                                , paramaters
                                , locker);
#if DEBUG
            onKeyWatcher.Stop();
            var timeExecuted = onKeyWatcher.ElapsedMilliseconds;
            logger.D("Time executed on key = " + timeExecuted + "(ms)");
#endif
            logger.I("Done: keyTag = " + keyTag + " windowTag = " + windowTag);

            return action as ICommandExecuter;
        }
    }
}
