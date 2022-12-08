using cyber_base.async_task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace extension_manager_service.implement.ui_event_handler.async_tasks
{
    internal class CommonEMSParamAsyncTask : BaseEMSParamAsyncTask
    {
        private Action<object, AsyncTaskResult, CancellationTokenSource>? _mainAct;
        private Action<object, AsyncTaskResult>? _callBack;
        Func<object, AsyncTaskResult, CancellationTokenSource, Task>? _mainFunc;

        public CommonEMSParamAsyncTask(object param
            , string name
            , Action<object, AsyncTaskResult, CancellationTokenSource>? mainAct
            , Action<object, AsyncTaskResult>? callBack = null
            , ulong delayTime = 0
            , ulong estimateTime = 0)
            : base(param, name, delayTime: delayTime, estimatedTime: estimateTime)
        {

            _mainAct = mainAct;
            _callBack = callBack;
        }

        public CommonEMSParamAsyncTask(object param
            , string name
            , Func<object, AsyncTaskResult, CancellationTokenSource, Task> mainFunc
            , Action<object, AsyncTaskResult>? callBack = null
            , ulong delayTime = 0
            , ulong estimateTime = 0)
            : base(param, name, delayTime: delayTime, estimatedTime: estimateTime)
        {
            _mainFunc = mainFunc;
            _callBack = callBack;
        }

        protected override async Task DoAsyncMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            if (_mainFunc != null)
            {
                await _mainFunc.Invoke(param, result, token);
            }
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            _mainAct?.Invoke(param, result, token);
        }

        protected override void DoCallback(object param, AsyncTaskResult result)
        {
            _callBack?.Invoke(param, result);
        }

        protected override bool IsTaskPossible(object param)
        {
            return true;
        }
    }
}
