﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogGuard_v0._1.Base.Command
{
    public class BaseDotNetCommandImpl : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<object> _act;
        public BaseDotNetCommandImpl(Action<object> act)
        {
            _act = act;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _act?.Invoke(parameter);
        }
    }
}
