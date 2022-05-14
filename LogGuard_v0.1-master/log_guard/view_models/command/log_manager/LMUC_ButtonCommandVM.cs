using cyber_base.view_model;
using log_guard.definitions;
using cyber_base.implement.command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.command.log_manager
{
    internal class LMUC_ButtonCommandVM : BaseLogGuardCommandVM
    {
        public CommandExecuterModel DeleteTagItemButtonCommand { get; set; }
        public CommandExecuterModel EditTagItemButtonCommand { get; set; }

        public CommandExecuterModel DeleteMessageItemButtonCommand { get; set; }
        public CommandExecuterModel EditMessageItemButtonCommand { get; set; }

        public LMUC_ButtonCommandVM(BaseViewModel parentsModel) : base(parentsModel)
        {
            DeleteTagItemButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_DELETE_TAG_ITEM_FEATURE
                    , paramaters);
            });

            EditTagItemButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_EDIT_TAG_ITEM_FEATURE
                    , paramaters);
            });

            DeleteMessageItemButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_DELETE_MESSAGE_ITEM_FEATURE
                    , paramaters);
            });

            EditMessageItemButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_EDIT_MESSAGE_ITEM_FEATURE
                    , paramaters);
            });
        }
    }
}
