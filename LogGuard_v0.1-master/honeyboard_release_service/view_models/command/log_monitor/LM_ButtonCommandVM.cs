using cyber_base.implement.command;
using cyber_base.view_model;
using honeyboard_release_service.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models.command.log_monitor
{
    internal class LM_ButtonCommandVM : BaseSwPublisherCommandVM
    {
        public CommandExecuterModel ClearLogButtonCommand { get; set; }
        public CommandExecuterModel ClipboardButtonCommand { get; set; }
        
        public LM_ButtonCommandVM(BaseViewModel parentsModel, string commandVMTag = "LM_ButtonCommandVM")
            : base(parentsModel, commandVMTag)
        {
            ClearLogButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_LM_CLEAR_LOG_CONTENT_FEATURE
                    , paramaters);
            });
            ClipboardButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_LM_CLIPBOARD_LOG_CONTENT_FEATURE
                    , paramaters);
            });
        }
    }
}
