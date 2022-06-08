﻿using cyber_base.async_task;
using cyber_base.implement.async_task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.async_tasks
{
    internal abstract class BaseRTParamAsyncTask : ParamAsyncTask
    {
        private Action<AsyncTaskResult>? _completedCallback;
        
        public BaseRTParamAsyncTask(object param
            , string name
            , [Optional] Action<AsyncTaskResult>? completedCallback
            , [Optional] CancellationTokenSource cancellationTokenSource
            , [Optional] Func<object, AsyncTaskResult, CancellationTokenSource, Task<AsyncTaskResult>> mainFunc
            , Func<object, bool>? canExecute = null
            , Func<object, AsyncTaskResult, Task<AsyncTaskResult>>? callback = null
            , ulong estimatedTime = 0
            , ulong delayTime = 0
            , int reportDelay = 1000)
            : base(mainFunc, cancellationTokenSource, param, canExecute, callback, name, estimatedTime, delayTime, reportDelay)
        {
            _mainFunc = _DoMainTask;
            _canExecute = _IsTaskPossile;
            _callback = _DoCallback;
            _cancellationTokenSource = new CancellationTokenSource();
            _completedCallback = completedCallback;
        }

        private async Task<AsyncTaskResult> _DoCallback(object param, AsyncTaskResult result)
        {
            await DoCallback(param, result);
            _completedCallback?.Invoke(result);
            return result;
        }

        private async Task<AsyncTaskResult> _DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            await DoMainTask(param, result, token);
            return result;
        }

        private bool _IsTaskPossile(object param)
        {
            return IsTaskPossible(param);
        }

        protected abstract Task DoCallback(object param, AsyncTaskResult result);
        protected abstract Task DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token);
        protected abstract bool IsTaskPossible(object param);
    }
}
