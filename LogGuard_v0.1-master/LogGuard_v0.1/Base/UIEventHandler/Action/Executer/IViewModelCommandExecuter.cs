﻿using LogGuard_v0._1.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.UIEventHandler.Action.Executer
{
    public interface IViewModelCommandExecuter : ICommandExecuter
    {
        BaseViewModel ViewModel { get; }
    }
}