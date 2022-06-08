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
        private bool _isCompleted;
        private bool _isFaulted;
        private bool _isCompletedCallback;
        private bool _isCanceled;
        protected AsyncTaskResult _result;
        protected string _name;
        protected ulong _delayTime = 0;
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
        public bool IsFaulted
        {
            get => _isFaulted;
            private set
            {
                _isFaulted = value;
            }
        }
        public bool IsCompleted
        {
            get => _isCompleted;
            private set
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
            private set
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
            //var reportTask = Task.Run(() => DoReportTask(asynTaskExecuteWatcher
            DoReportTask();
            try
            {
                // do main task
                await Task.Run(DoMainFunc)
                .ContinueWith((t) =>
                {
                    // stop clock
                    asynTaskExecuteWatcher.Stop();

                    // handle task exception
                    // only handle operation cancel exception
                    // exception will be thrown here
                    HandleMainTaskException(t);
                });
                long restLoadingTime = (long)DelayTime - asynTaskExecuteWatcher.ElapsedMilliseconds;
                if (restLoadingTime > 0)
                {
                    // delay the task if its execution duration smaller than inited delay time
                    await DoWaitRestDelay(restLoadingTime);
                }

                // set completed flag
                IsCompleted = true;

                // default set message result if it was not handled
                if(_result.MesResult == MessageAsyncTaskResult.Non)
                {
                    _result.MesResult = MessageAsyncTaskResult.Finished;
                }
                // update progress when it completed
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
                // when the task was canceled by user
                // the callback will be triggered
                _result.Messsage = oce.ToString();
                _result.MesResult = MessageAsyncTaskResult.Aborted;
                IsCompleted = false;
                IsFaulted = false;
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
            catch (Exception ex)
            {
                // when the task was canceled due to function exception
                // callback will not be triggered
                _result.Messsage = ex.ToString();
                _result.MesResult = MessageAsyncTaskResult.Aborted;
                IsCompleted = false;
                IsFaulted = true;
                IsCanceled = false;
                IsCompletedCallback = false;
            }
            return this;
        }

        protected abstract Task DoMainFunc();
        protected abstract Task DoCallback();
        protected abstract Task DoWaitRestDelay(long rest);

        protected virtual async Task DoReportTask()
        {
            var reportWatch = Stopwatch.StartNew();

            if (EstimatedTime == 0)
            {
                CurrentProgress = 100;
                return;
            }

            try
            {
                while (IsCompleted == false
                && IsCanceled == false
                && CurrentProgress < 100)
                {
                    var per = Math.Round
                        ((double)(reportWatch.ElapsedMilliseconds
                        / (double)EstimatedTime), 2);
                    per = per > 1 ? 1 : per;
                    CurrentProgress = per * 100;
                    await DoDelayForReportTask();
                }
            }
            catch
            {

            }
            finally
            {
                reportWatch.Stop();
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
