using cyber_base.utils;
using cyber_base.view_model;
using log_guard._config;
using log_guard.implement.flow.view_model;
using log_guard.models.vo;
using log_guard.view_models.log_manager.message_manager;
using log_guard.view_models.watcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.implement.ui_event_handler.actions.log_watcher.gesture
{
    internal class MSW_LWI_LogWatcher_MessageDoubleClickAction : LG_ViewModelCommandExecuter
    {
        public MSW_LWI_LogWatcher_MessageDoubleClickAction(string actionID
            , string builderID
            , BaseViewModel viewModel
            , ILogger logger)
            : base(actionID, builderID, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();
            var vm = DataTransfer[0] as LWI_ParseableViewModel;
            if (vm != null)
            {
                var message = vm.Message.ToString();
                var messageManagerVM = ViewModelManager
                    .Current.LogManagerUCViewModel.MessageManagerContent;
                var messItems = messageManagerVM.Messagetems;
                var contain = messItems
                    .FirstOrDefault((item) => item.Content == message);
                if (contain == null)
                {
                    if (messItems.Count < RUNE.MAXIMUM_MESSAGE_ITEM)
                    {
                        var messItemVM = new MessageManagerItemViewModel(messageManagerVM, new TrippleToggleItemVO(message));
                        messItems.Add(messItemVM);
                    }
                    else
                    {
                        LogGuardService
                            .Current?
                            .ServiceManager
                            .App
                            .ShowWaringBox("Message items have reached the maximum!");
                    }
                }
                else
                {
                    LogGuardService
                            .Current?
                            .ServiceManager
                            .App
                            .ShowWaringBox("This item already exists in message manager!");
                }
            }
        }
    }
}