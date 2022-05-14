using cyber_base.implement.utils;
using cyber_base.ui_event_handler.action.builder;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.ui_event_handler.listener;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.view_model
{
    public abstract class CommandViewModel : BaseViewModel
    {
        protected abstract Logger logger { get; }

        protected abstract IActionListener _keyActionListener { get; }

        protected CommandViewModel(BaseViewModel parentsModel) : base(parentsModel) { }

        /// <summary>
        /// Get command executer
        /// </summary>
        /// <param name="keyTag"></param>
        /// <param name="paramaters"></param>
        /// <param name="isViewModelOnKey"> True if create a command executer with a view model</param>
        /// <param name="builderTag"></param>
        /// <returns></returns>
        protected virtual ICommandExecuter OnKey(string keyTag, object paramaters, bool isViewModelOnKey, string builderTag)
        {
            logger.I("OnKey: keyTag = " + keyTag + " builderTag = " + builderTag);

#if DEBUG
            Stopwatch onKeyWatcher = Stopwatch.StartNew();
#endif

            var action = _keyActionListener.OnKey(isViewModelOnKey ? ParentsModel : null
                                , logger
                                , builderTag
                                , keyTag
                                , paramaters);

#if DEBUG
            onKeyWatcher.Stop();
            var timeExecuted = onKeyWatcher.ElapsedMilliseconds;
            logger.D("Time executed on key = " + timeExecuted + "(ms)");
#endif
            logger.I("Done: keyTag = " + keyTag + " builderTag = " + builderTag);

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
            logger.I("OnKey: keyTag = " + keyTag + " builderTag = " + windowTag);

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
            logger.I("Done: keyTag = " + keyTag + " builderTag = " + windowTag);

            return action as ICommandExecuter;
        }
    }
}
