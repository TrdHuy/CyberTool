using cyber_base.implement.command;
using cyber_base.view_model;
using log_guard.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.command.device
{
    internal class LOF_ButtonCommand : BaseLogGuardCommandVM
    {
        public CommandExecuterModel RefreshDeviceButtonCommand { get; set; }


        public LOF_ButtonCommand(BaseViewModel parentsModel) : base(parentsModel)
        {
            RefreshDeviceButtonCommand = new CommandExecuterModel((paramaters) =>
            {
                return OnKey(LogGuardKeyFeatureTag.KEY_TAG_MSW_LOGWATCHER_REFRESH_DEVICE_FEATURE
                    , paramaters);
            });
        }
    }
}
