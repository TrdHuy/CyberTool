﻿using cyber_base.async_task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_base.implement.async_task
{
    public class ParamAsyncTask : BaseAsyncTask
    {
        private Func<object, AsyncTaskResult, Task<AsyncTaskResult>>? _callback;
        private Func<object, AsyncTaskResult, CancellationTokenSource, Task<AsyncTaskResult>> _mainFunc;
        private Func<object, bool>? _canExecute;
        private CancellationTokenSource _cancellationTokenSource;
        private object _param;

        public Func<object, AsyncTaskResult, Task<AsyncTaskResult>>? CallbackHandler => _callback;
        public Func<object, AsyncTaskResult, CancellationTokenSource, Task<AsyncTaskResult>> MainFunc => _mainFunc;
        public Func<object, bool>? CanExecute => _canExecute;

        public ParamAsyncTask(
            Func<object, AsyncTaskResult, CancellationTokenSource, Task<AsyncTaskResult>> mainFunc
            , CancellationTokenSource cancellationTokenSource
            , object param
            , Func<object, bool>? canExecute = null
            , Func<object, AsyncTaskResult, Task<AsyncTaskResult>>? callback = null
            , string name = ""
            , ulong estimatedTime = 0
            , ulong delayTime = 0
            , int reportDelay = 1000) : base(name, estimatedTime, delayTime, reportDelay)
        {
            _param = param;
            _mainFunc = mainFunc;
            _canExecute = canExecute;
            _callback = callback;
            _cancellationTokenSource = cancellationTokenSource;
        }

        protected async override Task DoMainFunc()
        {
            var canExecute = CanExecute?.Invoke(_param) ?? true;
            if (canExecute)
            {
                await MainFunc.Invoke(_param, _result, _cancellationTokenSource)
                    .ContinueWith((task) =>
                    {
                        HandleMainTaskException(task);
                    });

                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    throw new OperationCanceledException("Task was aborted from user!");
                }
            }
        }

        protected async override Task DoCallback()
        {
            if (CallbackHandler != null)
            {
                await CallbackHandler.Invoke(_param, _result);
            }
        }

        protected override async Task DoWaitRestDelay(long rest)
        {
            await Task.Delay(Convert.ToInt32(rest)
                , _cancellationTokenSource.Token);
        }

        public override void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        protected async override Task DoDelayForReportTask()
        {
            await Task.Delay(_reportDelay
               , _cancellationTokenSource.Token);
        }
    }
}
