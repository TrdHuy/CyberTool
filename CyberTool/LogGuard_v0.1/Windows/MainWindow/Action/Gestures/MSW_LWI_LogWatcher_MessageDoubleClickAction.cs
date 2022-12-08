using LogGuard_v0._1._Config;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Implement.ViewModels;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.Action.Gestures
{
    public class MSW_LWI_LogWatcher_MessageDoubleClickAction : BaseViewModelCommandExecuter
    {
        protected LogGuardPageViewModel LGPViewModel
        {
            get
            {
                return ViewModel as LogGuardPageViewModel;
            }
        }

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
                var messageManagerVM = ViewModelHelper.Current.LogManagerUCViewModel.MessageManagerContent;
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
                        App.Current.ShowWaringBox("Message items have reached the maximum!");
                    }
                }
                else
                {
                    App.Current.ShowWaringBox("This item already exists in message manager!");
                }
            }
        }
    }
}