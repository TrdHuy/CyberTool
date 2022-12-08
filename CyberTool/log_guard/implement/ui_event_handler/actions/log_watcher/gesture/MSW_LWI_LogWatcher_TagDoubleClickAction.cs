using cyber_base.utils;
using cyber_base.view_model;
using log_guard._config;
using log_guard.implement.flow.view_model;
using log_guard.models.vo;
using log_guard.view_models.log_manager.tag_manager;
using log_guard.view_models.watcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.ui_event_handler.actions.log_watcher.gesture
{
    internal class MSW_LWI_LogWatcher_TagDoubleClickAction : LG_ViewModelCommandExecuter
    {
        public MSW_LWI_LogWatcher_TagDoubleClickAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var vm = DataTransfer[0] as LWI_ParseableViewModel;
            if (vm != null)
            {
                var tag = vm.Tag.ToString();
                var tagManagerVM = ViewModelManager.Current.LogManagerUCViewModel.TagManagerContent;
                var tagItems = tagManagerVM.TagItems;
                var contain = tagItems
                    .FirstOrDefault((item) => item.Content == tag);
                if (contain == null)
                {
                    if (tagItems.Count < RUNE.MAXIMUM_TAG_ITEM)
                    {
                        var tagItemVM = new TagManagerItemViewModel(tagManagerVM, new TrippleToggleItemVO(tag));
                        tagItems.Add(tagItemVM);
                    }
                    else
                    {
                        LogGuardService
                            .Current
                            .ServiceManager
                            .App
                            .ShowWaringBox("Tag items have reached the maximum!");
                    }
                }
                else
                {
                    LogGuardService
                            .Current
                            .ServiceManager
                            .App
                            .ShowWaringBox("This item already exists in tag manager!");
                }
            }
        }
    }
}