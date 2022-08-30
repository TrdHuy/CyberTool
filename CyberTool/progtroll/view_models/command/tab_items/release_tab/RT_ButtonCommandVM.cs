﻿using cyber_base.implement.command;
using cyber_base.view_model;
using progtroll.definitions;

namespace progtroll.view_models.command.tab_items.release_tab
{
    internal class RT_ButtonCommandVM : BaseSwPublisherCommandVM
    {
        public CommandExecuterModel RestoreLatestReleaseCLButtonCommand { get; set; }
        public CommandExecuterModel QuickReleaseButtonCommand { get; set; }
        public CommandExecuterModel CreateReleaseCLButtonCommand { get; set; }
        public CommandExecuterModel PushReleaseCLButtonCommand { get; set; }
        public CommandExecuterModel SaveReleaseTemplateButtonCommand { get; set; }

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

            PushReleaseCLButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_PUSH_RELEASE_COMMIT_FEATURE
                    , paramaters);
            });

            SaveReleaseTemplateButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_SAVE_RELEASE_TEMPLATE_FEATURE
                    , paramaters);
            });
        }
    }
}
