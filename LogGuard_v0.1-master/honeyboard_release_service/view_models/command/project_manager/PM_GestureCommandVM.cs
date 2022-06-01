using cyber_base.implement.command;
using cyber_base.view_model;
using honeyboard_release_service.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models.command.project_manager
{
    internal class PM_GestureCommandVM : BaseSwPublisherCommandVM
    {
        public CommandExecuterModel PathSelectedGestureCommand { get; set; }

        public PM_GestureCommandVM(BaseViewModel parentsModel, string commandVMTag = "PM_ButtonCommandVM")
            : base(parentsModel, commandVMTag)
        {
            PathSelectedGestureCommand = new  CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_PM_PROJECT_PATH_SELECTED_FEATURE
                    , paramaters);
            });
        }
    }
}
