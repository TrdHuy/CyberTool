using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LogGuard_v0._1.Windows.MainWindow.Action.Types.LogManager
{
    public class MSW_LMUC_EditMessageItemAction : BaseViewModelCommandExecuter
    {
        protected LogManagerUCViewModel LMUCViewModel
        {
            get
            {
                return ViewModel as LogManagerUCViewModel;
            }
        }

        public MSW_LMUC_EditMessageItemAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger) : base(actionID, builderID, viewModel, logger) { }

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
                        App.Current.ShowWaringBox("This item already exists in message manager!");
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