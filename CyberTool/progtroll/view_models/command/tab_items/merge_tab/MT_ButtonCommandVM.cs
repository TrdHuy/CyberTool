using cyber_base.implement.command;
using cyber_base.view_model;
using progtroll.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.view_models.command.tab_items.merge_tab
{
    internal class MT_ButtonCommandVM: BaseSwPublisherCommandVM
    {
        public CommandExecuterModel RestoreLatestMergeCLButtonCommand { get; set; }
        public CommandExecuterModel CreateMergeCLButtonCommand { get; set; }
        public CommandExecuterModel CheckMergeConflictButtonCommand { get; set; }
        public CommandExecuterModel PushMergeCLButtonCommand { get; set; }

        
        public MT_ButtonCommandVM(BaseViewModel parentsModel, string commandVMTag = "MT_ButtonCommandVM")
            : base(parentsModel, commandVMTag)
        {
            RestoreLatestMergeCLButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_RESTORE_LATEST_MERGE_FEATURE
                        , paramaters);
            });

            CreateMergeCLButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_CREATE_MERGE_CL_AND_COMMIT_FEATURE
                        , paramaters);
            });

            CheckMergeConflictButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_CHECK_MERGE_CONFLICT_FEATURE
                        , paramaters);
            });

            PushMergeCLButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_PUSH_MERGE_COMMIT_FEATURE
                        , paramaters);
            });
        }
    }
}
