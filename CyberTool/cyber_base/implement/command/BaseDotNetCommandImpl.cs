using System;
using System.Windows.Input;

namespace cyber_base.implement.command
{
    public class BaseDotNetCommandImpl : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private Action<object?> _act;
        public BaseDotNetCommandImpl(Action<object?> act)
        {
            _act = act;
        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _act?.Invoke(parameter);
        }
    }
}
