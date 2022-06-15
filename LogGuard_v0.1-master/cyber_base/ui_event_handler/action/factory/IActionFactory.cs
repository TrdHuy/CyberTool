using cyber_base.ui_event_handler.action.builder;
using cyber_base.utils;
using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.action.factory
{
    interface IActionFactory
    {
        ILogger Logger { get; }

        Dictionary<string, IActionBuilder> Builders { get; }

        IAction? CreateAction(string builderID, string keyID, BaseViewModel? viewModel = null, ILogger? logger = null);

        void RegisterBuilder(string builderID, IActionBuilder builder);

        void LockBuilder(string builderID, bool key, BuilderStatus status);
    }
}
