﻿using cyber_base.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.action.executer
{
    public abstract class AbstractCommandExecuter : ICommandExecuter
    {
        private bool _isCompleted = false;
        private bool _isCanceled = false;
        private List<object>? _dataTransfer;
        private string _actionID = "";
        private string _actionName = "";
        private string _builderID = "";

        public event NotifyIsCanceledChangedHandler? IsCanceledChanged;
        public event NotifyIsCompletedChangedHandler? IsCompletedChanged;

        public ILogger? Logger { get; private set; }

        public bool IsCompleted
        {
            get => _isCompleted;
            protected set
            {
                var oldValue = _isCompleted;
                _isCompleted = value;
                if (_isCompleted)
                {
                    ClearCache();
                }
                if (oldValue != value)
                    IsCompletedChanged?.Invoke(this, new ActionStatusArgs(value, oldValue));
            }
        }

        public bool IsCanceled
        {
            get => _isCanceled;
            protected set
            {
                var oldValue = _isCanceled;
                _isCanceled = value;
                if (_isCanceled)
                {
                    ClearCache();
                }
                if (oldValue != value)
                    IsCanceledChanged?.Invoke(this, new ActionStatusArgs(value, oldValue));

            }
        }

        public IList<object>? DataTransfer { get => _dataTransfer; }

        public string ActionID { get => _actionID; }
        public string BuilderID { get => _builderID; }
        public string ActionName { get => _actionName; }

        public AbstractCommandExecuter(string actionID, string builderID, object? dataTransfer, ILogger? logger)
        {
            this.Logger = logger;
            _actionID = actionID;
            _builderID = builderID;

            if (dataTransfer != null)
                AssignDataTransfer(dataTransfer);
        }

        public AbstractCommandExecuter(string actionName, string actionID, string builderID, object? dataTransfer, ILogger? logger)
        {
            this.Logger = logger;
            _actionID = actionID;
            _builderID = builderID;
            _actionName = actionName;

            if (dataTransfer != null)
                AssignDataTransfer(dataTransfer);
        }

        public void OnDestroy()
        {
            ClearCache();
            ExecuteOnDestroy();
        }

        public void OnCancel()
        {
            if (!IsCompleted)
            {
                IsCanceled = true;
                ExecuteOnCancel();
            }
        }

        public async Task<bool> ExecuteAsync(object? dataTransfer)
        {
            bool isExecuteable = false;

            if (CanExecute(dataTransfer))
            {
                isExecuteable = true;
                //Execute the command
                await ExecuteCommandAsync();
            }
            else
            {
                isExecuteable = false;
            }

            SetCompleteFlagAfterExecuteCommand();
            return isExecuteable;
        }

        public bool Execute(object? dataTransfer)
        {
            bool isExecuteable = false;

            if (CanExecute(dataTransfer))
            {
                isExecuteable = true;
                //Execute the command
                ExecuteCommand();
            }
            else
            {
                isExecuteable = false;
            }

            SetCompleteFlagAfterExecuteCommand();
            return isExecuteable;
        }

        private void AssignDataTransfer(object dataTransfer)
        {
            //Assign data to Cache
            _dataTransfer = new List<object>();
            var cast = dataTransfer as object[];
            if (cast != null)
            {
                foreach (object data in cast)
                {
                    _dataTransfer.Add(data);
                }
            }
            else if (dataTransfer != null)
            {
                _dataTransfer.Add(dataTransfer);
            }
        }

        public bool AlterExecute(object? dataTransfer)
        {
            ExecuteAlternativeCommand();
            SetCompleteFlagAfterExecuteCommand();
            return true;
        }

        /// <summary>
        /// Clear the data transfer
        /// </summary>
        private void ClearCache()
        {
            if (_dataTransfer != null)
            {
                _dataTransfer.Clear();
                _dataTransfer = null;
            }
        }


        /// <summary>
        /// Set completed flag for some command, because when some ExecuteVM() was call
        /// it may be async method, so should let inherited child overide the flag
        /// by their own.
        /// 
        /// And the flag will be true as default.
        /// </summary>
        protected abstract void SetCompleteFlagAfterExecuteCommand();


        /// <summary>
        /// The main method for executer, everything need to be executed will happen here
        /// </summary>
        protected abstract void ExecuteCommand();

        /// <summary>
        /// The main method for executer, everything need to be executed will happen here async
        /// </summary>
        protected abstract Task ExecuteCommandAsync();

        /// <summary>
        /// Check posibility of command with transfered data
        /// </summary>
        /// <param name="dataTransfer">data passed into executer</param>
        /// <returns>true if meet condition and execute the command</returns>
        protected abstract bool CanExecute(object? dataTransfer);


        /// <summary>
        /// The alternative method for executer, everything need to be executed will happen here
        /// </summary>
        protected abstract void ExecuteAlternativeCommand();


        /// <summary>
        /// Destroy a command executer, normally will clear the cache
        /// </summary>
        protected abstract void ExecuteOnDestroy();


        /// <summary>
        /// Cancel a command executer while it is running 
        /// </summary>
        protected abstract void ExecuteOnCancel();

    }
}
