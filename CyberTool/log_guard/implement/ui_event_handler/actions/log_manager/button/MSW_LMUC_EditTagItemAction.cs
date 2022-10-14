using cyber_base.utils;
using cyber_base.view_model;
using log_guard.view_models.log_manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace log_guard.implement.ui_event_handler.actions.log_manager.button
{
    internal class MSW_LMUC_EditTagItemAction : LM_ViewModelCommandExecuter
    {
        public MSW_LMUC_EditTagItemAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger) { }

        string a = "";
        string oldText
        {
            get
            {
                return a;
            }
            set
            {
                a = value;
            }
        }
        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();

            var tagItemVM = DataTransfer[0] as TrippleToggleItemViewModel;
            var editTagBox = DataTransfer[1] as TextBox;

            if (tagItemVM != null && editTagBox != null)
            {

                oldText = tagItemVM.Content.ToString();

                tagItemVM.IsEditMode = true;
                editTagBox.Focus();
                editTagBox.SelectAll();

                RoutedEventHandler lostFocus = (s, e) =>
                {
                    tagItemVM.IsEditMode = false;
                    var newTag = tagItemVM.Content;
                    if (newTag == "")
                    {
                        tagItemVM.Content = oldText;
                        return;
                    }

                    var contain = LMUCViewModel.TagManagerContent.TagItems.FirstOrDefault(item => item.Content == newTag && item != tagItemVM);
                    if (contain == null)
                    {
                        oldText = tagItemVM.Content.ToString();
                        return;
                    }
                    else
                    {
                        LogGuardService
                        .Current
                        .ServiceManager
                        .App
                        .ShowWaringBox("This item already exists in tag manager!");
                        tagItemVM.Content = oldText;
                        return;
                    }
                };

                editTagBox.LostFocus -= lostFocus;
                editTagBox.LostFocus += lostFocus;
            }
        }


    }
}