using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCLogManager
{
    public class MSW_LMUC_ControlButtonCommandVM : MSW_ButtonCommandViewModel
    {
        public CommandExecuterModel DeleteTagItemButtonCommand { get; set; }
        public CommandExecuterModel EditTagItemButtonCommand { get; set; }


        public MSW_LMUC_ControlButtonCommandVM(BaseViewModel parentsModel) : base(parentsModel)
        {
            DeleteTagItemButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(KeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_DELETE_TAG_ITEM_FEATURE
                    , paramaters);
            });

            EditTagItemButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(KeyFeatureTag.KEY_TAG_MSW_LOGMANAGER_EDIT_TAG_ITEM_FEATURE
                    , paramaters);
            });
        }
    }
}
