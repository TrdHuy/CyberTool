using LogGuard_v0._1._Config;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.RunThreadConfig;
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
    internal class MSW_LWI_LogWatcher_TagDoubleClickAction : BaseViewModelCommandExecuter
    {
        protected LogGuardPageViewModel LGPViewModel
        {
            get
            {
                return ViewModel as LogGuardPageViewModel;
            }
        }

        public MSW_LWI_LogWatcher_TagDoubleClickAction(string actionID
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
                var tag = vm.Tag.ToString();
                var tagManagerVM = ViewModelHelper.Current.LogManagerUCViewModel.TagManagerContent;
                var tagItems = tagManagerVM.TagItems;
                var contain = tagItems
                    .FirstOrDefault((item)=> item.Tag == tag);
                if(contain == null)
                {
                    if(tagItems.Count < RUNE.MAXIMUM_TAG_ITEM)
                    {
                        tagItems.Add(new TagItemViewModel(tagManagerVM, new LogTagVO(tag)));
                    }
                    else
                    {
                        App.Current.ShowWaringBox("Tag items have reached the maximum!");
                    }
                }
                else
                {
                    App.Current.ShowWaringBox("This item already exists in tag manager!");
                }
            }
        }
    }
}