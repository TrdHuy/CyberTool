﻿using cyber_base.implement.command;
using cyber_base.view_model;
using progtroll.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.view_models.command.project_manager
{
    internal class PM_ButtonCommandVM : BaseSwPublisherCommandVM
    {
        public CommandExecuterImpl FetchPrjectButtonCommand { get; set; }

        public PM_ButtonCommandVM(BaseViewModel parentsModel, string commandVMTag = "PM_ButtonCommandVM") 
            : base(parentsModel, commandVMTag)
        {
            FetchPrjectButtonCommand = new CommandExecuterImpl((paramaters) =>
            {
                return OnKey(PublisherKeyFeatureTag.KEY_TAG_PRT_PM_FETCH_PROJECT_FEATURE
                    , paramaters);
            });
        }
    }
}
