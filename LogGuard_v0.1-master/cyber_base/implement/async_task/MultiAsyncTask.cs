using cyber_base.async_task;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_base.implement.async_task
{
    public class MultiAsyncTask : BaseAsyncTask
    {
        private List<CancelableAsyncTask> _mainFuncs;
        private CancellationTokenSource _cancellationTokenSource;
        private List<AsyncTaskResult> _results;
        private Func<List<AsyncTaskResult>, AsyncTaskResult, Task<AsyncTaskResult>>? _callback;
        private CancelableAsyncTask? _currentExecuteTask;

        public Func<List<AsyncTaskResult>, AsyncTaskResult, Task<AsyncTaskResult>>? CallbackHandler => _callback;

        public MultiAsyncTask(
            List<CancelableAsyncTask> mainFunc
            , CancellationTokenSource cancellationTokenSource
            , Func<List<AsyncTaskResult>, AsyncTaskResult, Task<AsyncTaskResult>>? callback = null
            , string name = ""
            , ulong delayTime = 0
            , int reportDelay = 1000)
            : base(name, 0, delayTime, reportDelay)
        {
            _estimatedTime = 0;
            foreach (var ele in mainFunc)
            {
                _estimatedTime += ele.EstimatedTime;
            }
            _mainFuncs = mainFunc;
            _cancellationTokenSource = cancellationTokenSource;
            _results = new List<AsyncTaskResult>();
            _callback = callback;
        }

        public override void Cancel()
        {
            if (_currentExecuteTask != null
                && _currentExecuteTask.IsCompleted == false
                && _currentExecuteTask.IsCanceled == false)
            {
                _currentExecuteTask.Cancel();
            }
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        protected async override Task DoCallback()
        {
            if (CallbackHandler != null)
            {
                await CallbackHandler.Invoke(_results, _result);
            }
        }

        protected async override Task DoDelayForReportTask()
        {
            await Task.Delay(_reportDelay
            , _cancellationTokenSource.Token);
        }

        protected async override Task DoMainFunc()
        {
            foreach (var ele in _mainFuncs)
            {
                _currentExecuteTask = ele;
                await ele.Execute().ContinueWith((t) =>
                {
                    HandleMainTaskException(t);
                });

                _results.Add(ele.Result);
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    throw new OperationCanceledException("Task was aborted by user!");
                }
            }
        }

        protected async override Task DoWaitRestDelay(long rest)
        {
            await Task.Delay(Convert.ToInt32(rest)
                , _cancellationTokenSource.Token);
        }
    }
}
