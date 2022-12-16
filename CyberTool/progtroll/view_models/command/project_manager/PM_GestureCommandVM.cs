using cyber_base.implement.command;
using cyber_base.view_model;
using progtroll.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.view_models.command.project_manager
{
    internal class PM_GestureCommandVM : BaseSwPublisherCommandVM
    {
        public CommandExecuterImpl PathSelectedGestureCommand { get; set; }
        public CommandExecuterImpl SelectedBranchChangedCommand { get; set; }
        public CommandExecuterImpl VersionFilePathSelectedGestureCommand { get; set; }

        public PM_GestureCommandVM(BaseViewModel parentsModel, string commandVMTag = "PM_GestureCommandVM")
            : base(parentsModel, commandVMTag)
        {
            PathSelectedGestureCommand = new  CommandExecuterImpl((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_PM_PROJECT_PATH_SELECTED_FEATURE
                    , paramaters);
            });

            SelectedBranchChangedCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_PM_SELECTED_BRANCH_CHANGED_FEATURE
                    , paramaters);
            });

            VersionFilePathSelectedGestureCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_PM_VERSION_FILE_PATH_SELECTED_FEATUER
                    , paramaters);
            });
        }
    }
}
