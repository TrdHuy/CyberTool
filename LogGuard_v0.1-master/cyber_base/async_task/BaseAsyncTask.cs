using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_base.async_task
{
    public abstract class BaseAsyncTask : IAsyncTask
    {
        private ulong _delayTime;
        private bool _isCompleted;
        private bool _isCompletedCallback;
        private bool _isCanceled;
        protected AsyncTaskResult _result;
        private string _name;
        protected ulong _estimatedTime = 0;
        protected double _progress = 0d;
        protected int _reportDelay = 0;

        public ulong DelayTime => _delayTime;
        public AsyncTaskResult Result => _result;
        public bool IsCompletedCallback
        {
            get => _isCompletedCallback;
            protected set => _isCompletedCallback = value;
        }
        public bool IsCompleted
        {
            get => _isCompleted;
            protected set
            {
                var oldVal = _isCompleted;
                _isCompleted = value;

                if (oldVal != value)
                {
                    OnCompletedChanged?.Invoke(this, oldVal, value);
                }
            }
        }
        public bool IsCanceled
        {
            get => _isCanceled;
            protected set
            {
                var oldVal = _isCanceled;
                _isCanceled = value;

                if (oldVal != value)
                {
                    OncanceledChanged?.Invoke(this, oldVal, value);
                }
            }
        }
        public string Name => _name;
        public ulong EstimatedTime => _estimatedTime;
        public double CurrentProgress
        {
            get => _progress;
            protected set
            {
                var oldVal = _progress;
                _progress = value;

                if (oldVal != value)
                {
                    ProgressChanged?.Invoke(this, value);
                }
            }
        }


        public BaseAsyncTask(
             string name = ""
            , ulong estimatedTime = 0
            , ulong delayTime = 0
            , int reportDelay = 1000)
        {
            _name = name;
            _estimatedTime = estimatedTime;
            _delayTime = delayTime;
            _result = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);
            _reportDelay = reportDelay;
        }


        public event IsCompletedChangedHandler? OnCompletedChanged;
        public event IscanceldChangedHandler? OncanceledChanged;
        public event ProgressChangedHandler? ProgressChanged;

        public async Task<BaseAsyncTask> Execute(bool isAsyncCallback = false)
        {
            var asynTaskExecuteWatcher = Stopwatch.StartNew();
            var reportTask = Task.Run(() => DoReportTask(asynTaskExecuteWatcher));
            try
            {
                await Task.Run(DoMainFunc)
                .ContinueWith((t) =>
                {
                    asynTaskExecuteWatcher.Stop();
                    HandleMainTaskException(t);
                });
                long restLoadingTime = (long)DelayTime - asynTaskExecuteWatcher.ElapsedMilliseconds;
                if (restLoadingTime > 0)
                {
                    await DoWaitRestDelay(restLoadingTime);
                }
                IsCompleted = true;
                CurrentProgress = 100;

                if (isAsyncCallback)
                {
                    await Task.Run(DoCallback);
                }
                else
                {
                    await DoCallback();
                }
                IsCompletedCallback = true;
            }
            catch (OperationCanceledException oce)
            {
                _result.MesResult = MessageAsyncTaskResult.Aborted;
                IsCompleted = false;
                IsCanceled = true;
                if (isAsyncCallback)
                {
                    await Task.Run(DoCallback);
                }
                else
                {
                    await DoCallback();
                }
                IsCompletedCallback = true;
            }
            catch
            {
                IsCompleted = false;
                IsCanceled = true;
                IsCompletedCallback = false;
            }
            return this;
        }

        protected abstract Task DoMainFunc();
        protected abstract Task DoCallback();
        protected abstract Task DoWaitRestDelay(long rest);

        protected virtual async Task DoReportTask(Stopwatch taskWatcher)
        {
            if (EstimatedTime == 0)
            {
                return;
            }

            while (IsCompleted == false
                && IsCanceled == false
                && CurrentProgress < 100)
            {
                var per = Math.Round
                    ((double)(taskWatcher.ElapsedMilliseconds
                    / (double)EstimatedTime), 2);
                per = per > 1 ? 1 : per;
                CurrentProgress = per * 100;
                await DoDelayForReportTask();
            }
        }

        protected abstract Task DoDelayForReportTask();

        protected void HandleMainTaskException(Task task)
        {
            if (task.IsFaulted)
            {
                throw task.Exception ?? new AggregateException();
            }
            else if (task.IsCanceled)
            {
                throw new OperationCanceledException("Task was aborted from user!");
            }
        }

        public abstract void Cancel();

        //public static async IAsyncEnumerable<AsyncTaskResult> MultiCancelableExecute(
        //     BaseAsyncTask[] tasks
        //    , bool isAsyncCallback = false)
        //{
        //    foreach (var asyncTask in tasks)
        //    {
        //        var asyncTaskResult = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);
        //        try
        //        {
        //            var asynTaskExecuteWatcher = Stopwatch.StartNew();
        //            var canExecute = asyncTask.CanExecute?.Invoke() ?? true;

        //            if (canExecute)
        //            {
        //                try
        //                {
        //                    ///====================
        //                    /// Execute task
        //                    ///====================
        //                    asyncTaskResult = await Task.Run(async () =>
        //                    {
        //                        var res = await asyncTask.CancelableExecute?.Invoke(asyncTask._cancellationTokenSource.Token);
        //                        return res;
        //                    }, asyncTask._cancellationTokenSource.Token);


        //                    if (asyncTask._cancellationTokenSource.Token.IsCancellationRequested)
        //                    {
        //                        throw new OperationCanceledException();
        //                    }

        //                    if (asyncTaskResult != null && asyncTaskResult.MesResult == MessageAsyncTaskResult.Aborted)
        //                    {
        //                        throw new OperationCanceledException(asyncTaskResult.Messsage);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    asyncTask.IsCanceled = true;
        //                    asyncTaskResult.MesResult = MessageAsyncTaskResult.Aborted;
        //                }

        //                asynTaskExecuteWatcher.Stop();

        //                //
        //                //Console.WriteLine("Execute time = " + asynTaskExecuteWatcher.ElapsedMilliseconds + "(ms)");

        //                long restLoadingTime = asyncTask.DelayTime - asynTaskExecuteWatcher.ElapsedMilliseconds;
        //                if (restLoadingTime > 0 && !asyncTask.IsCanceled)
        //                {
        //                    try
        //                    {
        //                        await Task.Delay(Convert.ToInt32(restLoadingTime), asyncTask._cancellationTokenSource.Token);
        //                    }
        //                    catch
        //                    {
        //                        asyncTask.IsCanceled = true;
        //                    }
        //                }
        //                asyncTask.IsCompleted = true;

        //                ///====================
        //                /// Callback method
        //                ///====================
        //                if (isAsyncCallback)
        //                {
        //                    await Task.Run(() =>
        //                    {
        //                        asyncTask.CallbackHandler?.Invoke(asyncTaskResult);
        //                    });
        //                }
        //                else
        //                {
        //                    asyncTask.CallbackHandler?.Invoke(asyncTaskResult);
        //                }

        //                asyncTask.IsCompletedCallback = true;

        //            }

        //        }
        //        catch (OperationCanceledException)
        //        {
        //            asyncTask.IsCompletedCallback = false;
        //            asyncTask.IsCompleted = false;
        //            asyncTask.IsCanceled = true;
        //        }

        //        yield return asyncTaskResult;
        //    }
        //}

        public static string GetCurrentThreadInformation()
        {
            var taskName = "Main Task(Task #" + Task.CurrentId.ToString() + ")";
            String msg = null;
            Thread thread = Thread.CurrentThread;
            msg = String.Format("{0} thread information\n", taskName) +
                  String.Format("   Background: {0}\n", thread.IsBackground) +
                  String.Format("   Thread Pool: {0}\n", thread.IsThreadPoolThread) +
                  String.Format("   Thread ID: {0}\n", thread.ManagedThreadId);
            return msg;
        }
    }
}
