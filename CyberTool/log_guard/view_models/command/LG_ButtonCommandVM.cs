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
        public CommandExecuterModel PlayButtonCommand { get; set; }
        public CommandExecuterModel StopButtonCommand { get; set; }
        public CommandExecuterModel ClearButtonCommand { get; set; }
        public CommandExecuterModel LWCtrlAGestureCommand { get; set; }
        public CommandExecuterModel LWDeleteGestureCommand { get; set; }
        public CommandExecuterModel ZoomButtonCommand { get; set; }
        public CommandExecuterModel ImportLogFileButtonCommand { get; set; }

        public LG_ButtonCommandVM(BaseViewModel parentsModel) : base(parentsModel)
        {
            PlayButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_PLAY_FEATURE
                    , paramaters);
            });
            StopButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_STOP_FEATURE
                    , paramaters);
            });

            ClearButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_CLEAR_FEATURE
                    , paramaters);
            });

            LWCtrlAGestureCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_CTRLA_GESTURE_FEATURE
                   , paramaters);
            });

            LWDeleteGestureCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_DELETE_GESTURE_FEATURE
                   , paramaters);
            });

            ZoomButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_ZOOM_FEATURE
                    , paramaters);
            });

            ImportLogFileButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_IMPORT_LOG_FILE_FEATURE
                    , paramaters);
            });

        }
    }
}
