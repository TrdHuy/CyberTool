using cyber_base.utils;
using cyber_base.view_model;
using progtroll.implement.project_manager;
using progtroll.implement.user_data_manager;
using progtroll.view_models.tab_items;

namespace progtroll.implement.ui_event_handler.actions.release_tab.button
{
    internal class PRT_RT_SaveReleaseTemplateAction : RT_ViewModelCommandExecuter
    {
        public PRT_RT_SaveReleaseTemplateAction(string actionID, string builderID, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, viewModel, logger)
        {
        }

        protected override bool CanExecute(object? dataTransfer)
        {
            if (string.IsNullOrEmpty(RTViewModel.TaskID))
            {
                ProgTroll
                    .Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("Task ID can't be empty");

                return false;
            }

            if (string.IsNullOrEmpty(RTViewModel.CommitTitle))
            {
                ProgTroll
                    .Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("Commit Title can't be empty");

                return false;
            }

            return base.CanExecute(dataTransfer);
        }

        protected override void ExecuteCommand()
        {
            var displayName = ProgTroll
                    .Current
                    .ServiceManager?
                    .App
                    .OpenEditTextDialogWindow("");

            if (string.IsNullOrEmpty(displayName))
            {
                ProgTroll
                    .Current
                    .ServiceManager?
                    .App
                    .ShowWaringBox("You must enter the name for template");

                return;
            }

            var template = new ReleaseTemplateItemViewModel()
            {
                DisplayName = displayName,
                TaskID = RTViewModel.TaskID,
                CommitTitle = RTViewModel.CommitTitle
            };

            ReleasingProjectManager.Current.AddReleaseTemplateViewModelItem(template);
        }
    }
}
