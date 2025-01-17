﻿using cyber_base.utils;
using cyber_base.view_model;
using progtroll.view_models.tab_items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.implement.ui_event_handler.actions.release_tab
{
    internal class RT_ViewModelCommandExecuter : BaseViewModelCommandExecuter
    {
        protected ReleaseTabViewModel RTViewModel
        {
            get
            {
                return (ReleaseTabViewModel)ViewModel;
            }
        }

        public RT_ViewModelCommandExecuter(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger)
        {
        }
    }
}
