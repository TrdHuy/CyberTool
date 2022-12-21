using cyber_base.ui_event_handler.action.executer;
using cyber_base.ui_event_handler.listener;
using cyber_base.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace cyber_base.implement.command
{
    public class CommandExecuterImpl : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private Func<object?, ICommandExecuter?> _action;
        private ICommandExecuter? _commandExecuterCache;
        private bool _isAsync;

        protected ActionExecuteHelper ActionExecuteHelper { get; set; }

        protected virtual ICommandExecuter? CommandExecuterCache
        {
            get
            {
                return _commandExecuterCache;
            }
            set
            {
                _commandExecuterCache = value;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return CommandExecuterCache == null ? throw new NullReferenceException("Current cache is null") : CommandExecuterCache.IsCompleted;
            }
        }

        public bool IsCaneled
        {
            get
            {
                return CommandExecuterCache == null ? throw new NullReferenceException("Current cache is null") : CommandExecuterCache.IsCanceled;
            }
        }

        public CommandExecuterImpl(Func<object?, ICommandExecuter?> hpssAction, bool isAsync = false)
        {
            _isAsync = isAsync;
            _action = hpssAction;
            ActionExecuteHelper = ActionExecuteHelper.Current;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            CommandExecuterCache = _action?.Invoke(parameter);

            if (_isAsync)
            {
                await ExetcuteActionAsync(parameter);
            }
            else
            {
                ExetcuteAction(parameter);
            }
        }

        public void Cancel()
        {
            if (CommandExecuterCache != null)
            {
                CommandExecuterCache.OnCancel();
            }
        }

        protected virtual void ExetcuteAction(object? dataTransfer)
        {
            if (CommandExecuterCache == null)
            {
                return;
            }
            ActionExecuteHelper.ExecuteAction(CommandExecuterCache, dataTransfer);
        }

        protected virtual async Task ExetcuteActionAsync(object? dataTransfer)
        {
            if (CommandExecuterCache == null)
            {
                return;
            }
            await ActionExecuteHelper.ExecuteActionAsync(CommandExecuterCache, dataTransfer);
        }
    }
}
