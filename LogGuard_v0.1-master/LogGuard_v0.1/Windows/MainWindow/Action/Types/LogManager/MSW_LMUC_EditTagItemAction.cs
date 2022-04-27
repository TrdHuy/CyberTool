using LogGuard_v0._1.AppResources.AttachedProperties;
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
    internal class MSW_LMUC_EditTagItemAction : BaseViewModelCommandExecuter
    {
        protected LogManagerUCViewModel LMUCViewModel
        {
            get
            {
                return ViewModel as LogManagerUCViewModel;
            }
        }

        public MSW_LMUC_EditTagItemAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger) : base(actionID, builderID, viewModel, logger) { }

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

            var tagItemVM = DataTransfer[0] as TagItemViewModel;
            var editTagBox = DataTransfer[1] as TextBox;

            if (tagItemVM != null && editTagBox != null)
            {

                oldText = tagItemVM.Tag.ToString();

                tagItemVM.IsEditMode = true;
                editTagBox.Focus();
                editTagBox.SelectAll();

                RoutedEventHandler lostFocus = (s, e) =>
                {
                    tagItemVM.IsEditMode = false;
                    var newTag = tagItemVM.Tag;
                    if (newTag == "")
                    {
                        tagItemVM.Tag = oldText;
                        return;
                    }

                    var contain = LMUCViewModel.TagManagerContent.TagItems.FirstOrDefault(item => item.Tag == newTag && item != tagItemVM);
                    if (contain == null)
                    {
                        oldText = tagItemVM.Tag.ToString();
                        return;
                    }
                    else
                    {
                        App.Current.ShowWaringBox("This item already exists in tag manager!");
                        tagItemVM.Tag = oldText;
                        return;
                    }
                };

                editTagBox.LostFocus -= lostFocus;
                editTagBox.LostFocus += lostFocus;
            }
        }


    }
}