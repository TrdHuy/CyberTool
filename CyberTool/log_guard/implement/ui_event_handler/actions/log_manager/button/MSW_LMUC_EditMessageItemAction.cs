using cyber_base.utils;
using cyber_base.view_model;
using log_guard.view_models.log_manager.message_manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace log_guard.implement.ui_event_handler.actions.log_manager.button
{
    internal class MSW_LMUC_EditMessageItemAction : LM_ViewModelCommandExecuter
    {
        public MSW_LMUC_EditMessageItemAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger)
            : base(actionID, builderID, viewModel, logger) { }

        string oldText;
        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();

            var messageItemVM = DataTransfer[0] as MessageManagerItemViewModel;
            var editMessageBox = DataTransfer[1] as TextBox;

            if (messageItemVM != null && editMessageBox != null)
            {

                oldText = messageItemVM.Content.ToString();

                messageItemVM.IsEditMode = true;
                editMessageBox.Focus();
                editMessageBox.SelectAll();

                RoutedEventHandler lostFocus = (s, e) =>
                {
                    messageItemVM.IsEditMode = false;
                    var newTag = messageItemVM.Content;
                    if (newTag == "")
                    {
                        messageItemVM.Content = oldText;
                        return;
                    }

                    var contain = LMUCViewModel
                    .MessageManagerContent
                    .Messagetems
                    .FirstOrDefault(item => item.Content == newTag
                        && item != messageItemVM);
                    if (contain == null)
                    {
                        oldText = messageItemVM.Content.ToString();
                        return;
                    }
                    else
                    {
                        LogGuardService
                        .Current?
                        .ServiceManager?
                        .App?
                        .ShowWaringBox("This item already exists in message manager!");
                        messageItemVM.Content = oldText;
                        return;
                    }
                };

                editMessageBox.LostFocus -= lostFocus;
                editMessageBox.LostFocus += lostFocus;
            }
        }


    }
}