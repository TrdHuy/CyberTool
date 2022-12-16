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
        public CommandExecuterImpl DeleteTagItemButtonCommand { get; set; }
        public CommandExecuterImpl EditTagItemButtonCommand { get; set; }

        public CommandExecuterImpl DeleteMessageItemButtonCommand { get; set; }
        public CommandExecuterImpl EditMessageItemButtonCommand { get; set; }

        public LMUC_ButtonCommandVM(BaseViewModel parentsModel) : base(parentsModel)
        {
            DeleteTagItemButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_DELETE_TAG_ITEM_FEATURE
                    , paramaters);
            });

            EditTagItemButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_EDIT_TAG_ITEM_FEATURE
                    , paramaters);
            });

            DeleteMessageItemButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_DELETE_MESSAGE_ITEM_FEATURE
                    , paramaters);
            });

            EditMessageItemButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_EDIT_MESSAGE_ITEM_FEATURE
                    , paramaters);
            });
        }
    }
}
