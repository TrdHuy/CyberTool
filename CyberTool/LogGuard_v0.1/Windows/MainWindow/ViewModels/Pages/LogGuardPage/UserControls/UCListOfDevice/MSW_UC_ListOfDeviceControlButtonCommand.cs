using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCListOfDevice
{
    public class MSW_UC_ListOfDeviceControlButtonCommand : MSW_ButtonCommandViewModel
    {
        public CommandExecuterModel RefreshDeviceButtonCommand { get; set; }

        public MSW_UC_ListOfDeviceControlButtonCommand(BaseViewModel parentsModel) : base(parentsModel)
        {
           
            RefreshDeviceButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(KeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_REFRESH_DEVICE_FEATURE
                    , paramaters);
            });
        }
    }
}