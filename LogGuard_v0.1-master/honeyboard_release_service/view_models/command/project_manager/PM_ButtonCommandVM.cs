using cyber_base.implement.command;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models.command.project_manager
{
    internal class PM_ButtonCommandVM : BaseSwPublisherCommandVM
    {

        public PM_ButtonCommandVM(BaseViewModel parentsModel, string commandVMTag = "PM_ButtonCommandVM") 
            : base(parentsModel, commandVMTag)
        {
        }
    }
}
