using cyber_base.implement.command;
using cyber_base.view_model;
using progtroll.definitions;

namespace progtroll.view_models.command.tab_items.release_tab
{
    internal class RT_ButtonCommandVM : BaseSwPublisherCommandVM
    {
        public CommandExecuterImpl RestoreLatestReleaseCLButtonCommand { get; set; }
        public CommandExecuterImpl QuickReleaseButtonCommand { get; set; }
        public CommandExecuterImpl CreateReleaseCLButtonCommand { get; set; }
        public CommandExecuterImpl PushReleaseCLButtonCommand { get; set; }
        public CommandExecuterImpl SaveReleaseTemplateButtonCommand { get; set; }

        public RT_ButtonCommandVM(BaseViewModel parentsModel, string commandVMTag = "RT_ButtonCommandVM")
            : base(parentsModel, commandVMTag)
        {
            QuickReleaseButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_QUICK_RELEASE_FEATURE
                    , paramaters);
            });

            RestoreLatestReleaseCLButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_RESTORE_LATEST_RELEASE_FEATURE
                        , paramaters);
            });

            CreateReleaseCLButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_CREATE_RELEASE_CL_AND_COMMIT_FEATURE
                    , paramaters);
            });

            PushReleaseCLButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_PUSH_RELEASE_COMMIT_FEATURE
                    , paramaters);
            });

            SaveReleaseTemplateButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_SAVE_RELEASE_TEMPLATE_FEATURE
                    , paramaters);
            });
        }
    }
}
