using cyber_base.async_task;
using cyber_base.implement.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_base.implement.async_task
{
    public class SelfReferenceCancelableAsyncTask : BaseAsyncTask
    {
        private static Logger SRCATLogger = new Logger("SelfReferenceCancelableAsyncTask");

        private Func<SelfReferenceCancelableAsyncTask, CancellationTokenSource, AsyncTaskResult, Task<AsyncTaskResult>> _mainFunc;
        private Func<SelfReferenceCancelableAsyncTask, AsyncTaskResult, Task<AsyncTaskResult>>? _callback;
        private Func<SelfReferenceCancelableAsyncTask, bool>? _canExecute;

        private CancellationTokenSource _cancellationTokenSource;

        public Func<SelfReferenceCancelableAsyncTask, AsyncTaskResult, Task<AsyncTaskResult>>? CallbackHandler => _callback;
        public Func<SelfReferenceCancelableAsyncTask, bool>? CanExecute => _canExecute;
        public Func<SelfReferenceCancelableAsyncTask, CancellationTokenSource, AsyncTaskResult, Task<AsyncTaskResult>> MainFunc => _mainFunc;

        public SelfReferenceCancelableAsyncTask(
            Func<SelfReferenceCancelableAsyncTask, CancellationTokenSource, AsyncTaskResult, Task<AsyncTaskResult>> mainFunc
            , CancellationTokenSource cancellationTokenSource
            , Func<SelfReferenceCancelableAsyncTask, bool>? canExecute = null
            , Func<SelfReferenceCancelableAsyncTask, AsyncTaskResult, Task<AsyncTaskResult>>? callback = null
            , string name = ""
            , bool isEnableAutomaticallyReport = false
            , ulong estimatedTime = 0
            , ulong delayTime = 0
            , int reportDelay = 1000)
            : base(name, estimatedTime, delayTime, reportDelay)
        {
            _mainFunc = mainFunc;
            _canExecute = canExecute;
            _callback = callback;
            _cancellationTokenSource = cancellationTokenSource;
            _isEnableAutomaticallyReport = isEnableAutomaticallyReport;
        }

        public void SetCurrentProgress(double value)
        {
            if (!_isEnableAutomaticallyReport)
            {
                CurrentProgress = value;
            }
        }

        protected async override Task DoMainFunc()
        {
            await MainFunc.Invoke(this, _cancellationTokenSource, _result)
                .ContinueWith((task) =>
                {
                    HandleMainTaskException(task);
                });

            if (_cancellationTokenSource.IsCancellationRequested)
            {
                throw new OperationCanceledException("Task was aborted from user!");
            }
        }

        protected async override Task DoCallback()
        {
            if (CallbackHandler != null)
            {
                await CallbackHandler.Invoke(this, _result);
            }
        }

        protected override async Task DoWaitRestDelay(long rest)
        {
            await Task.Delay(Convert.ToInt32(rest)
                , _cancellationTokenSource.Token);
        }

        public override void Cancel()
        {
            try
            {
                lock (_cancellationTokenSource)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }
            }
            catch (Exception ex)
            {
                SRCATLogger.E(ex.ToString());
            }
        }

        protected async override Task DoDelayForReportTask()
        {
            await Task.Delay(_reportDelay
              , _cancellationTokenSource.Token);
        }

        protected override bool CanMainFuncExecute()
        {
            return CanExecute?.Invoke(this) ?? true;
        }

    }
}
