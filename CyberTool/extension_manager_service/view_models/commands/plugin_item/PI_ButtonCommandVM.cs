using cyber_base.implement.command;
using cyber_base.view_model;
using extension_manager_service.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.view_models.commands.plugin_item
{
    internal class PI_ButtonCommandVM : BaseEMSCommandViewModel
    {
        public CommandExecuterModel InstallButtonCommand { get; set; }
        public CommandExecuterModel UninstallButtonCommand { get; set; }

        public PI_ButtonCommandVM(BaseViewModel parentsModel
            , string commandVMTag = "PI_ButtonCommandVM") : base(parentsModel, commandVMTag)
        {
            InstallButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(ExtensionManagerKeyFeatureTag.KEY_TAG_EMS_INSTALL_PLUGIN_FEATURE
                    , paramaters);
            });
            UninstallButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(ExtensionManagerKeyFeatureTag.KEY_TAG_EMS_UNINSTALL_PLUGIN_FEATURE
                    , paramaters);
            });
        }
    }
}
