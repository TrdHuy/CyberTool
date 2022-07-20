using cyber_base.implement.command;
using cyber_base.view_model;
using honeyboard_release_service.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models.command.tab_items.release_tab
{
    internal class RT_ButtonCommandVM : BaseSwPublisherCommandVM
    {
        public CommandExecuterModel RestoreLatestReleaseCLButtonCommand { get; set; }
        public CommandExecuterModel QuickReleaseButtonCommand { get; set; }
        public CommandExecuterModel CreateReleaseCLButtonCommand { get; set; }
        
        public RT_ButtonCommandVM(BaseViewModel parentsModel, string commandVMTag = "RT_ButtonCommandVM")
            : base(parentsModel, commandVMTag)
        {
            QuickReleaseButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_QUICK_RELEASE_FEATURE
                    , paramaters);
            });

            RestoreLatestReleaseCLButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_RESTORE_LATEST_RELEASE_FEATURE
                        , paramaters);
            });

            CreateReleaseCLButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_CREATE_RELEASE_CL_AND_COMMIT_FEATURE
                    , paramaters);
            });
        }
    }
}
