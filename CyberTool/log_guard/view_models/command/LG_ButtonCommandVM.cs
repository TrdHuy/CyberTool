using cyber_base.view_model;
using log_guard.definitions;
using cyber_base.implement.command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.command
{
    internal class LG_ButtonCommandVM : BaseLogGuardCommandVM
    {
        public CommandExecuterImpl PlayButtonCommand { get; set; }
        public CommandExecuterImpl StopButtonCommand { get; set; }
        public CommandExecuterImpl ClearButtonCommand { get; set; }
        public CommandExecuterImpl LWCtrlAGestureCommand { get; set; }
        public CommandExecuterImpl LWDeleteGestureCommand { get; set; }
        public CommandExecuterImpl ZoomButtonCommand { get; set; }
        public CommandExecuterImpl ImportLogFileButtonCommand { get; set; }

        public LG_ButtonCommandVM(BaseViewModel parentsModel) : base(parentsModel)
        {
            PlayButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_PLAY_FEATURE
                    , paramaters);
            });
            StopButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_STOP_FEATURE
                    , paramaters);
            });

            ClearButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_CLEAR_FEATURE
                    , paramaters);
            });

            LWCtrlAGestureCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_CTRLA_GESTURE_FEATURE
                   , paramaters);
            });

            LWDeleteGestureCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_DELETE_GESTURE_FEATURE
                   , paramaters);
            });

            ZoomButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_ZOOM_FEATURE
                    , paramaters);
            });

            ImportLogFileButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_IMPORT_LOG_FILE_FEATURE
                    , paramaters);
            });

        }
    }
}
