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
    internal class LG_GestureCommandVM : BaseLogGuardCommandVM
    {
        public CommandExecuterModel LogTagDoubleClickCommand { get; set; }
        public CommandExecuterModel LogMessageDoubleClickCommand { get; set; }
        public CommandExecuterModel ParserFormatSelectedCommand { get; set; }

        public LG_GestureCommandVM(BaseViewModel parentsModel) : base(parentsModel)
        {
            LogTagDoubleClickCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_TAG_DOUBLE_CLICK_GESTURE_FEATURE
                    , paramaters);
            });

            LogMessageDoubleClickCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_MESSAGE_DOUBLE_CLICK_GESTURE_FEATURE
                    , paramaters);
            });

            ParserFormatSelectedCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_PARSER_ITEM_SELECTED_GESTURE_FEATURE
                    , paramaters);
            });
        }
    }
}
